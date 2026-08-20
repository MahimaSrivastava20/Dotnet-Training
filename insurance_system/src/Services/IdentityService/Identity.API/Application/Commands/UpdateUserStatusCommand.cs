using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Identity.API.Infrastructure.Data;
using Insurance.Shared.Models;

namespace Identity.API.Application.Commands
{
    public record UpdateUserStatusCommand(Guid UserId, bool IsActive) : IRequest<ApiResponse<bool>>;

    public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, ApiResponse<bool>>
    {
        private readonly IdentityDbContext _db;

        public UpdateUserStatusCommandHandler(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            if (user == null)
            {
                return ApiResponse<bool>.Fail("User not found.");
            }

            user.IsActive = request.IsActive;
            await _db.SaveChangesAsync(cancellationToken);

            var status = request.IsActive ? "activated" : "deactivated";
            return ApiResponse<bool>.Ok(true, $"User account has been {status}.");
        }
    }
}
