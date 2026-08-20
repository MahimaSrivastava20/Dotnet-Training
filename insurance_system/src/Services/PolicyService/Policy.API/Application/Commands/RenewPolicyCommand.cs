using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Policy.API.Infrastructure.Data;
using Policy.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Policy.API.Application.Commands
{
    public record RenewPolicyCommand(Guid PolicyId, Guid UserId) : IRequest<ApiResponse<UserPolicyDto>>;

    public class RenewPolicyCommandHandler : IRequestHandler<RenewPolicyCommand, ApiResponse<UserPolicyDto>>
    {
        private readonly PolicyDbContext _db;

        public RenewPolicyCommandHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<UserPolicyDto>> Handle(RenewPolicyCommand request, CancellationToken cancellationToken)
        {
            var policy = await _db.UserPolicies.FindAsync(new object[] { request.PolicyId }, cancellationToken);

            if (policy == null || policy.UserId != request.UserId)
            {
                return ApiResponse<UserPolicyDto>.Fail("User policy not found.");
            }

            policy.EndDate = policy.EndDate.AddYears(1);
            policy.IsActive = true;

            await _db.SaveChangesAsync(cancellationToken);

            var result = new UserPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                UserId = policy.UserId,
                PolicyCatalogId = policy.PolicyCatalogId,
                PolicyName = policy.PolicyName,
                Type = policy.Type.ToString(),
                FinalPremium = policy.FinalPremium,
                StartDate = policy.StartDate,
                EndDate = policy.EndDate,
                IsActive = policy.IsActive,
                RazorpayOrderId = policy.RazorpayOrderId,
                RazorpayPaymentId = policy.RazorpayPaymentId,
                IssuedAt = policy.IssuedAt
            };

            return ApiResponse<UserPolicyDto>.Ok(result, $"Policy {policy.PolicyNumber} successfully renewed until {policy.EndDate:yyyy-MM-dd}.");
        }
    }
}
