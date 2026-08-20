using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Identity.API.Application.DTOs;
using Identity.API.Infrastructure.Data;
using Insurance.Shared.Models;

namespace Identity.API.Application.Queries
{
    public record GetAllUsersQuery : IRequest<ApiResponse<List<UserProfileDto>>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserProfileDto>>>
    {
        private readonly IdentityDbContext _db;

        public GetAllUsersQueryHandler(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<UserProfileDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _db.Users
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<List<UserProfileDto>>.Ok(users);
        }
    }
}
