using IdentityService.DTOs;
using IdentityService.Models;

namespace IdentityService.Services;

public interface IUserService
{
    Task<UserResponseDto?> CreateSpecialistAsync(CreateSpecialistDto dto, UserRole role);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<bool> ToggleUserStatusAsync(Guid userId);
}
