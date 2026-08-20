using IdentityService.DTOs;

namespace IdentityService.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> VerifyEmailAsync(string email, string code);
    Task<bool> ResendVerificationAsync(string email);
}
