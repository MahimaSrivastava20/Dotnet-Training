using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs;
using PaymentService.Services;
using Razorpay.Api;
using SharedLibrary.DTOs;
using System.Security.Claims;

namespace PaymentService.Controllers;

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly IConfiguration _config;

    public PaymentsController(IPaymentService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetRole() => User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "";

    /// <summary>Creates a Razorpay order and returns the order_id + key for frontend checkout</summary>
    [HttpPost("create-order")]
    [Authorize(Roles = "Customer")]
    public IActionResult CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            var keyId = _config["Razorpay:KeyId"]!;
            var keySecret = _config["Razorpay:KeySecret"]!;
            var currency = _config["Razorpay:Currency"] ?? "INR";

            var client = new RazorpayClient(keyId, keySecret);

            // Razorpay amount is in paise (multiply rupees by 100)
            var amountInPaise = (int)(dto.Amount * 1.18m * 100);

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", currency },
                { "receipt", $"receipt_{Guid.NewGuid():N}".Substring(0, 20) },
                { "notes", new Dictionary<string, string> {
                    { "policyId", dto.PolicyId.ToString() },
                    { "customerId", GetUserId().ToString() }
                }}
            };

            var order = client.Order.Create(options);
            var orderId = order["id"].ToString();

            return Ok(ApiResponse<RazorpayOrderDto>.Ok(new RazorpayOrderDto
            {
                OrderId = orderId!,
                Amount = amountInPaise,
                Currency = currency,
                KeyId = keyId,
                PolicyId = dto.PolicyId,
                PolicyName = dto.PolicyName
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail($"Failed to create order: {ex.Message}"));
        }
    }

    /// <summary>Verify Razorpay payment signature and record the payment</summary>
    [HttpPost("verify")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Verify([FromBody] VerifyPaymentDto dto)
    {
        try
        {
            var keySecret = _config["Razorpay:KeySecret"]!;
            var payload = $"{dto.OrderId}|{dto.PaymentId}";
            using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(keySecret));
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            var computedSig = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (computedSig != dto.Signature)
                return BadRequest(ApiResponse.Fail("Payment signature verification failed"));

            // Record payment in our DB
            var result = await _service.ProcessPaymentAsync(new ProcessPaymentDto
            {
                PolicyId = dto.PolicyId,
                Amount = dto.Amount,
                RazorpayPaymentId = dto.PaymentId,
                RazorpayOrderId = dto.OrderId
            }, GetUserId());

            return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment verified and recorded"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.Fail($"Verification error: {ex.Message}"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Process([FromBody] ProcessPaymentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _service.ProcessPaymentAsync(dto, GetUserId());
        return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment processed successfully"));
    }

    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> MyPayments()
    {
        var result = await _service.GetMyPaymentsAsync(GetUserId());
        return Ok(ApiResponse<List<PaymentResponseDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment = await _service.GetByIdAsync(id);
        if (payment == null) return NotFound(ApiResponse.Fail("Payment not found"));

        var role = GetRole();
        if (role != "Admin" && payment.CustomerId != GetUserId())
            return Forbid();

        return Ok(ApiResponse<PaymentResponseDto>.Ok(payment));
    }
}
