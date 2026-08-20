using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket.API.Application.Commands;
using Ticket.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Ticket.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/tickets")]
    [Authorize(Roles = "Admin")]
    public class AdminTicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminTicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/assign")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignTicket(Guid id, [FromBody] AssignTicketDto dto)
        {
            var adminUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adminName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

            if (!Guid.TryParse(adminUserIdClaim, out var adminUserId)) return Unauthorized();

            var result = await _mediator.Send(new AssignTicketCommand(id, dto.AdjusterId, dto.AdjusterName, adminUserId, adminName));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
