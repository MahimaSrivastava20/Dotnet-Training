using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyService.DTOs;
using PolicyService.Services;
using SharedLibrary.DTOs;
using System.Security.Claims;

namespace PolicyService.Controllers;

[ApiController]
[Route("policies")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _service;
    public PoliciesController(IPolicyService service) => _service = service;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var policies = await _service.GetAllAsync();
        return Ok(ApiResponse<List<PolicyResponseDto>>.Ok(policies));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var p = await _service.GetByIdAsync(id);
        if (p == null) return NotFound(ApiResponse.Fail("Policy not found"));
        return Ok(ApiResponse<PolicyResponseDto>.Ok(p));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePolicyDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _service.CreateAsync(dto);
        return Ok(ApiResponse<PolicyResponseDto>.Ok(result, "Policy created"));
    }

    [HttpPost("purchase")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Purchase([FromBody] PurchasePolicyDto dto)
    {
        var result = await _service.PurchaseAsync(dto.PolicyId, GetUserId());
        if (result == null) return BadRequest(ApiResponse.Fail("Policy not found or inactive"));
        return Ok(ApiResponse<CustomerPolicyResponseDto>.Ok(result, "Policy purchase initiated. Complete payment to activate."));
    }

    [HttpPost("renew/{customerPolicyId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Renew(Guid customerPolicyId)
    {
        var result = await _service.RenewAsync(customerPolicyId, GetUserId());
        if (result == null) return NotFound(ApiResponse.Fail("Customer policy not found"));
        return Ok(ApiResponse<CustomerPolicyResponseDto>.Ok(result, "Policy renewed"));
    }

    [HttpGet("my-policies")]
    [Authorize]
    public async Task<IActionResult> MyPolicies()
    {
        var result = await _service.GetMyPoliciesAsync(GetUserId());
        return Ok(ApiResponse<List<CustomerPolicyResponseDto>>.Ok(result));
    }

    /// <summary>Internal endpoint for PaymentService to activate a policy after successful payment (no RabbitMQ needed)</summary>
    [HttpPost("activate")]
    [AllowAnonymous]
    public async Task<IActionResult> Activate([FromBody] ActivatePolicyDto dto, [FromHeader(Name = "X-Internal-Key")] string? internalKey)
    {
        if (internalKey != "InsuranceInternalKey2024")
            return Unauthorized(ApiResponse.Fail("Unauthorized internal call"));

        await _service.ActivatePolicyAsync(dto.CustomerId, dto.PolicyId);
        return Ok(ApiResponse.Ok("Policy activated"));
    }

    /// <summary>Internal endpoint for TicketService to deduct claim amount after approval</summary>
    [HttpPost("internal/deduct")]
    [AllowAnonymous]
    public async Task<IActionResult> InternalDeduct([FromBody] InternalDeductDto dto, [FromHeader(Name = "X-Internal-Key")] string? internalKey)
    {
        if (internalKey != "InsuranceInternalKey2024")
            return Unauthorized(ApiResponse.Fail("Unauthorized internal call"));

        var success = await _service.DeductClaimAsync(dto.CustomerPolicyId, dto.Amount);
        if (!success) return BadRequest(ApiResponse.Fail("Failed to deduct claim amount"));
        return Ok(ApiResponse.Ok("Claim amount deducted successfully"));
    }

    /// <summary>Fix existing stuck PendingPayment policies — call once to repair old data</summary>
    [HttpPost("fix-pending")]
    [AllowAnonymous]
    public async Task<IActionResult> FixPending([FromHeader(Name = "X-Internal-Key")] string? internalKey)
    {
        if (internalKey != "InsuranceInternalKey2024")
            return Unauthorized(ApiResponse.Fail("Unauthorized internal call"));

        var count = await _service.ActivateAllPendingAsync();
        return Ok(ApiResponse<int>.Ok(count, $"{count} policies activated."));
    }

    [HttpPost("customer/{customerPolicyId}/deduct-claim")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeductClaim(Guid customerPolicyId, [FromBody] DeductClaimDto dto)
    {
        var result = await _service.DeductClaimAsync(customerPolicyId, dto.Amount);
        if (!result) return BadRequest(ApiResponse.Fail("Insufficient coverage amount or policy not found."));
        return Ok(ApiResponse.Ok("Coverage amount deducted successfully."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePolicyDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound(ApiResponse.Fail("Policy not found"));
        return Ok(ApiResponse<PolicyResponseDto>.Ok(result, "Policy updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound(ApiResponse.Fail("Policy not found"));
        return Ok(ApiResponse.Ok("Policy deleted"));
    }
}
