using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _ctx;
    public UserRepository(IdentityDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _ctx.Users.FindAsync(id);

    public async Task<List<User>> GetAllAsync() =>
        await _ctx.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

    public async Task AddAsync(User user)
    {
        await _ctx.Users.AddAsync(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _ctx.Users.AnyAsync(u => u.Email == email);
}
