using AdminService.CQRS.Queries;
using AdminService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace AdminService.CQRS.Handlers;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IConfiguration _config;
    public GetDashboardHandler(IConfiguration config) => _config = config;

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var baseConnStr = _config.GetConnectionString("DefaultConnection") ?? "";
        if (!baseConnStr.Contains("TrustServerCertificate"))
        {
            baseConnStr += ";TrustServerCertificate=True";
        }
        
        var identityConn = baseConnStr.Replace("InsuranceAdminDb", "InsuranceIdentityDb");
        var policyConn = baseConnStr.Replace("InsuranceAdminDb", "InsurancePolicyDb");
        var ticketConn = baseConnStr.Replace("InsuranceAdminDb", "InsuranceTicketDb");
        var paymentConn = baseConnStr.Replace("InsuranceAdminDb", "InsurancePaymentDb");
        var notifConn = baseConnStr.Replace("InsuranceAdminDb", "InsuranceNotificationDb");

        var dto = new DashboardDto();

        try { using var c = new SqlConnection(identityConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Users", c); dto.TotalUsers = (int)await cmd.ExecuteScalarAsync(); } catch (Exception ex) { Console.WriteLine("Error Users: " + ex.Message); }
        try { using var c = new SqlConnection(policyConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Policies", c); dto.TotalPolicies = (int)await cmd.ExecuteScalarAsync(); } catch (Exception ex) { Console.WriteLine("Error Policies: " + ex.Message); }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Tickets", c); dto.TotalTickets = (int)await cmd.ExecuteScalarAsync(); } catch (Exception ex) { Console.WriteLine("Error Tickets: " + ex.Message); }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE Status = 'Open'", c); dto.OpenTickets = (int)await cmd.ExecuteScalarAsync(); } catch { }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM ClaimDetails", c); dto.TotalClaims = (int)await cmd.ExecuteScalarAsync(); } catch (Exception ex) { Console.WriteLine("Error Claims: " + ex.Message); }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM ClaimDetails WHERE ApprovalStatus = 'Pending'", c); dto.PendingClaims = (int)await cmd.ExecuteScalarAsync(); } catch { }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM ClaimDetails WHERE ApprovalStatus = 'Approved'", c); dto.ApprovedClaims = (int)await cmd.ExecuteScalarAsync(); } catch { }
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM ClaimDetails WHERE ApprovalStatus = 'Rejected'", c); dto.RejectedClaims = (int)await cmd.ExecuteScalarAsync(); } catch { }
        try { using var c = new SqlConnection(paymentConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Payments", c); dto.TotalPayments = (int)await cmd.ExecuteScalarAsync(); } catch (Exception ex) { Console.WriteLine("Error Payments: " + ex.Message); }
        try { using var c = new SqlConnection(paymentConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE Status = 'Completed'", c); var res = await cmd.ExecuteScalarAsync(); dto.TotalPaymentsAmount = res == DBNull.Value ? 0 : Convert.ToDecimal(res); } catch { }
        try { using var c = new SqlConnection(notifConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Notifications", c); dto.TotalNotifications = (int)await cmd.ExecuteScalarAsync(); } catch { }

        // Fetch Total Queries
        try { using var c = new SqlConnection(ticketConn); await c.OpenAsync(); using var cmd = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE Type = 'Support'", c); dto.TotalQueries = (int)await cmd.ExecuteScalarAsync(); } catch { }

        return dto;
    }
}

public class GetTicketReportHandler : IRequestHandler<GetTicketReportQuery, List<TicketReportItemDto>>
{
    private readonly IConfiguration _config;
    public GetTicketReportHandler(IConfiguration config) => _config = config;

    public async Task<List<TicketReportItemDto>> Handle(GetTicketReportQuery request, CancellationToken cancellationToken)
    {
        var list = new List<TicketReportItemDto>();
        var baseConnStr = _config.GetConnectionString("DefaultConnection") ?? "";
        if (!baseConnStr.Contains("TrustServerCertificate")) baseConnStr += ";TrustServerCertificate=True";
        var ticketConn = baseConnStr.Replace("InsuranceAdminDb", "InsuranceTicketDb");

        try {
            using var c = new SqlConnection(ticketConn);
            await c.OpenAsync(cancellationToken);
            var query = "SELECT Type, Status, COUNT(*) as Count, MAX(CreatedAt) as Period FROM Tickets WHERE 1=1";
            using var cmd = new SqlCommand(query, c);
            
            if (request.From.HasValue) {
                cmd.CommandText += " AND CreatedAt >= @from";
                cmd.Parameters.AddWithValue("@from", request.From.Value);
            }
            if (request.To.HasValue) {
                cmd.CommandText += " AND CreatedAt <= @to";
                cmd.Parameters.AddWithValue("@to", request.To.Value);
            }
            cmd.CommandText += " GROUP BY Type, Status";

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken)) {
                list.Add(new TicketReportItemDto {
                    Type = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Status = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1),
                    Count = reader.GetInt32(2),
                    Period = reader.IsDBNull(3) ? DateTime.UtcNow : reader.GetDateTime(3)
                });
            }
        } catch (Exception ex) { Console.WriteLine("Report error: " + ex.Message); }
        return list;
    }
}

public class GetClaimReportHandler : IRequestHandler<GetClaimReportQuery, List<ClaimReportItemDto>>
{
    private readonly IConfiguration _config;
    public GetClaimReportHandler(IConfiguration config) => _config = config;

    public async Task<List<ClaimReportItemDto>> Handle(GetClaimReportQuery request, CancellationToken cancellationToken)
    {
        var list = new List<ClaimReportItemDto>();
        var baseConnStr = _config.GetConnectionString("DefaultConnection") ?? "";
        if (!baseConnStr.Contains("TrustServerCertificate")) baseConnStr += ";TrustServerCertificate=True";
        var ticketConn = baseConnStr.Replace("InsuranceAdminDb", "InsuranceTicketDb");

        try {
            using var c = new SqlConnection(ticketConn);
            await c.OpenAsync(cancellationToken);
            var query = "SELECT ApprovalStatus, COUNT(*) as Count, SUM(ClaimAmount) as TotalAmount FROM ClaimDetails WHERE 1=1";
            using var cmd = new SqlCommand(query, c);

            if (request.From.HasValue) {
                cmd.CommandText += " AND CreatedAt >= @from";
                cmd.Parameters.AddWithValue("@from", request.From.Value);
            }
            if (request.To.HasValue) {
                cmd.CommandText += " AND CreatedAt <= @to";
                cmd.Parameters.AddWithValue("@to", request.To.Value);
            }
            cmd.CommandText += " GROUP BY ApprovalStatus";

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken)) {
                list.Add(new ClaimReportItemDto {
                    ApprovalStatus = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.GetInt32(1),
                    TotalAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                });
            }
        } catch (Exception ex) { Console.WriteLine("Report error: " + ex.Message); }
        return list;
    }
}

public class GetPaymentReportHandler : IRequestHandler<GetPaymentReportQuery, List<PaymentReportItemDto>>
{
    private readonly IConfiguration _config;
    public GetPaymentReportHandler(IConfiguration config) => _config = config;

    public async Task<List<PaymentReportItemDto>> Handle(GetPaymentReportQuery request, CancellationToken cancellationToken)
    {
        var list = new List<PaymentReportItemDto>();
        var baseConnStr = _config.GetConnectionString("DefaultConnection") ?? "";
        if (!baseConnStr.Contains("TrustServerCertificate")) baseConnStr += ";TrustServerCertificate=True";
        var paymentConn = baseConnStr.Replace("InsuranceAdminDb", "InsurancePaymentDb");

        try {
            using var c = new SqlConnection(paymentConn);
            await c.OpenAsync(cancellationToken);
            var query = "SELECT Status, COUNT(*) as Count, SUM(Amount) as TotalAmount FROM Payments WHERE 1=1";
            using var cmd = new SqlCommand(query, c);
            
            if (request.From.HasValue) {
                cmd.CommandText += " AND CreatedAt >= @from";
                cmd.Parameters.AddWithValue("@from", request.From.Value);
            }
            if (request.To.HasValue) {
                cmd.CommandText += " AND CreatedAt <= @to";
                cmd.Parameters.AddWithValue("@to", request.To.Value);
            }
            cmd.CommandText += " GROUP BY Status";

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken)) {
                list.Add(new PaymentReportItemDto {
                    Status = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Count = reader.GetInt32(1),
                    TotalAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                });
            }
        } catch (Exception ex) { Console.WriteLine("Report error: " + ex.Message); }
        return list;
    }
}
