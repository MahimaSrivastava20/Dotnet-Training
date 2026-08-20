namespace IdentityService.Models;

public enum UserRole
{
    Customer,
    ClaimsSpecialist,
    SupportSpecialist,
    Admin
}

public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // OTP Email Verification
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool IsEmailVerified { get; set; } = false;
}
