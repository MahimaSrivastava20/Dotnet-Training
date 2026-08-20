using IdentityService.DTOs;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;

namespace IdentityService.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    public AdminController(IUserService userService) => _userService = userService;

    [HttpPost("create-claims-specialist")]
    public async Task<IActionResult> CreateClaimsSpecialist([FromBody] CreateSpecialistDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _userService.CreateSpecialistAsync(dto, UserRole.ClaimsSpecialist);
        if (result == null) return Conflict(ApiResponse.Fail("Email already in use"));
        return Ok(ApiResponse<UserResponseDto>.Ok(result, "Claims Specialist created"));
    }

    [HttpPost("create-support-specialist")]
    public async Task<IActionResult> CreateSupportSpecialist([FromBody] CreateSpecialistDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Validation failed"));
        var result = await _userService.CreateSpecialistAsync(dto, UserRole.SupportSpecialist);
        if (result == null) return Conflict(ApiResponse.Fail("Email already in use"));
        return Ok(ApiResponse<UserResponseDto>.Ok(result, "Support Specialist created"));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<List<UserResponseDto>>.Ok(users));
    }

    [HttpPut("users/{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var success = await _userService.ToggleUserStatusAsync(id);
        if (!success) return NotFound(ApiResponse.Fail("User not found"));
        return Ok(ApiResponse.Ok("User status toggled"));
    }
}
