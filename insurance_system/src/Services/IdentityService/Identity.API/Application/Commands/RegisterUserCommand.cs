using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Data;
using Identity.API.Application.DTOs;
using Insurance.Shared.Models;
using Insurance.Shared.Security;
using Microsoft.Extensions.Options;

namespace Identity.API.Application.Commands
{
    public record RegisterUserCommand(RegisterUserDto Dto) : IRequest<ApiResponse<AuthResponseDto>>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IdentityDbContext _db;
        private readonly JwtSettings _jwtSettings;

        public RegisterUserCommandHandler(IdentityDbContext db, IOptions<JwtSettings> jwtSettings)
        {
            _db = db;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken))
            {
                return ApiResponse<AuthResponseDto>.Fail("A user with this email address already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email.ToLower(),
                PasswordHash = passwordHash,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
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

            return ApiResponse<AuthResponseDto>.Ok(response, "User registered successfully.");
        }
    }
}
