namespace SharedLibrary.Events;

public class TicketAssignedEvent : BaseEvent
{
    public Guid TicketId { get; set; }
    public Guid AssignedToUserId { get; set; }
    public Guid CustomerId { get; set; }
    public string TicketTitle { get; set; } = string.Empty;
}
