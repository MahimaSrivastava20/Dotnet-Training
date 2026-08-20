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
    public record CreateTicketCommand(Guid CustomerId, string CustomerName, CreateTicketDto Dto) : IRequest<ApiResponse<TicketDetailsDto>>;

    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, ApiResponse<TicketDetailsDto>>
    {
        private readonly TicketDbContext _db;

        public CreateTicketCommandHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<TicketDetailsDto>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticketNumber = $"TCK-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var ticket = new SupportTicket
            {
                TicketNumber = ticketNumber,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                PolicyNumber = request.Dto.PolicyNumber,
                Subject = request.Dto.Subject,
                Description = request.Dto.Description,
                Status = TicketStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var initialAudit = new TicketAuditLog
            {
                TicketId = ticket.Id,
                Action = "Support Ticket Created",
                PreviousStatus = null,
                NewStatus = TicketStatus.Created,
                ChangedByUserId = request.CustomerId,
                ChangedByName = request.CustomerName,
                ChangedAt = DateTime.UtcNow
            };

            ticket.AuditLogs.Add(initialAudit);

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new TicketDetailsDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerId = ticket.CustomerId,
                CustomerName = ticket.CustomerName,
                PolicyNumber = ticket.PolicyNumber,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };

            return ApiResponse<TicketDetailsDto>.Ok(result, $"Ticket created with Number: {ticket.TicketNumber}");
        }
    }
}
