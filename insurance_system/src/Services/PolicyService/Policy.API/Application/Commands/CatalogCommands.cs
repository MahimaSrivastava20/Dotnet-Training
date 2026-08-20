using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Policy.API.Domain.Entities;
using Policy.API.Infrastructure.Data;
using Policy.API.Application.DTOs;
using Insurance.Shared.Models;

namespace Policy.API.Application.Commands
{
    public record CreatePolicyCatalogCommand(CreatePolicyCatalogDto Dto) : IRequest<ApiResponse<PolicyCatalogDto>>;

    public class CreatePolicyCatalogCommandHandler : IRequestHandler<CreatePolicyCatalogCommand, ApiResponse<PolicyCatalogDto>>
    {
        private readonly PolicyDbContext _db;

        public CreatePolicyCatalogCommandHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<PolicyCatalogDto>> Handle(CreatePolicyCatalogCommand request, CancellationToken cancellationToken)
        {
            var catalog = new PolicyCatalog
            {
                Name = request.Dto.Name,
                Type = request.Dto.Type,
                BasePremiumAmount = request.Dto.BasePremiumAmount,
                CoverageDetails = request.Dto.CoverageDetails,
                TermsAndConditions = request.Dto.TermsAndConditions,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.PolicyCatalogs.Add(catalog);
            await _db.SaveChangesAsync(cancellationToken);

            var result = new PolicyCatalogDto
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Type = catalog.Type,
                BasePremiumAmount = catalog.BasePremiumAmount,
                CoverageDetails = catalog.CoverageDetails,
                TermsAndConditions = catalog.TermsAndConditions,
                IsActive = catalog.IsActive
            };

            return ApiResponse<PolicyCatalogDto>.Ok(result, "Policy catalog item created.");
        }
    }

    public record DeletePolicyCatalogCommand(Guid Id) : IRequest<ApiResponse<bool>>;

    public class DeletePolicyCatalogCommandHandler : IRequestHandler<DeletePolicyCatalogCommand, ApiResponse<bool>>
    {
        private readonly PolicyDbContext _db;

        public DeletePolicyCatalogCommandHandler(PolicyDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<bool>> Handle(DeletePolicyCatalogCommand request, CancellationToken cancellationToken)
        {
            var item = await _db.PolicyCatalogs.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item == null)
            {
                return ApiResponse<bool>.Fail("Policy catalog item not found.");
            }

            item.IsActive = false; // Soft delete
            await _db.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Policy catalog item deactivated.");
        }
    }
}
