using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ticket.API.Domain.Entities;
using Ticket.API.Infrastructure.Data;
using Ticket.API.Application.DTOs;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Ticket.API.Application.Commands
{
    public record AssignTicketCommand(Guid TicketId, Guid AdjusterId, string AdjusterName, Guid AdminUserId, string AdminName) : IRequest<ApiResponse<bool>>;

    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, ApiResponse<bool>>
    {
        private readonly TicketDbContext _db;

        public AssignTicketCommandHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _db.SupportTickets.FindAsync(new object[] { request.TicketId }, cancellationToken);
            if (ticket == null)
            {
                return ApiResponse<bool>.Fail("Ticket not found.");
            }

            var oldStatus = ticket.Status;
            ticket.AssignedAdjusterId = request.AdjusterId;
            ticket.AssignedAdjusterName = request.AdjusterName;
            ticket.Status = TicketStatus.Assigned;
            ticket.UpdatedAt = DateTime.UtcNow;

            var auditLog = new TicketAuditLog
            {
                TicketId = ticket.Id,
                Action = $"Ticket assigned to Claims Adjuster: {request.AdjusterName}",
                PreviousStatus = oldStatus,
                NewStatus = TicketStatus.Assigned,
                ChangedByUserId = request.AdminUserId,
                ChangedByName = request.AdminName,
                ChangedAt = DateTime.UtcNow
            };

            _db.TicketAuditLogs.Add(auditLog);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, $"Ticket {ticket.TicketNumber} assigned to {request.AdjusterName}.");
        }
    }
}
