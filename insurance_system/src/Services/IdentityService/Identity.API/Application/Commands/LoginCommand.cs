using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Identity.API.Infrastructure.Data;
using Identity.API.Application.DTOs;
using Insurance.Shared.Models;
using Insurance.Shared.Security;
using Microsoft.Extensions.Options;

namespace Identity.API.Application.Commands
{
    public record LoginCommand(LoginDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IdentityDbContext _db;
        private readonly JwtSettings _jwtSettings;

        public LoginCommandHandler(IdentityDbContext db, IOptions<JwtSettings> jwtSettings)
        {
            _db = db;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Dto.Email.ToLower(), cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.Fail("User account has been deactivated. Please contact an administrator.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            var token = JwtTokenHelper.GenerateToken(user.Id, user.Email, user.FullName, user.Role, _jwtSettings);

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)
            };

            return ApiResponse<AuthResponseDto>.Ok(response, "Login successful.");
        }
    }
}
