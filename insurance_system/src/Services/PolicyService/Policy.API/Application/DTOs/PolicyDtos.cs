using System;
using Insurance.Shared.Enums;

namespace Policy.API.Application.DTOs
{
    public class PolicyCatalogDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PolicyType Type { get; set; }
        public decimal BasePremiumAmount { get; set; }
        public string CoverageDetails { get; set; } = string.Empty;
        public string TermsAndConditions { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreatePolicyCatalogDto
    {
        public string Name { get; set; } = string.Empty;
        public PolicyType Type { get; set; }
        public decimal BasePremiumAmount { get; set; }
        public string CoverageDetails { get; set; } = string.Empty;
        public string TermsAndConditions { get; set; } = string.Empty;
    }

    public class CalculatePremiumRequestDto
    {
        public Guid PolicyCatalogId { get; set; }
        public int Age { get; set; } = 30;
        public int DurationYears { get; set; } = 1;
        public bool IncludeAddonCoverage { get; set; } = false;
    }

    public class PremiumCalculationResultDto
    {
        public Guid PolicyCatalogId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public decimal BasePremium { get; set; }
        public decimal AgeRiskFactorMultiplier { get; set; }
        public decimal AddonCoverageCost { get; set; }
        public decimal CalculatedFinalPremium { get; set; }
    }

    public class InitiatePurchaseDto
    {
        public Guid PolicyCatalogId { get; set; }
        public int Age { get; set; } = 30;
        public int DurationYears { get; set; } = 1;
        public bool IncludeAddonCoverage { get; set; } = false;
    }

    public class RazorpayOrderResponseDto
    {
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public Guid PolicyCatalogId { get; set; }
    }

    public class VerifyRazorpayPaymentDto
    {
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
        public Guid PolicyCatalogId { get; set; }
        public decimal Amount { get; set; }
    }

    public class UserPolicyDto
    {
        public Guid Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid PolicyCatalogId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal FinalPremium { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
    }
}
