using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Identity.API.Application.Commands;
using Identity.API.Application.DTOs;
using Identity.API.Infrastructure.Data;
using Identity.API.Infrastructure.Services;
using Insurance.Shared.Enums;
using Insurance.Shared.Security;
using Moq;

namespace Insurance.Tests
{
    public class IdentityServiceTests
    {
        private IdentityDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new IdentityDbContext(options);
        }

        private IOptions<JwtSettings> GetJwtSettings()
        {
            return Options.Create(new JwtSettings
            {
                SecretKey = "SuperSecretInsuranceSystemJwtKey_MustBeAtLeast32BytesLong!",
                Issuer = "InsuranceSystemGateway",
                Audience = "InsuranceSystemClients",
                ExpirationMinutes = 1440
            });
        }

        [Fact]
        public async Task RegisterUserCommand_ShouldCreateUser_WhenEmailIsUnique()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var handler = new RegisterUserCommandHandler(db, GetJwtSettings());
            var dto = new RegisterUserDto
            {
                Email = "testuser@insurance.com",
                Password = "Password123!",
                FullName = "Test User",
                PhoneNumber = "+123456789",
                Role = UserRole.Customer
            };

            // Act
            var response = await handler.Handle(new RegisterUserCommand(dto), CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal("testuser@insurance.com", response.Data.Email);
            Assert.NotEmpty(response.Data.Token);
        }

        [Fact]
        public async Task LoginCommand_ShouldAuthenticate_WhenCredentialsAreValid()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var registerHandler = new RegisterUserCommandHandler(db, GetJwtSettings());
            await registerHandler.Handle(new RegisterUserCommand(new RegisterUserDto
            {
                Email = "loginuser@insurance.com",
                Password = "Password123!",
                FullName = "Login User",
                Role = UserRole.Customer
            }), CancellationToken.None);

            var loginHandler = new LoginCommandHandler(db, GetJwtSettings());

            // Act
            var loginResponse = await loginHandler.Handle(new LoginCommand(new LoginDto
            {
                Email = "loginuser@insurance.com",
                Password = "Password123!"
            }), CancellationToken.None);

            // Assert
            Assert.True(loginResponse.Success);
            Assert.NotNull(loginResponse.Data?.Token);
        }

        [Fact]
        public async Task SendOtpCommand_ShouldGenerateAndStoreOtp()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var mockEmail = new Mock<IEmailService>();
            var handler = new SendOtpCommandHandler(db, mockEmail.Object);

            // Act
            var response = await handler.Handle(new SendOtpCommand("otp@insurance.com"), CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(6, response.Data.Length);
            mockEmail.Verify(e => e.SendOtpEmailAsync("otp@insurance.com", response.Data), Times.Once);
        }
    }
}
