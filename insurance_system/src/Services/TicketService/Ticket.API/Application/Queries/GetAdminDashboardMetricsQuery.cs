using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ticket.API.Infrastructure.Data;
using Ticket.API.Application.DTOs;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Ticket.API.Application.Queries
{
    public record GetAdminDashboardMetricsQuery : IRequest<ApiResponse<AdminDashboardMetricsDto>>;

    public class GetAdminDashboardMetricsQueryHandler : IRequestHandler<GetAdminDashboardMetricsQuery, ApiResponse<AdminDashboardMetricsDto>>
    {
        private readonly TicketDbContext _db;

        public GetAdminDashboardMetricsQueryHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<AdminDashboardMetricsDto>> Handle(GetAdminDashboardMetricsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _db.SupportTickets.ToListAsync(cancellationToken);

            var metrics = new AdminDashboardMetricsDto
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == TicketStatus.Created || t.Status == TicketStatus.Assigned),
                InProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress || t.Status == TicketStatus.Reopened),
                ResolvedTickets = tickets.Count(t => t.Status == TicketStatus.Resolved),
                ClosedTickets = tickets.Count(t => t.Status == TicketStatus.Closed),
                TicketsByStatus = tickets.GroupBy(t => t.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                AdjusterWorkload = tickets.Where(t => !string.IsNullOrEmpty(t.AssignedAdjusterName))
                                         .GroupBy(t => t.AssignedAdjusterName!)
                                         .ToDictionary(g => g.Key, g => g.Count())
            };

            return ApiResponse<AdminDashboardMetricsDto>.Ok(metrics, "Admin metrics retrieved.");
        }
    }
}
