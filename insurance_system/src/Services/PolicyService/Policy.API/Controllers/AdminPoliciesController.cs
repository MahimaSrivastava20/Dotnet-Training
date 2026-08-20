using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Policy.API.Application.Commands;
using Policy.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Policy.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/policies")]
    [Authorize(Roles = "Admin")]
    public class AdminPoliciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminPoliciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PolicyCatalogDto>>> CreatePolicyCatalog([FromBody] CreatePolicyCatalogDto dto)
        {
            var result = await _mediator.Send(new CreatePolicyCatalogCommand(dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePolicyCatalog(Guid id)
        {
            var result = await _mediator.Send(new DeletePolicyCatalogCommand(id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
