using System;
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
    public record GetTicketByIdQuery(Guid TicketId, Guid UserId, UserRole Role) : IRequest<ApiResponse<TicketDetailsDto>>;

    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, ApiResponse<TicketDetailsDto>>
    {
        private readonly TicketDbContext _db;

        public GetTicketByIdQueryHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<TicketDetailsDto>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _db.SupportTickets
                .Include(t => t.Comments)
                .Include(t => t.AuditLogs)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket == null)
            {
                return ApiResponse<TicketDetailsDto>.Fail("Ticket not found.");
            }

            if (request.Role == UserRole.Customer && ticket.CustomerId != request.UserId)
            {
                return ApiResponse<TicketDetailsDto>.Fail("Unauthorized access to ticket.");
            }

            var dto = new TicketDetailsDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                CustomerId = ticket.CustomerId,
                CustomerName = ticket.CustomerName,
                PolicyNumber = ticket.PolicyNumber,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                AssignedAdjusterId = ticket.AssignedAdjusterId,
                AssignedAdjusterName = ticket.AssignedAdjusterName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                Comments = ticket.Comments.Where(c => request.Role != UserRole.Customer || !c.IsInternal).Select(c => new TicketCommentDto
                {
                    Id = c.Id,
                    TicketId = c.TicketId,
                    AuthorUserId = c.AuthorUserId,
                    AuthorName = c.AuthorName,
                    AuthorRole = c.AuthorRole,
                    CommentText = c.CommentText,
                    IsInternal = c.IsInternal,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                AuditLogs = ticket.AuditLogs.Select(a => new TicketAuditLogDto
                {
                    Id = a.Id,
                    Action = a.Action,
                    PreviousStatus = a.PreviousStatus.HasValue ? a.PreviousStatus.Value.ToString() : null,
                    NewStatus = a.NewStatus.ToString(),
                    ChangedByName = a.ChangedByName,
                    ChangedAt = a.ChangedAt
                }).ToList()
            };

            return ApiResponse<TicketDetailsDto>.Ok(dto);
        }
    }
}
