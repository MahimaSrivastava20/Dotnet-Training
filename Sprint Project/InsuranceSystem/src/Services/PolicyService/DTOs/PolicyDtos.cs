using System.ComponentModel.DataAnnotations;

namespace PolicyService.DTOs;

public class CreatePolicyDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)] public decimal Premium { get; set; }
    [Range(0.01, double.MaxValue)] public decimal CoverageAmount { get; set; }
    [Required] public string CoverageDetails { get; set; } = string.Empty;
    [Required] public string Terms { get; set; } = string.Empty;
}

public class PurchasePolicyDto
{
    [Required] public Guid PolicyId { get; set; }
}

public class ActivatePolicyDto
{
    [Required] public Guid CustomerId { get; set; }
    [Required] public Guid PolicyId { get; set; }
}

public class DeductClaimDto
{
    [Required] public decimal Amount { get; set; }
}

public class InternalDeductDto
{
    [Required] public Guid CustomerPolicyId { get; set; }
    [Required] public decimal Amount { get; set; }
}

public class PolicyResponseDto
{
    public Guid PolicyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public decimal CoverageAmount { get; set; }
    public string CoverageDetails { get; set; } = string.Empty;
    public string Terms { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerPolicyResponseDto
{
    public Guid CustomerPolicyId { get; set; }
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string PolicyType { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public decimal CoverageAmount { get; set; }
    public decimal RemainingCoverageAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
