using AdminService.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;

namespace AdminService.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var result = await _mediator.Send(new GetDashboardQuery());
        return Ok(ApiResponse<DashboardDto>.Ok(result));
    }

    [HttpGet("reports/tickets")]
    public async Task<IActionResult> TicketReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetTicketReportQuery(from, to));
        return Ok(ApiResponse<List<TicketReportItemDto>>.Ok(result));
    }

    [HttpGet("reports/claims")]
    public async Task<IActionResult> ClaimReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetClaimReportQuery(from, to));
        return Ok(ApiResponse<List<ClaimReportItemDto>>.Ok(result));
    }

    [HttpGet("reports/payments")]
    public async Task<IActionResult> PaymentReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetPaymentReportQuery(from, to));
        return Ok(ApiResponse<List<PaymentReportItemDto>>.Ok(result));
    }
}
