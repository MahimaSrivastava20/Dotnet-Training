namespace SharedLibrary.Events;

public class PaymentCompletedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PolicyId { get; set; }
    public decimal Amount { get; set; }
    public bool IsSuccess { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
}
