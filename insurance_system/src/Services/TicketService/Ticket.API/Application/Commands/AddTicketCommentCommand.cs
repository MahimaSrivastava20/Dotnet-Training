using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ticket.API.Domain.Entities;
using Ticket.API.Infrastructure.Data;
using Ticket.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Ticket.API.Application.Commands
{
    public record AddTicketCommentCommand(Guid TicketId, Guid UserId, string UserName, string UserRole, AddCommentDto Dto) : IRequest<ApiResponse<TicketCommentDto>>;

    public class AddTicketCommentCommandHandler : IRequestHandler<AddTicketCommentCommand, ApiResponse<TicketCommentDto>>
    {
        private readonly TicketDbContext _db;

        public AddTicketCommentCommandHandler(TicketDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<TicketCommentDto>> Handle(AddTicketCommentCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _db.SupportTickets.FindAsync(new object[] { request.TicketId }, cancellationToken);
            if (ticket == null)
            {
                return ApiResponse<TicketCommentDto>.Fail("Ticket not found.");
            }

            var comment = new TicketComment
            {
                TicketId = request.TicketId,
                AuthorUserId = request.UserId,
                AuthorName = request.UserName,
                AuthorRole = request.UserRole,
                CommentText = request.Dto.CommentText,
                IsInternal = request.Dto.IsInternal,
                CreatedAt = DateTime.UtcNow
            };

            ticket.UpdatedAt = DateTime.UtcNow;

            _db.TicketComments.Add(comment);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new TicketCommentDto
            {
                Id = comment.Id,
                TicketId = comment.TicketId,
                AuthorUserId = comment.AuthorUserId,
                AuthorName = comment.AuthorName,
                AuthorRole = comment.AuthorRole,
                CommentText = comment.CommentText,
                IsInternal = comment.IsInternal,
                CreatedAt = comment.CreatedAt
            };

            return ApiResponse<TicketCommentDto>.Ok(result, "Comment added to ticket thread.");
        }
    }
}
