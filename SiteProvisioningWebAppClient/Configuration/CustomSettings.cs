namespace SiteProvisioningWebAppClient.Configuration
{
    public class CustomSettings : ICustomSettings
    {
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
        public string TenantId { get; set; } = String.Empty;
        public string RedirectUri { get; set; } = String.Empty;
        public string AdminURL { get; set; } = String.Empty;
        public string SiteURL { get; set; } = String.Empty;
        public string Scopes { get; set; } = String.Empty;
        public string TenantScopes { get; set; } = String.Empty;
        public string CSOMScopes { get; set; } = String.Empty;
        public string GraphScopes { get; set; } = String.Empty;
        public string CertThumprint { get; set; } = String.Empty;
        public string KeyVaultUrl { get; set; } = String.Empty;
        public string CertName { get; set; } = String.Empty;

    }
}
