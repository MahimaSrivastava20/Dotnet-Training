namespace SharedLibrary.Events;

public class TicketUpdatedEvent : BaseEvent
{
    public Guid TicketId { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string TicketTitle { get; set; } = string.Empty;
}
