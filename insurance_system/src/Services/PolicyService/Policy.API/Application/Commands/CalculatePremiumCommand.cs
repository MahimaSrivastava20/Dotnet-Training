using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Policy.API.Infrastructure.Data;
using Policy.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Policy.API.Application.Commands
{
    public record CalculatePremiumCommand(CalculatePremiumRequestDto Dto) : IRequest<ApiResponse<PremiumCalculationResultDto>>;

    public class CalculatePremiumCommandHandler : IRequestHandler<CalculatePremiumCommand, ApiResponse<PremiumCalculationResultDto>>
    {
        private readonly PolicyDbContext _db;

        public CalculatePremiumCommandHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<PremiumCalculationResultDto>> Handle(CalculatePremiumCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _db.PolicyCatalogs.FindAsync(new object[] { request.Dto.PolicyCatalogId }, cancellationToken);

            if (catalog == null || !catalog.IsActive)
            {
                return ApiResponse<PremiumCalculationResultDto>.Fail("Selected insurance policy catalog item is not active or available.");
            }

            // Dynamic calculation logic based on Age & Duration
            decimal ageMultiplier = 1.0m;
            if (request.Dto.Age > 45) ageMultiplier = 1.35m;
            else if (request.Dto.Age > 35) ageMultiplier = 1.15m;
            else if (request.Dto.Age < 25) ageMultiplier = 0.95m;

            decimal addonCost = request.Dto.IncludeAddonCoverage ? 1500m : 0m;
            decimal baseForDuration = catalog.BasePremiumAmount * request.Dto.DurationYears;
            decimal calculatedFinal = (baseForDuration * ageMultiplier) + addonCost;

            var result = new PremiumCalculationResultDto
            {
                PolicyCatalogId = catalog.Id,
                PolicyName = catalog.Name,
                BasePremium = catalog.BasePremiumAmount,
                AgeRiskFactorMultiplier = ageMultiplier,
                AddonCoverageCost = addonCost,
                CalculatedFinalPremium = Math.Round(calculatedFinal, 2)
            };

            return ApiResponse<PremiumCalculationResultDto>.Ok(result, "Dynamic premium calculated successfully.");
        }
    }
}
