using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Identity.API.Infrastructure.Data;
using Insurance.Shared.Models;

namespace Identity.API.Application.Commands
{
    public record VerifyOtpCommand(string Email, string OtpCode) : IRequest<ApiResponse<bool>>;

    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<bool>>
    {
        private readonly IdentityDbContext _db;

        public VerifyOtpCommandHandler(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var otpRecord = await _db.OtpRecords.FirstOrDefaultAsync(
                o => o.Email.ToLower() == request.Email.ToLower() &&
                     o.OtpCode == request.OtpCode &&
                     !o.IsUsed &&
                     o.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

            if (otpRecord == null)
            {
                return ApiResponse<bool>.Fail("Invalid or expired OTP code.");
            }

            otpRecord.IsUsed = true;
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "OTP verified successfully.");
        }
    }
}
