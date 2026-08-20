namespace SharedLibrary.Events;

public class PolicyPurchasedEvent : BaseEvent
{
    public Guid CustomerPolicyId { get; set; }
    public Guid PolicyId { get; set; }
    public Guid CustomerId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
