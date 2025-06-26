namespace SiteProvisioningWebAppClient.Configuration
{
    public interface ICustomSettings
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string TenantId { get; set; }
        string RedirectUri { get; set; }
        string AdminURL { get; set; }
        string SiteURL { get; set; }
        string Scopes { get; set; }
        string TenantScopes { get; set; }
        string CSOMScopes { get; set; }
        string GraphScopes { get; set; }
        string CertThumprint { get; set; }
        string KeyVaultUrl { get; set; }
        string CertName { get; set; }

    }
}
