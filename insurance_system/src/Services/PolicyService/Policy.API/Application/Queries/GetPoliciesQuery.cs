using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Policy.API.Infrastructure.Data;
using Policy.API.Application.DTOs;
using Insurance.Shared.Enums;
using Insurance.Shared.Models;

namespace Policy.API.Application.Queries
{
    public record GetPoliciesQuery(PolicyType? TypeFilter = null, decimal? MaxPrice = null) : IRequest<ApiResponse<List<PolicyCatalogDto>>>;

    public class GetPoliciesQueryHandler : IRequestHandler<GetPoliciesQuery, ApiResponse<List<PolicyCatalogDto>>>
    {
        private readonly PolicyDbContext _db;

        public GetPoliciesQueryHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<PolicyCatalogDto>>> Handle(GetPoliciesQuery request, CancellationToken cancellationToken)
        {
            var query = _db.PolicyCatalogs.AsQueryable().Where(p => p.IsActive);

            if (request.TypeFilter.HasValue)
            {
                query = query.Where(p => p.Type == request.TypeFilter.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.BasePremiumAmount <= request.MaxPrice.Value);
            }

            var list = await query.Select(p => new PolicyCatalogDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                BasePremiumAmount = p.BasePremiumAmount,
                CoverageDetails = p.CoverageDetails,
                TermsAndConditions = p.TermsAndConditions,
                IsActive = p.IsActive
            }).ToListAsync(cancellationToken);

            return ApiResponse<List<PolicyCatalogDto>>.Ok(list);
        }
    }
}
