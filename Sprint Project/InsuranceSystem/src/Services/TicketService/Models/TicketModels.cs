namespace TicketService.Models;

public enum TicketType { Support, Claim }
public enum TicketStatus { Open, InProgress, Resolved, Closed }
public enum ApprovalStatus { Pending, Approved, Rejected }

public class Ticket
{
    public Guid TicketId { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketType Type { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public Guid CustomerId { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? PolicyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ClaimDetails? ClaimDetails { get; set; }
}

public class Comment
{
    public Guid CommentId { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}

public class ClaimDetails
{
    public Guid ClaimId { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public decimal ClaimAmount { get; set; }
    public string Documents { get; set; } = string.Empty;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}
