using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Policy.API.Infrastructure.Data;
using Policy.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Policy.API.Application.Queries
{
    public record GetMyPoliciesQuery(Guid UserId) : IRequest<ApiResponse<List<UserPolicyDto>>>;

    public class GetMyPoliciesQueryHandler : IRequestHandler<GetMyPoliciesQuery, ApiResponse<List<UserPolicyDto>>>
    {
        private readonly PolicyDbContext _db;

        public GetMyPoliciesQueryHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<UserPolicyDto>>> Handle(GetMyPoliciesQuery request, CancellationToken cancellationToken)
        {
            var list = await _db.UserPolicies
                .Where(up => up.UserId == request.UserId)
                .Select(up => new UserPolicyDto
                {
                    Id = up.Id,
                    PolicyNumber = up.PolicyNumber,
                    UserId = up.UserId,
                    PolicyCatalogId = up.PolicyCatalogId,
                    PolicyName = up.PolicyName,
                    Type = up.Type.ToString(),
                    FinalPremium = up.FinalPremium,
                    StartDate = up.StartDate,
                    EndDate = up.EndDate,
                    IsActive = up.IsActive,
                    RazorpayOrderId = up.RazorpayOrderId,
                    RazorpayPaymentId = up.RazorpayPaymentId,
                    IssuedAt = up.IssuedAt
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<List<UserPolicyDto>>.Ok(list);
        }
    }
}
