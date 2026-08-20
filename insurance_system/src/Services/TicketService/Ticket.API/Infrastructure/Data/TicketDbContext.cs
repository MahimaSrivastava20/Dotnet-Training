using Microsoft.EntityFrameworkCore;
using Ticket.API.Domain.Entities;
using Insurance.Shared.Enums;
using System;

namespace Ticket.API.Infrastructure.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public DbSet<TicketAuditLog> TicketAuditLogs => Set<TicketAuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SupportTicket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasIndex(t => t.TicketNumber).IsUnique();
                entity.Property(t => t.Subject).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<TicketAuditLog>(entity =>
            {
                entity.HasKey(a => a.Id);
            });

            // Seed sample tickets
            var customerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var adjusterId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var ticket1Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<SupportTicket>().HasData(
                new SupportTicket
                {
                    Id = ticket1Id,
                    TicketNumber = "TCK-20260819-1001",
                    CustomerId = customerId,
                    CustomerName = "John Doe (Customer)",
                    PolicyNumber = "POL-20260819-7788",
                    Subject = "Cashless Claim Approval Request for Hospitalization",
                    Description = "Requesting expedited claim pre-authorization for upcoming medical procedure under Comprehensive Health Shield Plus.",
                    Status = TicketStatus.Assigned,
                    AssignedAdjusterId = adjusterId,
                    AssignedAdjusterName = "Senior Claims Adjuster",
                    CreatedAt = new DateTime(2026, 8, 18, 10, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 8, 18, 11, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<TicketComment>().HasData(
                new TicketComment
                {
                    Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                    TicketId = ticket1Id,
                    AuthorUserId = customerId,
                    AuthorName = "John Doe (Customer)",
                    AuthorRole = "Customer",
                    CommentText = "I have uploaded hospital discharge estimates. Please confirm receipt.",
                    IsInternal = false,
                    CreatedAt = new DateTime(2026, 8, 18, 10, 05, 0, DateTimeKind.Utc)
                },
                new TicketComment
                {
                    Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                    TicketId = ticket1Id,
                    AuthorUserId = adjusterId,
                    AuthorName = "Senior Claims Adjuster",
                    AuthorRole = "ClaimsAdjuster",
                    CommentText = "Documents received and currently under medical panel audit.",
                    IsInternal = false,
                    CreatedAt = new DateTime(2026, 8, 18, 11, 00, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
