namespace SharedLibrary.Events;

public class TicketCreatedEvent : BaseEvent
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
}
