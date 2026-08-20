using System;
using Insurance.Shared.Enums;

namespace Ticket.API.Domain.Entities
{
    public class TicketAuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TicketId { get; set; }
        public string Action { get; set; } = string.Empty;
        public TicketStatus? PreviousStatus { get; set; }
        public TicketStatus NewStatus { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
