namespace PaymentService.Models;

public enum PaymentStatus { Pending, Completed, Failed, Refunded }

public class Payment
{
    public Guid PaymentId { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid PolicyId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string TransactionReference { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
