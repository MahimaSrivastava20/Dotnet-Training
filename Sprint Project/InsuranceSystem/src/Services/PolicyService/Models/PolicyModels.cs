namespace PolicyService.Models;

public enum PolicyType { Life, Health, Vehicle, Property, TermLife, Investment, Travel, ChildSavings, Retirement, TwoWheeler, FamilyHealth, TermWomen, ReturnOfPremium, GuaranteedReturn, EmployeeGroup, HomeInsurance }
public enum CustomerPolicyStatus { Active, Expired, Cancelled, PendingPayment }

public class Policy
{
    public Guid PolicyId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public PolicyType Type { get; set; }
    public decimal Premium { get; set; }
    public decimal CoverageAmount { get; set; }
    public string CoverageDetails { get; set; } = string.Empty;
    public string Terms { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CustomerPolicy> CustomerPolicies { get; set; } = new List<CustomerPolicy>();
}

public class CustomerPolicy
{
    public Guid CustomerPolicyId { get; set; } = Guid.NewGuid();
    public Guid PolicyId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal RemainingCoverageAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CustomerPolicyStatus Status { get; set; } = CustomerPolicyStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Policy Policy { get; set; } = null!;
}
