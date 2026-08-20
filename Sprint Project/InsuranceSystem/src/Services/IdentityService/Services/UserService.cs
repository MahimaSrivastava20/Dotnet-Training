using IdentityService.DTOs;
using IdentityService.Models;
using IdentityService.Repositories;

namespace IdentityService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<UserResponseDto?> CreateSpecialistAsync(CreateSpecialistDto dto, UserRole role)
    {
        if (await _repo.EmailExistsAsync(dto.Email)) return null;

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role
        };
        await _repo.AddAsync(user);
        return MapToDto(user);
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<bool> ToggleUserStatusAsync(Guid userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null) return false;
        user.IsActive = !user.IsActive;
        await _repo.UpdateAsync(user);
        return true;
    }

    private static UserResponseDto MapToDto(User u) => new()
    {
        UserId = u.UserId,
        Name = u.Name,
        Email = u.Email,
        Role = u.Role.ToString(),
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
