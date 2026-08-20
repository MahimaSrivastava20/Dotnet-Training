using System.ComponentModel.DataAnnotations;

namespace TicketService.DTOs;

public class CreateTicketDto
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty; // "Support" or "Claim"
    public Guid? PolicyId { get; set; }
    // Claim-specific
    public decimal? ClaimAmount { get; set; }
    public string? Documents { get; set; }
}

public class UpdateTicketStatusDto
{
    [Required] public string Status { get; set; } = string.Empty;
}

public class AssignTicketDto
{
    [Required] public Guid AssignedTo { get; set; }
}

public class AddCommentDto
{
    [Required] public string Message { get; set; } = string.Empty;
}

public class ClaimActionDto
{
    public string? Reason { get; set; }
}

public class TicketResponseDto
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid? AssignedTo { get; set; }
    public Guid? PolicyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ClaimDetailsDto? ClaimDetails { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ClaimDetailsDto
{
    public Guid ClaimId { get; set; }
    public decimal ClaimAmount { get; set; }
    public string Documents { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}
