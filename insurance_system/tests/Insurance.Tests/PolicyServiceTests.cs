using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Policy.API.Application.Commands;
using Policy.API.Application.DTOs;
using Policy.API.Domain.Entities;
using Policy.API.Infrastructure.Data;
using Policy.API.Infrastructure.Services;
using Insurance.Shared.Enums;
using Moq;

namespace Insurance.Tests
{
    public class PolicyServiceTests
    {
        private PolicyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<PolicyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new PolicyDbContext(options);
        }

        [Fact]
        public async Task CalculatePremiumCommand_ShouldApplyAgeMultiplier()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var catalog = new PolicyCatalog
            {
                Id = Guid.NewGuid(),
                Name = "Health Shield",
                Type = PolicyType.Health,
                BasePremiumAmount = 10000,
                IsActive = true
            };
            db.PolicyCatalogs.Add(catalog);
            await db.SaveChangesAsync();

            var handler = new CalculatePremiumCommandHandler(db);
            var req = new CalculatePremiumRequestDto
            {
                PolicyCatalogId = catalog.Id,
                Age = 50, // Multiplier 1.35
                DurationYears = 1,
                IncludeAddonCoverage = true // +1500
            };

            // Act
            var res = await handler.Handle(new CalculatePremiumCommand(req), CancellationToken.None);

            // Assert
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            // 10000 * 1.35 + 1500 = 15000 INR
            Assert.Equal(15000m, res.Data.CalculatedFinalPremium);
        }

        [Fact]
        public async Task VerifyPaymentAndIssuePolicy_ShouldIssuePolicyWithNumber()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var catalog = new PolicyCatalog
            {
                Id = Guid.NewGuid(),
                Name = "Auto Secure",
                Type = PolicyType.Vehicle,
                BasePremiumAmount = 5000,
                IsActive = true
            };
            db.PolicyCatalogs.Add(catalog);
            await db.SaveChangesAsync();

            var mockRazorpay = new Mock<IRazorpayPaymentService>();
            mockRazorpay.Setup(r => r.VerifyPaymentSignature(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(true);

            var handler = new VerifyPaymentAndIssuePolicyCommandHandler(db, mockRazorpay.Object);
            var userId = Guid.NewGuid();

            var dto = new VerifyRazorpayPaymentDto
            {
                RazorpayOrderId = "order_mock_123",
                RazorpayPaymentId = "pay_mock_456",
                RazorpaySignature = "sig_mock_789",
                PolicyCatalogId = catalog.Id,
                Amount = 5000
            };

            // Act
            var res = await handler.Handle(new VerifyPaymentAndIssuePolicyCommand(userId, dto), CancellationToken.None);

            // Assert
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            Assert.StartsWith("POL-", res.Data.PolicyNumber);
            Assert.True(res.Data.IsActive);
        }
    }
}
