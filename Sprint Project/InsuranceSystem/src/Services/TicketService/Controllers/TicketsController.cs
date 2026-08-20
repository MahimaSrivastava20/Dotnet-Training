using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;
using System.Security.Claims;
using TicketService.DTOs;
using TicketService.Services;

namespace TicketService.Controllers;

[ApiController]
[Route("tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ICommentService _commentService;
    private readonly IClaimService _claimService;

    public TicketsController(ITicketService ts, ICommentService cs, IClaimService cls)
    {
        _ticketService = ts;
        _commentService = cs;
        _claimService = cls;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetRole() => User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "";
    private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _ticketService.CreateAsync(dto, GetUserId());
        if (result == null) return BadRequest(ApiResponse.Fail("Invalid ticket type"));
        return Ok(ApiResponse<TicketResponseDto>.Ok(result, "Ticket created"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync(GetUserId(), GetRole());
        return Ok(ApiResponse<List<TicketResponseDto>>.Ok(tickets));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null) return NotFound(ApiResponse.Fail("Ticket not found"));
        return Ok(ApiResponse<TicketResponseDto>.Ok(ticket));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "ClaimsSpecialist,SupportSpecialist,Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTicketStatusDto dto)
    {
        var success = await _ticketService.UpdateStatusAsync(id, dto.Status, GetUserId(), GetRole());
        if (!success) return BadRequest(ApiResponse.Fail("Cannot update status. Check ticket type vs role."));
        return Ok(ApiResponse.Ok("Status updated"));
    }

    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTicketDto dto)
    {
        var success = await _ticketService.AssignAsync(id, dto.AssignedTo);
        if (!success) return NotFound(ApiResponse.Fail("Ticket not found"));
        return Ok(ApiResponse.Ok("Ticket assigned"));
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _commentService.AddCommentAsync(id, dto, GetUserId(), GetUserName());
        if (result == null) return NotFound(ApiResponse.Fail("Ticket not found"));
        return Ok(ApiResponse<CommentDto>.Ok(result, "Comment added"));
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetComments(Guid id)
    {
        var comments = await _commentService.GetCommentsAsync(id);
        return Ok(ApiResponse<List<CommentDto>>.Ok(comments));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "ClaimsSpecialist,Admin")]
    public async Task<IActionResult> ApproveClaim(Guid id, [FromBody] ClaimActionDto? dto)
    {
        var success = await _claimService.ApproveClaimAsync(id, GetUserId());
        if (!success) return BadRequest(ApiResponse.Fail("Cannot approve. Not a claim ticket or claim not found."));
        return Ok(ApiResponse.Ok("Claim approved"));
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "ClaimsSpecialist,Admin")]
    public async Task<IActionResult> RejectClaim(Guid id, [FromBody] ClaimActionDto dto)
    {
        var success = await _claimService.RejectClaimAsync(id, dto, GetUserId());
        if (!success) return BadRequest(ApiResponse.Fail("Cannot reject. Not a claim ticket or claim not found."));
        return Ok(ApiResponse.Ok("Claim rejected"));
    }
}
