using IdentityService.DTOs;
using IdentityService.Services;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;

namespace IdentityService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _auth.RegisterAsync(dto);
        if (result == null) return Conflict(ApiResponse.Fail("Email already in use"));
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registration successful"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _auth.LoginAsync(dto);
        if (result == null) return Unauthorized(ApiResponse.Fail("Invalid credentials or account deactivated"));
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _auth.VerifyEmailAsync(dto.Email, dto.Code);
        if (!result) return BadRequest(ApiResponse.Fail("Invalid verification code"));
        return Ok(ApiResponse.Ok("Email verified successfully"));
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _auth.ResendVerificationAsync(dto.Email);
        if (!result) return NotFound(ApiResponse.Fail("User not found"));
        return Ok(ApiResponse.Ok("Verification code sent"));
    }
}
