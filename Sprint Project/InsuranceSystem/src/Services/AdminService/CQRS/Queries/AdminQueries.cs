using MediatR;

namespace AdminService.CQRS.Queries;

public record GetDashboardQuery : IRequest<DashboardDto>;
public record GetTicketReportQuery(DateTime? From, DateTime? To) : IRequest<List<TicketReportItemDto>>;
public record GetClaimReportQuery(DateTime? From, DateTime? To) : IRequest<List<ClaimReportItemDto>>;
public record GetPaymentReportQuery(DateTime? From, DateTime? To) : IRequest<List<PaymentReportItemDto>>;

public class DashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int TotalClaims { get; set; }
    public int ApprovedClaims { get; set; }
    public int RejectedClaims { get; set; }
    public int TotalPolicies { get; set; }
    public decimal TotalPaymentsAmount { get; set; }
    public int TotalPayments { get; set; }
    public int TotalNotifications { get; set; }
    public int TotalQueries { get; set; }
    public int PendingClaims { get; set; }
}

public class TicketReportItemDto
{
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime Period { get; set; }
}

public class ClaimReportItemDto
{
    public string ApprovalStatus { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PaymentReportItemDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}
