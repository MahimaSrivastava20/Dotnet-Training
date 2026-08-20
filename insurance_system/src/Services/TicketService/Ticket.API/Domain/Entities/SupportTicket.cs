using System;
using System.Collections.Generic;
using Insurance.Shared.Enums;

namespace Ticket.API.Domain.Entities
{
    public class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TicketNumber { get; set; } = string.Empty; // e.g. TCK-20260819-9876
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Created;
        public Guid? AssignedAdjusterId { get; set; }
        public string? AssignedAdjusterName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<TicketComment> Comments { get; set; } = new();
        public List<TicketAuditLog> AuditLogs { get; set; } = new();
    }
}
