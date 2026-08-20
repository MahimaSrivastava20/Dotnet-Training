using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Identity.API.Application.DTOs;
using Identity.API.Infrastructure.Data;
using Insurance.Shared.Models;

namespace Identity.API.Application.Queries
{
    public record GetProfileQuery(Guid UserId) : IRequest<ApiResponse<UserProfileDto>>;

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ApiResponse<UserProfileDto>>
    {
        private readonly IdentityDbContext _db;

        public GetProfileQueryHandler(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(new object[] { request.UserId }, cancellationToken);

            if (user == null)
            {
                return ApiResponse<UserProfileDto>.Fail("User profile not found.");
            }

            var dto = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return ApiResponse<UserProfileDto>.Ok(dto);
        }
    }
}
