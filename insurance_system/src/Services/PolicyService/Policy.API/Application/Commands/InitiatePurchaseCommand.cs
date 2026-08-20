using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Policy.API.Infrastructure.Data;
using Policy.API.Infrastructure.Services;
using Policy.API.Application.DTOs;
using Policy.API.Domain.Entities;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Policy.API.Application.Commands
{
    public record InitiatePurchaseCommand(Guid UserId, InitiatePurchaseDto Dto) : IRequest<ApiResponse<RazorpayOrderResponseDto>>;

    public class InitiatePurchaseCommandHandler : IRequestHandler<InitiatePurchaseCommand, ApiResponse<RazorpayOrderResponseDto>>
    {
        private readonly PolicyDbContext _db;
        private readonly IRazorpayPaymentService _razorpayService;

        public InitiatePurchaseCommandHandler(PolicyDbContext db, IRazorpayPaymentService razorpayService)
        {
            _db = db;
            _razorpayService = razorpayService;
        }

        public async Task<ApiResponse<RazorpayOrderResponseDto>> Handle(InitiatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _db.PolicyCatalogs.FindAsync(new object[] { request.Dto.PolicyCatalogId }, cancellationToken);
            if (catalog == null || !catalog.IsActive)
            {
                return ApiResponse<RazorpayOrderResponseDto>.Fail("Policy catalog item unavailable.");
            }

            // Calculate premium amount
            decimal ageMultiplier = request.Dto.Age > 45 ? 1.35m : (request.Dto.Age > 35 ? 1.15m : 1.0m);
            decimal addonCost = request.Dto.IncludeAddonCoverage ? 1500m : 0m;
            decimal finalPremium = Math.Round((catalog.BasePremiumAmount * request.Dto.DurationYears * ageMultiplier) + addonCost, 2);

            var receiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(100, 999)}";

            // Create Razorpay Order via Payment Gateway Integration
            var (orderId, receipt, amount) = _razorpayService.CreateRazorpayOrder(finalPremium, receiptNumber);

            var paymentRecord = new PaymentRecord
            {
                UserId = request.UserId,
                PolicyCatalogId = catalog.Id,
                RazorpayOrderId = orderId,
                Amount = finalPremium,
                Currency = "INR",
                Status = PaymentStatus.Pending,
                ReceiptNumber = receiptNumber,
                CreatedAt = DateTime.UtcNow
            };

            _db.PaymentRecords.Add(paymentRecord);
            await _db.SaveChangesAsync(cancellationToken);

            var response = new RazorpayOrderResponseDto
            {
                RazorpayOrderId = orderId,
                ReceiptNumber = receiptNumber,
                Amount = finalPremium,
                Currency = "INR",
                PolicyCatalogId = catalog.Id
            };

            return ApiResponse<RazorpayOrderResponseDto>.Ok(response, "Razorpay payment order initiated.");
        }
    }
}
