using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.API.Application.Commands;
using Identity.API.Application.DTOs;
using Identity.API.Application.Queries;
using Insurance.Shared.Models;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserProfileDto>>>> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _mediator.Send(new AssignUserRoleCommand(dto.UserId, dto.Role));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateUserStatusCommand(id, dto.IsActive));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
