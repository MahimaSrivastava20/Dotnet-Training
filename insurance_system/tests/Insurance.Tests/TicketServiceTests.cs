using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Ticket.API.Application.Commands;
using Ticket.API.Application.DTOs;
using Ticket.API.Infrastructure.Data;
using Insurance.Shared.Enums;

namespace Insurance.Tests
{
    public class TicketServiceTests
    {
        private TicketDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new TicketDbContext(options);
        }

        [Fact]
        public async Task CreateTicketCommand_ShouldInitializeTicketAndAuditLog()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var handler = new CreateTicketCommandHandler(db);
            var customerId = Guid.NewGuid();

            var dto = new CreateTicketDto
            {
                Subject = "Claim Authorization Inquiry",
                Description = "Please update status of claim reimbursement.",
                PolicyNumber = "POL-20260819-1234"
            };

            // Act
            var res = await handler.Handle(new CreateTicketCommand(customerId, "Jane Customer", dto), CancellationToken.None);

            // Assert
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            Assert.StartsWith("TCK-", res.Data.TicketNumber);
            Assert.Equal("Created", res.Data.Status);
        }

        [Fact]
        public async Task AssignTicketCommand_ShouldUpdateStatusToAssigned()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var createHandler = new CreateTicketCommandHandler(db);
            var created = await createHandler.Handle(new CreateTicketCommand(Guid.NewGuid(), "Cust", new CreateTicketDto
            {
                Subject = "Help needed",
                Description = "Ticket description"
            }), CancellationToken.None);

            var assignHandler = new AssignTicketCommandHandler(db);
            var adjusterId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            // Act
            var res = await assignHandler.Handle(new AssignTicketCommand(created.Data!.Id, adjusterId, "Adjuster Bob", adminId, "Admin Alice"), CancellationToken.None);

            // Assert
            Assert.True(res.Success);
            var ticket = await db.SupportTickets.FindAsync(created.Data.Id);
            Assert.NotNull(ticket);
            Assert.Equal(TicketStatus.Assigned, ticket.Status);
            Assert.Equal("Adjuster Bob", ticket.AssignedAdjusterName);
        }
    }
}
