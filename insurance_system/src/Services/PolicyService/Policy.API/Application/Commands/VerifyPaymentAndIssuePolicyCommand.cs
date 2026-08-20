using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Policy.API.Infrastructure.Data;
using Policy.API.Infrastructure.Services;
using Policy.API.Application.DTOs;
using Policy.API.Domain.Entities;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Policy.API.Application.Commands
{
    public record VerifyPaymentAndIssuePolicyCommand(Guid UserId, VerifyRazorpayPaymentDto Dto) : IRequest<ApiResponse<UserPolicyDto>>;

    public class VerifyPaymentAndIssuePolicyCommandHandler : IRequestHandler<VerifyPaymentAndIssuePolicyCommand, ApiResponse<UserPolicyDto>>
    {
        private readonly PolicyDbContext _db;
        private readonly IRazorpayPaymentService _razorpayService;

        public VerifyPaymentAndIssuePolicyCommandHandler(PolicyDbContext db, IRazorpayPaymentService razorpayService)
        {
            _db = db;
            _razorpayService = razorpayService;
        }

        public async Task<ApiResponse<UserPolicyDto>> Handle(VerifyPaymentAndIssuePolicyCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var isValid = _razorpayService.VerifyPaymentSignature(dto.RazorpayOrderId, dto.RazorpayPaymentId, dto.RazorpaySignature);
            if (!isValid)
            {
                return ApiResponse<UserPolicyDto>.Fail("Razorpay payment signature verification failed.");
            }

            var paymentRecord = await _db.PaymentRecords.FirstOrDefaultAsync(p => p.RazorpayOrderId == dto.RazorpayOrderId, cancellationToken);
            if (paymentRecord != null)
            {
                paymentRecord.RazorpayPaymentId = dto.RazorpayPaymentId;
                paymentRecord.RazorpaySignature = dto.RazorpaySignature;
                paymentRecord.Status = PaymentStatus.Success;
            }

            var catalog = await _db.PolicyCatalogs.FindAsync(new object[] { dto.PolicyCatalogId }, cancellationToken);
            if (catalog == null)
            {
                return ApiResponse<UserPolicyDto>.Fail("Catalog policy item not found.");
            }

            // Unique policy number generation: POL-YYYYMMDD-XXXX
            var policyNumber = $"POL-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddYears(1);

            var userPolicy = new UserPolicy
            {
                PolicyNumber = policyNumber,
                UserId = request.UserId,
                PolicyCatalogId = catalog.Id,
                PolicyName = catalog.Name,
                Type = catalog.Type,
                FinalPremium = paymentRecord?.Amount ?? dto.Amount,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                RazorpayOrderId = dto.RazorpayOrderId,
                RazorpayPaymentId = dto.RazorpayPaymentId,
                IssuedAt = DateTime.UtcNow
            };

            _db.UserPolicies.Add(userPolicy);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new UserPolicyDto
            {
                Id = userPolicy.Id,
                PolicyNumber = userPolicy.PolicyNumber,
                UserId = userPolicy.UserId,
                PolicyCatalogId = userPolicy.PolicyCatalogId,
                PolicyName = userPolicy.PolicyName,
                Type = userPolicy.Type.ToString(),
                FinalPremium = userPolicy.FinalPremium,
                StartDate = userPolicy.StartDate,
                EndDate = userPolicy.EndDate,
                IsActive = userPolicy.IsActive,
                RazorpayOrderId = userPolicy.RazorpayOrderId,
                RazorpayPaymentId = userPolicy.RazorpayPaymentId,
                IssuedAt = userPolicy.IssuedAt
            };

            return ApiResponse<UserPolicyDto>.Ok(result, $"Payment verified successfully. Policy issued with Number: {userPolicy.PolicyNumber}");
        }
    }
}
