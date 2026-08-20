using System;
using Insurance.Shared.Enums;

namespace Policy.API.Domain.Entities
{
    public class PolicyCatalog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public PolicyType Type { get; set; } = PolicyType.Health;
        public decimal BasePremiumAmount { get; set; }
        public string CoverageDetails { get; set; } = string.Empty;
        public string TermsAndConditions { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
