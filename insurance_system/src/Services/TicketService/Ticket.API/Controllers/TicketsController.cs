using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket.API.Application.Commands;
using Ticket.API.Application.DTOs;
using Ticket.API.Application.Queries;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Ticket.API.Controllers
{
    [ApiController]
    [Route("api/v1/tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketDetailsDto>>> CreateTicket([FromBody] CreateTicketDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Customer";

            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new CreateTicketCommand(userId, userName, dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TicketDetailsDto>>>> GetTickets()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!Guid.TryParse(userIdClaim, out var userId) || !Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetTicketsQuery(userId, role));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TicketDetailsDto>>> GetTicketById(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!Guid.TryParse(userIdClaim, out var userId) || !Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new GetTicketByIdQuery(id, userId, role));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateTicketStatusDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "User";

            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new UpdateTicketStatusCommand(id, dto.Status, userId, userName));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/comments")]
        public async Task<ActionResult<ApiResponse<TicketCommentDto>>> AddComment(Guid id, [FromBody] AddCommentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "User";
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Customer";

            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new AddTicketCommentCommand(id, userId, userName, userRole, dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
