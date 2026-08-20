using Microsoft.EntityFrameworkCore;
using TicketService.Data;
using TicketService.Models;

namespace TicketService.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, bool includeDetails = false);
    Task<List<Ticket>> GetAllAsync(Guid? customerId = null, string? role = null, Guid? assignedTo = null);
    Task AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
}

public interface ICommentRepository
{
    Task<List<Comment>> GetByTicketIdAsync(Guid ticketId);
    Task AddAsync(Comment comment);
}

public interface IClaimRepository
{
    Task<ClaimDetails?> GetByTicketIdAsync(Guid ticketId);
    Task AddAsync(ClaimDetails claim);
    Task UpdateAsync(ClaimDetails claim);
}

public class TicketRepository : ITicketRepository
{
    private readonly TicketDbContext _ctx;
    public TicketRepository(TicketDbContext ctx) => _ctx = ctx;

    public async Task<Ticket?> GetByIdAsync(Guid id, bool includeDetails = false)
    {
        var q = _ctx.Tickets.AsQueryable();
        if (includeDetails)
            q = q.Include(t => t.Comments).Include(t => t.ClaimDetails);
        return await q.FirstOrDefaultAsync(t => t.TicketId == id);
    }

    public async Task<List<Ticket>> GetAllAsync(Guid? customerId = null, string? role = null, Guid? assignedTo = null)
    {
        var q = _ctx.Tickets.Include(t => t.ClaimDetails).AsQueryable();
        if (customerId.HasValue) q = q.Where(t => t.CustomerId == customerId);
        if (role == "ClaimsSpecialist") q = q.Where(t => t.Type == TicketType.Claim);
        if (role == "SupportSpecialist") q = q.Where(t => t.Type == TicketType.Support);
        if (assignedTo.HasValue) q = q.Where(t => t.AssignedTo == assignedTo);
        return await q.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(Ticket ticket)
    {
        await _ctx.Tickets.AddAsync(ticket);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _ctx.Tickets.Update(ticket);
        await _ctx.SaveChangesAsync();
    }
}

public class CommentRepository : ICommentRepository
{
    private readonly TicketDbContext _ctx;
    public CommentRepository(TicketDbContext ctx) => _ctx = ctx;

    public async Task<List<Comment>> GetByTicketIdAsync(Guid ticketId) =>
        await _ctx.Comments.Where(c => c.TicketId == ticketId).OrderBy(c => c.CreatedAt).ToListAsync();

    public async Task AddAsync(Comment comment)
    {
        await _ctx.Comments.AddAsync(comment);
        await _ctx.SaveChangesAsync();
    }
}

public class ClaimRepository : IClaimRepository
{
    private readonly TicketDbContext _ctx;
    public ClaimRepository(TicketDbContext ctx) => _ctx = ctx;

    public async Task<ClaimDetails?> GetByTicketIdAsync(Guid ticketId) =>
        await _ctx.ClaimDetails.FirstOrDefaultAsync(c => c.TicketId == ticketId);

    public async Task AddAsync(ClaimDetails claim)
    {
        await _ctx.ClaimDetails.AddAsync(claim);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClaimDetails claim)
    {
        _ctx.ClaimDetails.Update(claim);
        await _ctx.SaveChangesAsync();
    }
}
