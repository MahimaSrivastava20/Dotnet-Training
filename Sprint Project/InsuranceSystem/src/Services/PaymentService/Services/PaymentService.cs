using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;
using SharedLibrary.Events;
using SharedLibrary.Messaging;
using System.Text;
using System.Text.Json;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto, Guid customerId);
    Task<List<PaymentResponseDto>> GetMyPaymentsAsync(Guid customerId);
    Task<PaymentResponseDto?> GetByIdAsync(Guid id);
}

public class PaymentManagementService : IPaymentService
{
    private readonly PaymentDbContext _ctx;
    private readonly IRabbitMQPublisher _publisher;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentManagementService(PaymentDbContext ctx, IRabbitMQPublisher publisher, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _ctx = ctx;
        _publisher = publisher;
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto, Guid customerId)
    {
        // Simulate payment processing (always succeeds in demo)
        var payment = new Payment
        {
            CustomerId = customerId,
            PolicyId = dto.PolicyId,
            Amount = dto.Amount,
            Status = PaymentStatus.Completed,
            TransactionReference = dto.RazorpayPaymentId ?? ("SIM_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper())
        };

        await _ctx.Payments.AddAsync(payment);
        await _ctx.SaveChangesAsync();

        // Directly activate the policy in PolicyService (no RabbitMQ dependency)
        await ActivatePolicyDirectlyAsync(customerId, dto.PolicyId);

        // Also publish to RabbitMQ for notifications (best-effort)
        try
        {
            _publisher.Publish(new PaymentCompletedEvent
            {
                PaymentId = payment.PaymentId,
                CustomerId = customerId,
                PolicyId = dto.PolicyId,
                Amount = dto.Amount,
                IsSuccess = true,
                TransactionReference = payment.TransactionReference
            }, "payment.completed");
        }
        catch { }

        return Map(payment);
    }

    private async Task ActivatePolicyDirectlyAsync(Guid customerId, Guid policyId)
    {
        try
        {
            var policyServiceUrl = _config["PolicyService:BaseUrl"] ?? "http://localhost:5003";
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Internal-Key", "InsuranceInternalKey2024");

            var payload = JsonSerializer.Serialize(new { CustomerId = customerId, PolicyId = policyId });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            await client.PostAsync($"{policyServiceUrl}/policies/activate", content);
        }
        catch
        {
            // Silent fail — RabbitMQ consumer will handle it if available
        }
    }

    public async Task<List<PaymentResponseDto>> GetMyPaymentsAsync(Guid customerId) =>
        await _ctx.Payments.Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => Map(p)).ToListAsync();

    public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
    {
        var p = await _ctx.Payments.FindAsync(id);
        return p == null ? null : Map(p);
    }

    private static PaymentResponseDto Map(Payment p) => new()
    {
        PaymentId = p.PaymentId,
        CustomerId = p.CustomerId,
        PolicyId = p.PolicyId,
        Amount = p.Amount,
        Status = p.Status.ToString(),
        TransactionReference = p.TransactionReference,
        CreatedAt = p.CreatedAt
    };
}
