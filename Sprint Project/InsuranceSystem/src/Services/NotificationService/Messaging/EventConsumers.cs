using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Services;
using SharedLibrary.Events;
using SharedLibrary.Messaging;

namespace NotificationService.Messaging;

public class UserRegisteredConsumer : RabbitMQConsumerBase
{
    public UserRegisteredConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "user.registered";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<UserRegisteredEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.UserId,
            Message = $"Welcome {evt.Name}! Your account has been created successfully.",
            Type = "UserRegistered"
        });
        await db.SaveChangesAsync();

        // Send welcome email
        await emailSvc.SendEmailAsync(evt.Email, evt.Name,
            "Welcome to SafeHaven Insurance! 🎉",
            $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto;">
              <h2 style="color:#6750A4;">Welcome to SafeHaven, {evt.Name}!</h2>
              <p>Your account has been created successfully. You can now browse and purchase insurance policies.</p>
              <a href="http://localhost:4200/login" style="background:#6750A4;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;display:inline-block;margin-top:16px;">Login Now</a>
              <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
            </div>
            """);
    }
}

public class TicketCreatedConsumer : RabbitMQConsumerBase
{
    public TicketCreatedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "ticket.created";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<TicketCreatedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Your ticket '{evt.Title}' ({evt.Type}) has been created successfully.",
            Type = "TicketCreated"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                $"Ticket Received: {evt.Title}",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#6750A4;">We received your {evt.Type} request</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Your ticket <strong>"{evt.Title}"</strong> has been submitted successfully. Our team will review it shortly.</p>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class TicketAssignedConsumer : RabbitMQConsumerBase
{
    public TicketAssignedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "ticket.assigned";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<TicketAssignedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Your ticket '{evt.TicketTitle}' has been assigned to a specialist.",
            Type = "TicketAssigned"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                $"Your Ticket Has Been Assigned",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#6750A4;">Ticket Assigned to a Specialist</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Your ticket <strong>"{evt.TicketTitle}"</strong> has been assigned to one of our specialists who will get back to you soon.</p>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class TicketUpdatedConsumer : RabbitMQConsumerBase
{
    public TicketUpdatedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "ticket.updated";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<TicketUpdatedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Your ticket '{evt.TicketTitle}' status updated to: {evt.NewStatus}.",
            Type = "TicketUpdated"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                $"Ticket Status Updated: {evt.NewStatus}",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#6750A4;">Ticket Status Update</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Your ticket <strong>"{evt.TicketTitle}"</strong> has been updated to status: <strong>{evt.NewStatus}</strong>.</p>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class ClaimApprovedConsumer : RabbitMQConsumerBase
{
    public ClaimApprovedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "claim.approved";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<ClaimApprovedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Your claim has been APPROVED! Amount: ₹{evt.ClaimAmount:N2}.",
            Type = "ClaimApproved"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                "Your Claim Has Been Approved ✅",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#2E7D32;">Claim Approved!</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Great news! Your insurance claim of <strong>₹{evt.ClaimAmount:N2}</strong> has been <strong style="color:#2E7D32;">APPROVED</strong>.</p>
                  <p>The approved amount has been deducted from your remaining coverage balance. Please log in to your dashboard to view the updated details.</p>
                  <a href="http://localhost:4200/dashboard" style="background:#6750A4;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;display:inline-block;margin-top:16px;">View Dashboard</a>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class ClaimRejectedConsumer : RabbitMQConsumerBase
{
    public ClaimRejectedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "claim.rejected";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<ClaimRejectedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Your claim has been REJECTED. Reason: {evt.Reason}.",
            Type = "ClaimRejected"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                "Update on Your Insurance Claim",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#C62828;">Claim Status Update</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>We regret to inform you that your claim has been <strong style="color:#C62828;">REJECTED</strong>.</p>
                  <p><strong>Reason:</strong> {evt.Reason}</p>
                  <p>If you believe this is an error, please contact our support team.</p>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class PaymentCompletedConsumer : RabbitMQConsumerBase
{
    public PaymentCompletedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "payment.completed";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<PaymentCompletedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Payment of ₹{evt.Amount:N2} {(evt.IsSuccess ? "completed" : "failed")}. Ref: {evt.TransactionReference}.",
            Type = "PaymentCompleted"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            var statusColor = evt.IsSuccess ? "#2E7D32" : "#C62828";
            var statusText = evt.IsSuccess ? "SUCCESSFUL" : "FAILED";
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                $"Payment {statusText} — ₹{evt.Amount:N2}",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:{statusColor};">Payment {statusText}</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Your payment of <strong>₹{evt.Amount:N2}</strong> was <strong style="color:{statusColor};">{statusText}</strong>.</p>
                  <p><strong>Transaction Reference:</strong> {evt.TransactionReference}</p>
                  {(evt.IsSuccess ? "<p>Your policy is now active. Thank you for choosing SafeHaven!</p>" : "<p>Please try again or contact support if the issue persists.</p>")}
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}

public class PolicyPurchasedConsumer : RabbitMQConsumerBase
{
    public PolicyPurchasedConsumer(IServiceProvider sp, IConfiguration cfg)
        : base(sp, cfg["RabbitMQ:Host"] ?? "localhost") { }
    protected override string QueueName => "policy.purchased";
    protected override async Task HandleMessageAsync(string message, IServiceScope scope)
    {
        var evt = Deserialize<PolicyPurchasedEvent>(message);
        if (evt == null) return;
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userLookup = scope.ServiceProvider.GetRequiredService<IUserLookupService>();

        await db.Notifications.AddAsync(new Notification
        {
            UserId = evt.CustomerId,
            Message = $"Policy '{evt.PolicyName}' is now active. Valid: {evt.StartDate:d} to {evt.EndDate:d}.",
            Type = "PolicyPurchased"
        });
        await db.SaveChangesAsync();

        var user = await userLookup.GetUserEmailAsync(evt.CustomerId);
        if (user != null)
        {
            await emailSvc.SendEmailAsync(user.Value.Email, user.Value.Name,
                $"Policy Activated: {evt.PolicyName} 🎉",
                $"""
                <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                  <h2 style="color:#6750A4;">Your Policy is Active!</h2>
                  <p>Hi {user.Value.Name},</p>
                  <p>Your policy <strong>"{evt.PolicyName}"</strong> is now active and you are covered!</p>
                  <table style="border-collapse:collapse;width:100%;margin-top:16px;">
                    <tr style="background:#f3f0f9;"><td style="padding:8px 12px;font-weight:bold;">Start Date</td><td style="padding:8px 12px;">{evt.StartDate:D}</td></tr>
                    <tr><td style="padding:8px 12px;font-weight:bold;">End Date</td><td style="padding:8px 12px;">{evt.EndDate:D}</td></tr>
                  </table>
                  <a href="http://localhost:4200/dashboard" style="background:#6750A4;color:#fff;padding:12px 24px;border-radius:8px;text-decoration:none;display:inline-block;margin-top:16px;">View My Policy</a>
                  <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
                </div>
                """);
        }
    }
}
