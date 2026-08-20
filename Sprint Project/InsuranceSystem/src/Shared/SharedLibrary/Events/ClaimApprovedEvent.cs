namespace SharedLibrary.Events;

public class ClaimApprovedEvent : BaseEvent
{
    public Guid TicketId { get; set; }
    public Guid ClaimId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal ClaimAmount { get; set; }
    public Guid? PolicyId { get; set; }
}
