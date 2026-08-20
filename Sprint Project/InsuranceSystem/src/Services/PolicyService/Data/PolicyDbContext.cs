using Microsoft.EntityFrameworkCore;
using PolicyService.Models;

namespace PolicyService.Data;

public class PolicyDbContext : DbContext
{
    public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options) { }

    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<CustomerPolicy> CustomerPolicies => Set<CustomerPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Policy>(e =>
        {
            e.HasKey(p => p.PolicyId);
            e.Property(p => p.Type).HasConversion<string>();
            e.Property(p => p.Premium).HasPrecision(18, 2);
            e.HasMany(p => p.CustomerPolicies).WithOne(cp => cp.Policy).HasForeignKey(cp => cp.PolicyId);
        });

        modelBuilder.Entity<CustomerPolicy>(e =>
        {
            e.HasKey(cp => cp.CustomerPolicyId);
            e.Property(cp => cp.Status).HasConversion<string>();
        });
    }
}
