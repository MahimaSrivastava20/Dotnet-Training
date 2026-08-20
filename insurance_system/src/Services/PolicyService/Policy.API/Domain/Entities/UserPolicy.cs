using System;
using Insurance.Shared.Enums;

namespace Policy.API.Domain.Entities
{
    public class UserPolicy
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PolicyNumber { get; set; } = string.Empty; // e.g. POL-20260819-A1B2
        public Guid UserId { get; set; }
        public Guid PolicyCatalogId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public PolicyType Type { get; set; }
        public decimal FinalPremium { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public PolicyCatalog? CatalogItem { get; set; }
    }
}
