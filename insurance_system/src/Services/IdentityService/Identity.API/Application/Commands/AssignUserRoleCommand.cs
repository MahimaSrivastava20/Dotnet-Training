using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Identity.API.Infrastructure.Data;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Identity.API.Application.Commands
{
    public record AssignUserRoleCommand(Guid UserId, UserRole Role) : IRequest<ApiResponse<bool>>;

    public class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, ApiResponse<bool>>
    {
        private readonly IdentityDbContext _db;

        public AssignUserRoleCommandHandler(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("User not found.");
            }

            user.Role = request.Role;
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, $"Role updated to {request.Role} for user {user.Email}.");
        }
    }
}
