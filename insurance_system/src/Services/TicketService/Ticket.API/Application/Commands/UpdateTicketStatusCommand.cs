using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ticket.API.Domain.Entities;
using Ticket.API.Infrastructure.Data;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Ticket.API.Application.Commands
{
    public record UpdateTicketStatusCommand(Guid TicketId, TicketStatus NewStatus, Guid UserId, string UserName) : IRequest<ApiResponse<bool>>;

    public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, ApiResponse<bool>>
    {
        private readonly TicketDbContext _db;

        public UpdateTicketStatusCommandHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _db.SupportTickets.FindAsync(new object[] { request.TicketId }, cancellationToken);
            if (ticket == null)
            {
                return ApiResponse<bool>.Fail("Ticket not found.");
            }

            var oldStatus = ticket.Status;
            ticket.Status = request.NewStatus;
            ticket.UpdatedAt = DateTime.UtcNow;

            var auditLog = new TicketAuditLog
            {
                TicketId = ticket.Id,
                Action = $"Status changed from {oldStatus} to {request.NewStatus}",
                PreviousStatus = oldStatus,
                NewStatus = request.NewStatus,
                ChangedByUserId = request.UserId,
                ChangedByName = request.UserName,
                ChangedAt = DateTime.UtcNow
            };

            _db.TicketAuditLogs.Add(auditLog);
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, $"Ticket status updated to {request.NewStatus}.");
        }
    }
}
