using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket.API.Application.DTOs;
using Ticket.API.Application.Queries;
using Insurance.Shared.Models;

namespace Ticket.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ApiResponse<AdminDashboardMetricsDto>>> GetDashboardMetrics()
        {
            var result = await _mediator.Send(new GetAdminDashboardMetricsQuery());
            return Ok(result);
        }
    }
}
