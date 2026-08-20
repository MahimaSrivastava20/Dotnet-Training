using Microsoft.EntityFrameworkCore;
using TicketService.Models;

namespace TicketService.Data;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ClaimDetails> ClaimDetails => Set<ClaimDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(e =>
        {
            e.HasKey(t => t.TicketId);
            e.Property(t => t.Type).HasConversion<string>();
            e.Property(t => t.Status).HasConversion<string>();
            e.HasMany(t => t.Comments).WithOne(c => c.Ticket).HasForeignKey(c => c.TicketId);
            e.HasOne(t => t.ClaimDetails).WithOne(cd => cd.Ticket).HasForeignKey<ClaimDetails>(cd => cd.TicketId);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.HasKey(c => c.CommentId);
        });

        modelBuilder.Entity<ClaimDetails>(e =>
        {
            e.HasKey(cd => cd.ClaimId);
            e.Property(cd => cd.ApprovalStatus).HasConversion<string>();
            e.Property(cd => cd.ClaimAmount).HasPrecision(18, 2);
        });
    }
}
