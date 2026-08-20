using Microsoft.EntityFrameworkCore;
using Policy.API.Domain.Entities;
using Insurance.Shared.Enums;
using System;

namespace Policy.API.Infrastructure.Data
{
    public class PolicyDbContext : DbContext
    {
        public PolicyDbContext(DbContextOptions<PolicyDbContext> options) : base(options) { }

        public DbSet<PolicyCatalog> PolicyCatalogs => Set<PolicyCatalog>();
        public DbSet<UserPolicy> UserPolicies => Set<UserPolicy>();
        public DbSet<PaymentRecord> PaymentRecords => Set<PaymentRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PolicyCatalog>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            });

            modelBuilder.Entity<UserPolicy>(entity =>
            {
                entity.HasKey(up => up.Id);
                entity.HasIndex(up => up.PolicyNumber).IsUnique();
            });

            modelBuilder.Entity<PaymentRecord>(entity =>
            {
                entity.HasKey(pr => pr.Id);
            });

            // Seed initial Insurance Policy Catalog
            modelBuilder.Entity<PolicyCatalog>().HasData(
                new PolicyCatalog
                {
                    Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    Name = "Comprehensive Health Shield Plus",
                    Type = PolicyType.Health,
                    BasePremiumAmount = 12000,
                    CoverageDetails = "Full cashless hospitalization up to $50,000, OPD coverage, Free annual checkups.",
                    TermsAndConditions = "Waiting period of 30 days for general illnesses. Pre-existing diseases covered after 2 years.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new PolicyCatalog
                {
                    Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    Name = "Motor Drive Secure Auto Insurance",
                    Type = PolicyType.Vehicle,
                    BasePremiumAmount = 8500,
                    CoverageDetails = "Zero Depreciation, 24/7 Roadside Assistance, Third-Party Liability & Own Damage protection.",
                    TermsAndConditions = "Valid driving license required. Claims must be registered within 48 hours of incident.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new PolicyCatalog
                {
                    Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                    Name = "Term Life Assurance Protect",
                    Type = PolicyType.Life,
                    BasePremiumAmount = 15000,
                    CoverageDetails = "Sum assured of $250,000 paid to designated beneficiaries upon death or critical disability.",
                    TermsAndConditions = "Medical checkup required for age above 45. Suicide exclusion during the first 12 months.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new PolicyCatalog
                {
                    Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                    Name = "Home & Property Guardian",
                    Type = PolicyType.Property,
                    BasePremiumAmount = 9500,
                    CoverageDetails = "Covers structural damage from fire, natural disasters, theft, and loss of contents up to $100,000.",
                    TermsAndConditions = "Property survey required for high value contents. Excludes intentional damages.",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
