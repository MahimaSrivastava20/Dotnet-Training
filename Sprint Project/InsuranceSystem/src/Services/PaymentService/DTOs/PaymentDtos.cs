using System.ComponentModel.DataAnnotations;

namespace PaymentService.DTOs;

public class ProcessPaymentDto
{
    [Required] public Guid PolicyId { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpayOrderId { get; set; }
}

public class CreateOrderDto
{
    [Required] public Guid PolicyId { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    public string PolicyName { get; set; } = string.Empty;
}

public class RazorpayOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public int Amount { get; set; } // in paise
    public string Currency { get; set; } = "INR";
    public string KeyId { get; set; } = string.Empty;
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
}

public class VerifyPaymentDto
{
    [Required] public string OrderId { get; set; } = string.Empty;
    [Required] public string PaymentId { get; set; } = string.Empty;
    [Required] public string Signature { get; set; } = string.Empty;
    [Required] public Guid PolicyId { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
