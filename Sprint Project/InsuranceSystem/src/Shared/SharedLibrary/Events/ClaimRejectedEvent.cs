namespace SharedLibrary.Events;

public class ClaimRejectedEvent : BaseEvent
{
    public Guid TicketId { get; set; }
    public Guid ClaimId { get; set; }
    public Guid CustomerId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
