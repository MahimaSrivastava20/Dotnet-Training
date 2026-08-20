using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Services;
using SharedLibrary.DTOs;
using System.Security.Claims;

namespace NotificationService.Controllers;

[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _ctx;
    private readonly IEmailService _emailService;

    public NotificationsController(NotificationDbContext ctx, IEmailService emailService)
    {
        _ctx = ctx;
        _emailService = emailService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("my")]
    public async Task<IActionResult> GetMyNotifications()
    {
        var notifications = await _ctx.Notifications
            .Where(n => n.UserId == GetUserId())
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(ApiResponse<List<Notification>>.Ok(notifications));
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var n = await _ctx.Notifications.FindAsync(id);
        if (n == null || n.UserId != GetUserId()) return NotFound(ApiResponse.Fail("Not found"));
        n.IsRead = true;
        await _ctx.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Marked as read"));
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        var unread = await _ctx.Notifications
            .Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        unread.ForEach(n => n.IsRead = true);
        await _ctx.SaveChangesAsync();
        return Ok(ApiResponse.Ok($"{unread.Count} notifications marked as read"));
    }

    /// <summary>Test endpoint — sends a test email directly to verify SMTP works</summary>
    [HttpPost("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> TestEmail([FromHeader(Name = "X-Internal-Key")] string? key)
    {
        if (key != "InsuranceInternalKey2024")
            return Unauthorized(ApiResponse.Fail("Unauthorized"));

        await _emailService.SendEmailAsync(
            "srimahima5@gmail.com",
            "Mahima",
            "✅ SafeHaven Email Test",
            """
            <div style="font-family:sans-serif;max-width:600px;margin:auto;">
              <h2 style="color:#6750A4;">Email Notifications are Working! 🎉</h2>
              <p>Hi Mahima,</p>
              <p>This is a test email from your <strong>SafeHaven Insurance</strong> system.</p>
              <p>If you can see this, your email notifications are correctly configured and will be sent automatically for all future events like claim approvals, payments, and policy activations.</p>
              <p style="color:#999;margin-top:24px;font-size:12px;">SafeHaven Insurance — Confidence in every claim.</p>
            </div>
            """);

        return Ok(ApiResponse.Ok("Test email sent to srimahima5@gmail.com. Check your inbox!"));
    }
}
