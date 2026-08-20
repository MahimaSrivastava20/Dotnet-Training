using PolicyService.Data;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace PolicyService.Messaging;

public class ClaimApprovedConsumer : RabbitMQConsumerBase
{
    public ClaimApprovedConsumer(IServiceProvider sp, IConfiguration config)
        : base(sp, config["RabbitMQ:Host"] ?? "localhost") { }

    protected override string QueueName => "claim.approved";

    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<ClaimApprovedEvent>(message);
        if (evt == null || !evt.PolicyId.HasValue) return;

        var ctx = scope.ServiceProvider.GetRequiredService<PolicyDbContext>();
        
        var cp = await ctx.CustomerPolicies.FindAsync(evt.PolicyId.Value);
        if (cp != null)
        {
            cp.RemainingCoverageAmount -= evt.ClaimAmount;
            if (cp.RemainingCoverageAmount < 0) cp.RemainingCoverageAmount = 0;
            await ctx.SaveChangesAsync();
        }
    }
}
