using PolicyService.Services;
using RabbitMQ.Client.Events;
using SharedLibrary.Events;
using SharedLibrary.Messaging;
using System.Text;
using System.Text.Json;

namespace PolicyService.Messaging;

public class PaymentCompletedConsumer : RabbitMQConsumerBase
{
    public PaymentCompletedConsumer(IServiceProvider sp, IConfiguration config)
        : base(sp, config["RabbitMQ:Host"] ?? "localhost") { }

    protected override string QueueName => "payment.completed";

    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<PaymentCompletedEvent>(message);
        if (evt == null || !evt.IsSuccess) return;

        var policyService = scope.ServiceProvider.GetRequiredService<IPolicyService>();
        await policyService.ActivatePolicyAsync(evt.CustomerId, evt.PolicyId);
    }
}
