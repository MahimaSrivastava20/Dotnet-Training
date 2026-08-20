using SharedLibrary.Events;
using SharedLibrary.Messaging;
using TicketService.DTOs;
using TicketService.Models;
using TicketService.Repositories;

namespace TicketService.Services;

public interface ITicketService
{
    Task<TicketResponseDto?> CreateAsync(CreateTicketDto dto, Guid customerId);
    Task<List<TicketResponseDto>> GetAllAsync(Guid userId, string role);
    Task<TicketResponseDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateStatusAsync(Guid id, string status, Guid userId, string role);
    Task<bool> AssignAsync(Guid id, Guid assignedTo);
}

public interface ICommentService
{
    Task<CommentDto?> AddCommentAsync(Guid ticketId, AddCommentDto dto, Guid userId, string userName);
    Task<List<CommentDto>> GetCommentsAsync(Guid ticketId);
}

public interface IClaimService
{
    Task<bool> ApproveClaimAsync(Guid ticketId, Guid approverId);
    Task<bool> RejectClaimAsync(Guid ticketId, ClaimActionDto dto, Guid rejecterId);
}

public class TicketManagementService : ITicketService
{
    private readonly ITicketRepository _repo;
    private readonly IClaimRepository _claimRepo;
    private readonly IRabbitMQPublisher _publisher;

    public TicketManagementService(ITicketRepository repo, IClaimRepository claimRepo, IRabbitMQPublisher publisher)
    {
        _repo = repo;
        _claimRepo = claimRepo;
        _publisher = publisher;
    }

    public async Task<TicketResponseDto?> CreateAsync(CreateTicketDto dto, Guid customerId)
    {
        if (!Enum.TryParse<TicketType>(dto.Type, true, out var type)) return null;

        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            Type = type,
            CustomerId = customerId,
            PolicyId = dto.PolicyId
        };
        await _repo.AddAsync(ticket);

        if (type == TicketType.Claim && dto.ClaimAmount.HasValue)
        {
            var claim = new ClaimDetails
            {
                TicketId = ticket.TicketId,
                ClaimAmount = dto.ClaimAmount.Value,
                Documents = dto.Documents ?? string.Empty
            };
            await _claimRepo.AddAsync(claim);
        }

        try { _publisher.Publish(new TicketCreatedEvent { TicketId = ticket.TicketId, Title = ticket.Title, Type = ticket.Type.ToString(), CustomerId = customerId }, "ticket.created"); } catch { }

        return await MapAsync(ticket);
    }

    public async Task<List<TicketResponseDto>> GetAllAsync(Guid userId, string role)
    {
        var customerId = role == "Customer" ? userId : (Guid?)null;
        var tickets = await _repo.GetAllAsync(customerId, role);
        var result = new List<TicketResponseDto>();
        foreach (var t in tickets) result.Add(await MapAsync(t));
        return result;
    }

    public async Task<TicketResponseDto?> GetByIdAsync(Guid id)
    {
        var ticket = await _repo.GetByIdAsync(id, true);
        return ticket == null ? null : await MapAsync(ticket);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status, Guid userId, string role)
    {
        var ticket = await _repo.GetByIdAsync(id);
        if (ticket == null) return false;
        if (role == "ClaimsSpecialist" && ticket.Type != TicketType.Claim) return false;
        if (role == "SupportSpecialist" && ticket.Type != TicketType.Support) return false;
        if (!Enum.TryParse<TicketStatus>(status, true, out var newStatus)) return false;

        ticket.Status = newStatus;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(ticket);
        try { _publisher.Publish(new TicketUpdatedEvent { TicketId = ticket.TicketId, NewStatus = newStatus.ToString(), CustomerId = ticket.CustomerId, TicketTitle = ticket.Title }, "ticket.updated"); } catch { }
        return true;
    }

    public async Task<bool> AssignAsync(Guid id, Guid assignedTo)
    {
        var ticket = await _repo.GetByIdAsync(id);
        if (ticket == null) return false;
        ticket.AssignedTo = assignedTo;
        ticket.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(ticket);
        try { _publisher.Publish(new TicketAssignedEvent { TicketId = ticket.TicketId, AssignedToUserId = assignedTo, CustomerId = ticket.CustomerId, TicketTitle = ticket.Title }, "ticket.assigned"); } catch { }
        return true;
    }

    private async Task<TicketResponseDto> MapAsync(Ticket t)
    {
        var claim = t.ClaimDetails ?? await _claimRepo.GetByTicketIdAsync(t.TicketId);
        return new TicketResponseDto
        {
            TicketId = t.TicketId,
            Title = t.Title,
            Description = t.Description,
            Type = t.Type.ToString(),
            Status = t.Status.ToString(),
            CustomerId = t.CustomerId,
            AssignedTo = t.AssignedTo,
            PolicyId = t.PolicyId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            ClaimDetails = claim == null ? null : new ClaimDetailsDto
            {
                ClaimId = claim.ClaimId,
                ClaimAmount = claim.ClaimAmount,
                Documents = claim.Documents,
                ApprovalStatus = claim.ApprovalStatus.ToString(),
                RejectionReason = claim.RejectionReason
            },
            Comments = t.Comments.Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                UserId = c.UserId,
                UserName = c.UserName,
                Message = c.Message,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }
}

public class CommentManagementService : ICommentService
{
    private readonly ICommentRepository _repo;
    private readonly ITicketRepository _ticketRepo;

    public CommentManagementService(ICommentRepository repo, ITicketRepository ticketRepo)
    {
        _repo = repo;
        _ticketRepo = ticketRepo;
    }

    public async Task<CommentDto?> AddCommentAsync(Guid ticketId, AddCommentDto dto, Guid userId, string userName)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId);
        if (ticket == null) return null;

        var comment = new Comment
        {
            TicketId = ticketId,
            UserId = userId,
            UserName = userName,
            Message = dto.Message
        };
        await _repo.AddAsync(comment);

        return new CommentDto
        {
            CommentId = comment.CommentId,
            UserId = comment.UserId,
            UserName = comment.UserName,
            Message = comment.Message,
            CreatedAt = comment.CreatedAt
        };
    }

    public async Task<List<CommentDto>> GetCommentsAsync(Guid ticketId)
    {
        var comments = await _repo.GetByTicketIdAsync(ticketId);
        return comments.Select(c => new CommentDto
        {
            CommentId = c.CommentId,
            UserId = c.UserId,
            UserName = c.UserName,
            Message = c.Message,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}

public class ClaimManagementService : IClaimService
{
    private readonly IClaimRepository _claimRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IRabbitMQPublisher _publisher;

    public ClaimManagementService(IClaimRepository claimRepo, ITicketRepository ticketRepo, IRabbitMQPublisher publisher)
    {
        _claimRepo = claimRepo;
        _ticketRepo = ticketRepo;
        _publisher = publisher;
    }

    public async Task<bool> ApproveClaimAsync(Guid ticketId, Guid approverId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId);
        if (ticket == null || ticket.Type != TicketType.Claim) return false;

        var claim = await _claimRepo.GetByTicketIdAsync(ticketId);
        if (claim == null) return false;

        claim.ApprovalStatus = ApprovalStatus.Approved;
        ticket.Status = TicketStatus.Resolved;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _claimRepo.UpdateAsync(claim);
        await _ticketRepo.UpdateAsync(ticket);

        try { _publisher.Publish(new ClaimApprovedEvent { TicketId = ticketId, ClaimId = claim.ClaimId, CustomerId = ticket.CustomerId, ClaimAmount = claim.ClaimAmount, PolicyId = ticket.PolicyId }, "claim.approved"); } catch { }

        // Send internal HTTP request to guarantee deduction if RabbitMQ is not running
        if (ticket.PolicyId.HasValue)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("X-Internal-Key", "InsuranceInternalKey2024");
                var content = new System.Net.Http.StringContent(
                    System.Text.Json.JsonSerializer.Serialize(new { CustomerPolicyId = ticket.PolicyId.Value, Amount = claim.ClaimAmount }),
                    System.Text.Encoding.UTF8,
                    "application/json");
                await client.PostAsync("http://localhost:5003/policies/internal/deduct", content);
            }
            catch { }
        }

        return true;
    }

    public async Task<bool> RejectClaimAsync(Guid ticketId, ClaimActionDto dto, Guid rejecterId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId);
        if (ticket == null || ticket.Type != TicketType.Claim) return false;

        var claim = await _claimRepo.GetByTicketIdAsync(ticketId);
        if (claim == null) return false;

        claim.ApprovalStatus = ApprovalStatus.Rejected;
        claim.RejectionReason = dto.Reason;
        ticket.Status = TicketStatus.Closed;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _claimRepo.UpdateAsync(claim);
        await _ticketRepo.UpdateAsync(ticket);

        try { _publisher.Publish(new ClaimRejectedEvent { TicketId = ticketId, ClaimId = claim.ClaimId, CustomerId = ticket.CustomerId, Reason = dto.Reason ?? "No reason provided" }, "claim.rejected"); } catch { }
        return true;
    }
}
