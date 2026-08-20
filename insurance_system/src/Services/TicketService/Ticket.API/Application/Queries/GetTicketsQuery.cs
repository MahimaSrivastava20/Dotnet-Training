using System;
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
    public record GetTicketsQuery(Guid UserId, UserRole Role) : IRequest<ApiResponse<List<TicketDetailsDto>>>;

    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, ApiResponse<List<TicketDetailsDto>>>
    {
        private readonly TicketDbContext _db;

        public GetTicketsQueryHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<TicketDetailsDto>>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            var query = _db.SupportTickets
                .Include(t => t.Comments)
                .Include(t => t.AuditLogs)
                .AsQueryable();

            if (request.Role == UserRole.Customer)
            {
                query = query.Where(t => t.CustomerId == request.UserId);
            }
            else if (request.Role == UserRole.ClaimsAdjuster)
            {
                query = query.Where(t => t.AssignedAdjusterId == request.UserId);
            }

            var list = await query
                .OrderByDescending(t => t.UpdatedAt)
                .Select(t => new TicketDetailsDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    CustomerId = t.CustomerId,
                    CustomerName = t.CustomerName,
                    PolicyNumber = t.PolicyNumber,
                    Subject = t.Subject,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    AssignedAdjusterId = t.AssignedAdjusterId,
                    AssignedAdjusterName = t.AssignedAdjusterName,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    Comments = t.Comments.Where(c => request.Role != UserRole.Customer || !c.IsInternal).Select(c => new TicketCommentDto
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
                    AuditLogs = t.AuditLogs.Select(a => new TicketAuditLogDto
                    {
                        Id = a.Id,
                        Action = a.Action,
                        PreviousStatus = a.PreviousStatus.HasValue ? a.PreviousStatus.Value.ToString() : null,
                        NewStatus = a.NewStatus.ToString(),
                        ChangedByName = a.ChangedByName,
                        ChangedAt = a.ChangedAt
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<List<TicketDetailsDto>>.Ok(list);
        }
    }
}
