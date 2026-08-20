using Microsoft.EntityFrameworkCore;
using Identity.API.Domain.Entities;
using Insurance.Shared.Enums;
using System;

namespace Identity.API.Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<OtpRecord> OtpRecords => Set<OtpRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<OtpRecord>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Email).IsRequired();
                entity.Property(o => o.OtpCode).IsRequired().HasMaxLength(10);
            });

            // Seed initial Admin and Claims Adjuster accounts
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adjusterId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var customerId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Email = "admin@insurance.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    FullName = "System Administrator",
                    PhoneNumber = "+1234567890",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = adjusterId,
                    Email = "adjuster@insurance.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Adjuster@123"),
                    FullName = "Senior Claims Adjuster",
                    PhoneNumber = "+1987654321",
                    Role = UserRole.ClaimsAdjuster,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = customerId,
                    Email = "customer@insurance.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    FullName = "John Doe (Customer)",
                    PhoneNumber = "+1122334455",
                    Role = UserRole.Customer,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
