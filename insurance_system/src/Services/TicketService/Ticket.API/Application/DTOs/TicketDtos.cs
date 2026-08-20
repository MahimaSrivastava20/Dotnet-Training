using System;
using System.Collections.Generic;
using Insurance.Shared.Enums;

namespace Ticket.API.Application.DTOs
{
    public class CreateTicketDto
    {
        public string? PolicyNumber { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AssignTicketDto
    {
        public Guid AdjusterId { get; set; }
        public string AdjusterName { get; set; } = string.Empty;
    }

    public class UpdateTicketStatusDto
    {
        public TicketStatus Status { get; set; }
    }

    public class AddCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false;
    }

    public class TicketCommentDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AuthorUserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TicketAuditLogDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? PreviousStatus { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }

    public class TicketDetailsDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? AssignedAdjusterId { get; set; }
        public string? AssignedAdjusterName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TicketCommentDto> Comments { get; set; } = new();
        public List<TicketAuditLogDto> AuditLogs { get; set; } = new();
    }

    public class AdminDashboardMetricsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public Dictionary<string, int> TicketsByStatus { get; set; } = new();
        public Dictionary<string, int> AdjusterWorkload { get; set; } = new();
    }
}
