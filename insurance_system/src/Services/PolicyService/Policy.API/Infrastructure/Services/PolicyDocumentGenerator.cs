using System;
using System.Text;
using Policy.API.Domain.Entities;

namespace Policy.API.Infrastructure.Services
{
    public interface IPolicyDocumentGenerator
    {
        string GeneratePolicyDocumentText(UserPolicy policy);
    }

    public class PolicyDocumentGenerator : IPolicyDocumentGenerator
    {
        public string GeneratePolicyDocumentText(UserPolicy policy)
        {
            var sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine("                 OFFICIAL INSURANCE POLICY CERTIFICATE                     ");
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"Policy Number     : {policy.PolicyNumber}");
            sb.AppendLine($"Policy Name       : {policy.PolicyName}");
            sb.AppendLine($"Policy Type       : {policy.Type}");
            sb.AppendLine($"Holder User ID    : {policy.UserId}");
            sb.AppendLine($"Effective Date    : {policy.StartDate:yyyy-MM-dd}");
            sb.AppendLine($"Expiration Date   : {policy.EndDate:yyyy-MM-dd}");
            sb.AppendLine($"Total Premium Paid: {policy.FinalPremium:C2} INR");
            sb.AppendLine($"Razorpay Order ID : {policy.RazorpayOrderId}");
            sb.AppendLine($"Razorpay PaymentId: {policy.RazorpayPaymentId}");
            sb.AppendLine($"Status            : {(policy.IsActive ? "ACTIVE / IN FORCE" : "EXPIRED")}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine("This document serves as proof of insurance coverage issued by the system.");
            sb.AppendLine("==========================================================================");

            return sb.ToString();
        }
    }
}
