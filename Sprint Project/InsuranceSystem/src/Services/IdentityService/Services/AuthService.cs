using IdentityService.DTOs;
using IdentityService.Models;
using IdentityService.Repositories;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Events;
using SharedLibrary.Messaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;
    private readonly IRabbitMQPublisher _publisher;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository repo, IConfiguration config, IRabbitMQPublisher publisher, IEmailService emailService)
    {
        _repo = repo;
        _config = config;
        _publisher = publisher;
        _emailService = emailService;
    }

    private string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _repo.EmailExistsAsync(dto.Email)) return null;

        var otp = GenerateOtp();
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Customer,
            OtpCode = otp,
            OtpExpiry = DateTime.UtcNow.AddMinutes(15),
            IsEmailVerified = false
        };
        await _repo.AddAsync(user);

        // Send OTP email in real-time
        await _emailService.SendEmailAsync(user.Email, user.Name, 
            "Your Verification Code - SafeHaven Insurance",
            $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto;text-align:center;">
              <h2 style="color:#6750A4;">Verify Your Email Address</h2>
              <p>Hi {user.Name},</p>
              <p>Thank you for registering. Please use the following code to verify your email address:</p>
              <div style="font-size:32px;font-weight:bold;letter-spacing:4px;color:#333;margin:24px 0;">{otp}</div>
              <p style="color:#666;">This code is valid for 15 minutes.</p>
              <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance</p>
            </div>
            """);

        // The user must verify the OTP before they get a token
        return new AuthResponseDto 
        { 
            Token = "pending_verification",
            Role = user.Role.ToString(),
            UserId = user.UserId,
            Name = user.Name,
            ExpiresAt = DateTime.UtcNow
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user == null || !user.IsActive) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;
        
        // Ensure email is verified
        if (!user.IsEmailVerified)
        {
            // The frontend might need a specific error, but returning null works for now
            return null;
        }

        return GenerateToken(user);
    }

    public async Task<bool> VerifyEmailAsync(string email, string code)
    {
        var user = await _repo.GetByEmailAsync(email);
        if (user == null) return false;
        
        if (user.IsEmailVerified) return true; // Already verified

        if (user.OtpCode == code && user.OtpExpiry > DateTime.UtcNow)
        {
            user.IsEmailVerified = true;
            user.OtpCode = null; // clear OTP
            user.OtpExpiry = null;
            await _repo.UpdateAsync(user);

            // Now that they are verified, we can publish the UserRegisteredEvent to NotificationService
            // so they get the official "Welcome" email from the queue.
            try
            {
                _publisher.Publish(new UserRegisteredEvent
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString()
                }, "user.registered");
            }
            catch { /* RabbitMQ optional */ }

            return true;
        }
        
        return false;
    }

    public async Task<bool> ResendVerificationAsync(string email)
    {
        var user = await _repo.GetByEmailAsync(email);
        if (user == null || user.IsEmailVerified) return false;
        
        var otp = GenerateOtp();
        user.OtpCode = otp;
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(15);
        await _repo.UpdateAsync(user);

        // Send OTP email again
        await _emailService.SendEmailAsync(user.Email, user.Name, 
            "Your New Verification Code - SafeHaven Insurance",
            $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto;text-align:center;">
              <h2 style="color:#6750A4;">Verify Your Email Address</h2>
              <p>Hi {user.Name},</p>
              <p>You requested a new verification code. Please use the following code:</p>
              <div style="font-size:32px;font-weight:bold;letter-spacing:4px;color:#333;margin:24px 0;">{otp}</div>
              <p style="color:#666;">This code is valid for 15 minutes.</p>
              <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance</p>
            </div>
            """);

        return true;
    }

    private AuthResponseDto GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("role", user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = user.Role.ToString(),
            UserId = user.UserId,
            Name = user.Name,
            ExpiresAt = expires
        };
    }
}
