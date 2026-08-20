using AdminService.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Data;

public class AdminReadDbContext : DbContext
{
    public AdminReadDbContext(DbContextOptions<AdminReadDbContext> options) : base(options) { }

    // Read-only views - mirror data from other services
    public DbSet<UserView> UserViews => Set<UserView>();
    public DbSet<TicketView> TicketViews => Set<TicketView>();
    public DbSet<ClaimView> ClaimViews => Set<ClaimView>();
    public DbSet<PolicyView> PolicyViews => Set<PolicyView>();
    public DbSet<PaymentView> PaymentViews => Set<PaymentView>();
    public DbSet<NotificationView> NotificationViews => Set<NotificationView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserView>(e => { e.HasKey(u => u.UserId); e.ToTable("UserViews"); });
        modelBuilder.Entity<TicketView>(e => { e.HasKey(t => t.TicketId); e.ToTable("TicketViews"); e.Property(t => t.Type).HasConversion<string>(); e.Property(t => t.Status).HasConversion<string>(); });
        modelBuilder.Entity<ClaimView>(e => { e.HasKey(c => c.ClaimId); e.ToTable("ClaimViews"); e.Property(c => c.ClaimAmount).HasPrecision(18, 2); });
        modelBuilder.Entity<PolicyView>(e => { e.HasKey(p => p.PolicyId); e.ToTable("PolicyViews"); e.Property(p => p.Premium).HasPrecision(18, 2); });
        modelBuilder.Entity<PaymentView>(e => { e.HasKey(p => p.PaymentId); e.ToTable("PaymentViews"); e.Property(p => p.Amount).HasPrecision(18, 2); });
        modelBuilder.Entity<NotificationView>(e => { e.HasKey(n => n.NotificationId); e.ToTable("NotificationViews"); });
    }
}
