namespace Insurance.Shared.Security
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = "SuperSecretInsuranceSystemJwtKey_MustBeAtLeast32BytesLong!";
        public string Issuer { get; set; } = "InsuranceSystemGateway";
        public string Audience { get; set; } = "InsuranceSystemClients";
        public int ExpirationMinutes { get; set; } = 1440; // 24 hours
    }
}
