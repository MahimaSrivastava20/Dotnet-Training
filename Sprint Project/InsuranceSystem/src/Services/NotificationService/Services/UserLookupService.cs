using Microsoft.Data.SqlClient;

namespace NotificationService.Services;

public interface IUserLookupService
{
    Task<(string Email, string Name)?> GetUserEmailAsync(Guid userId);
}

/// <summary>
/// Looks up user email directly from the IdentityService database via raw SQL.
/// This avoids needing an HTTP call and keeps things fast.
/// </summary>
public class UserLookupService : IUserLookupService
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserLookupService> _logger;

    public UserLookupService(IConfiguration config, ILogger<UserLookupService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<(string Email, string Name)?> GetUserEmailAsync(Guid userId)
    {
        try
        {
            var connStr = _config.GetConnectionString("IdentityConnection");
            if (string.IsNullOrEmpty(connStr)) return null;

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT Email, Name FROM Users WHERE UserId = @UserId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId.ToString());
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (reader.GetString(0), reader.GetString(1));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not look up email for user {UserId}", userId);
        }
        return null;
    }
}
