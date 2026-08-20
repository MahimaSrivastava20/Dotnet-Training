using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure.Data;
using Identity.API.Infrastructure.Services;
using Insurance.Shared.Models;

namespace Identity.API.Application.Commands
{
    public record SendOtpCommand(string Email) : IRequest<ApiResponse<string>>;

    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApiResponse<string>>
    {
        private readonly IdentityDbContext _db;
        private readonly IEmailService _emailService;

        public SendOtpCommandHandler(IdentityDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<ApiResponse<string>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otpRecord = new OtpRecord
            {
                Email = request.Email.ToLower(),
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.OtpRecords.Add(otpRecord);
            await _db.SaveChangesAsync(cancellationToken);

            await _emailService.SendOtpEmailAsync(request.Email, otpCode);

            return ApiResponse<string>.Ok(otpCode, $"OTP generated and dispatched via MailKit to {request.Email}.");
        }
    }
}
