using Microsoft.EntityFrameworkCore;
using PolicyService.Data;
using PolicyService.DTOs;
using PolicyService.Models;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace PolicyService.Services;

public interface IPolicyService
{
    Task<List<PolicyResponseDto>> GetAllAsync();
    Task<PolicyResponseDto?> GetByIdAsync(Guid id);
    Task<PolicyResponseDto> CreateAsync(CreatePolicyDto dto);
    Task<PolicyResponseDto?> UpdateAsync(Guid id, CreatePolicyDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<CustomerPolicyResponseDto?> PurchaseAsync(Guid policyId, Guid customerId);
    Task<CustomerPolicyResponseDto?> RenewAsync(Guid customerPolicyId, Guid customerId);
    Task<List<CustomerPolicyResponseDto>> GetMyPoliciesAsync(Guid customerId);
    Task ActivatePolicyAsync(Guid customerId, Guid policyId);
    Task<int> ActivateAllPendingAsync();
    Task<bool> DeductClaimAsync(Guid customerPolicyId, decimal amount);
}

public class PolicyManagementService : IPolicyService
{
    private readonly PolicyDbContext _ctx;
    private readonly IRabbitMQPublisher _publisher;

    public PolicyManagementService(PolicyDbContext ctx, IRabbitMQPublisher publisher)
    {
        _ctx = ctx;
        _publisher = publisher;
    }

    public async Task<List<PolicyResponseDto>> GetAllAsync() =>
        await _ctx.Policies.Where(p => p.IsActive).Select(p => MapPolicy(p)).ToListAsync();

    public async Task<PolicyResponseDto?> GetByIdAsync(Guid id)
    {
        var policy = await _ctx.Policies.FindAsync(id);
        return policy == null ? null : MapPolicy(policy);
    }

    public async Task<PolicyResponseDto> CreateAsync(CreatePolicyDto dto)
    {
        if (!Enum.TryParse<PolicyType>(dto.Type, true, out var type))
            type = PolicyType.Health;

        var policy = new Policy
        {
            Name = dto.Name,
            Type = type,
            Premium = dto.Premium,
            CoverageAmount = dto.CoverageAmount,
            CoverageDetails = dto.CoverageDetails,
            Terms = dto.Terms
        };
        await _ctx.Policies.AddAsync(policy);
        await _ctx.SaveChangesAsync();
        return MapPolicy(policy);
    }

    public async Task<PolicyResponseDto?> UpdateAsync(Guid id, CreatePolicyDto dto)
    {
        var policy = await _ctx.Policies.FindAsync(id);
        if (policy == null) return null;

        if (!Enum.TryParse<PolicyType>(dto.Type, true, out var type))
            type = PolicyType.Health;

        policy.Name = dto.Name;
        policy.Type = type;
        policy.Premium = dto.Premium;
        policy.CoverageAmount = dto.CoverageAmount;
        policy.CoverageDetails = dto.CoverageDetails;
        policy.Terms = dto.Terms;

        await _ctx.SaveChangesAsync();
        return MapPolicy(policy);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var policy = await _ctx.Policies.FindAsync(id);
        if (policy == null) return false;

        // Soft delete
        policy.IsActive = false;
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<CustomerPolicyResponseDto?> PurchaseAsync(Guid policyId, Guid customerId)
    {
        var policy = await _ctx.Policies.FindAsync(policyId);
        if (policy == null || !policy.IsActive) return null;

        var cp = new CustomerPolicy
        {
            PolicyId = policyId,
            CustomerId = customerId,
            RemainingCoverageAmount = policy.CoverageAmount,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            Status = CustomerPolicyStatus.PendingPayment
        };
        await _ctx.CustomerPolicies.AddAsync(cp);
        await _ctx.SaveChangesAsync();
        return MapCustomerPolicy(cp, policy);
    }

    public async Task ActivatePolicyAsync(Guid customerId, Guid policyId)
    {
        var cp = await _ctx.CustomerPolicies
            .Include(c => c.Policy)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.PolicyId == policyId && c.Status == CustomerPolicyStatus.PendingPayment);

        if (cp == null) return;
        cp.Status = CustomerPolicyStatus.Active;
        await _ctx.SaveChangesAsync();

        try
        {
            _publisher.Publish(new PolicyPurchasedEvent
            {
                CustomerPolicyId = cp.CustomerPolicyId,
                PolicyId = cp.PolicyId,
                CustomerId = cp.CustomerId,
                PolicyName = cp.Policy.Name,
                StartDate = cp.StartDate,
                EndDate = cp.EndDate
            }, "policy.purchased");
        }
        catch { }
    }

    public async Task<CustomerPolicyResponseDto?> RenewAsync(Guid customerPolicyId, Guid customerId)
    {
        var cp = await _ctx.CustomerPolicies.Include(c => c.Policy)
            .FirstOrDefaultAsync(c => c.CustomerPolicyId == customerPolicyId && c.CustomerId == customerId);
        if (cp == null) return null;

        cp.StartDate = DateTime.UtcNow;
        cp.EndDate = DateTime.UtcNow.AddYears(1);
        cp.Status = CustomerPolicyStatus.Active;
        await _ctx.SaveChangesAsync();
        return MapCustomerPolicy(cp, cp.Policy);
    }

    public async Task<List<CustomerPolicyResponseDto>> GetMyPoliciesAsync(Guid customerId) =>
        await _ctx.CustomerPolicies.Include(c => c.Policy)
            .Where(c => c.CustomerId == customerId)
            .Select(c => MapCustomerPolicy(c, c.Policy))
            .ToListAsync();

    public async Task<int> ActivateAllPendingAsync()
    {
        var pending = await _ctx.CustomerPolicies
            .Include(c => c.Policy)
            .Where(c => c.Status == CustomerPolicyStatus.PendingPayment)
            .ToListAsync();

        foreach (var cp in pending)
        {
            cp.Status = CustomerPolicyStatus.Active;
        }
        await _ctx.SaveChangesAsync();

        // Publish events for newly activated policies
        foreach (var cp in pending)
        {
            try
            {
                _publisher.Publish(new PolicyPurchasedEvent
                {
                    CustomerPolicyId = cp.CustomerPolicyId,
                    PolicyId = cp.PolicyId,
                    CustomerId = cp.CustomerId,
                    PolicyName = cp.Policy.Name,
                    StartDate = cp.StartDate,
                    EndDate = cp.EndDate
                }, "policy.purchased");
            }
            catch { }
        }

        return pending.Count;
    }

    public async Task<bool> DeductClaimAsync(Guid customerPolicyId, decimal amount)
    {
        var cp = await _ctx.CustomerPolicies.FindAsync(customerPolicyId);
        if (cp == null || cp.RemainingCoverageAmount < amount) return false;
        
        cp.RemainingCoverageAmount -= amount;
        await _ctx.SaveChangesAsync();
        return true;
    }

    private static PolicyResponseDto MapPolicy(Policy p) => new()
    {
        PolicyId = p.PolicyId,
        Name = p.Name,
        Type = p.Type.ToString(),
        Premium = p.Premium,
        CoverageAmount = p.CoverageAmount,
        CoverageDetails = p.CoverageDetails,
        Terms = p.Terms,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };

    private static CustomerPolicyResponseDto MapCustomerPolicy(CustomerPolicy cp, Policy p) => new()
    {
        CustomerPolicyId = cp.CustomerPolicyId,
        PolicyId = cp.PolicyId,
        PolicyName = p.Name,
        PolicyType = p.Type.ToString(),
        Premium = p.Premium,
        CoverageAmount = p.CoverageAmount,
        RemainingCoverageAmount = cp.RemainingCoverageAmount,
        StartDate = cp.StartDate,
        EndDate = cp.EndDate,
        Status = cp.Status.ToString(),
        CreatedAt = cp.CreatedAt
    };
}
