using Microsoft.Extensions.Options;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SiteProvisioningWebAppClient.Configuration;
using SiteProvisioningWebAppClient.Helpers;
using SiteProvisioningWebAppClient.Models;

namespace SiteProvisioningWebAppClient.Services
{
    public class SiteProvisionService : ISiteProvisionService
    {
        private readonly CustomSettings _customSettings;

        public SiteProvisionService(IOptions<CustomSettings> settings)
        {
            _customSettings = settings.Value;
        }

        public async Task<string> ProvisionSiteAsync(SiteProvisionRequest request)
        {
            var scopes = new string[] { _customSettings.TenantScopes };
            string adminSiteUrl = _customSettings.AdminURL;
            string siteUrl = _customSettings.SiteURL;
            string accessToken = await SharePointAuthHelper.GetAppPermAccessTokenCSOM(_customSettings, scopes);
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Failed to acquire access token.");
            }

            ClientContext tenantContext = SharePointAuthHelper.GetClientContextWithAccessToken(adminSiteUrl, accessToken);
            var tenant = new Tenant(tenantContext);
            var siteCreationProperties = new SiteCreationProperties
            {
                Url = $"{siteUrl}/{request.SiteName}",
                Title = request.SiteName,
                Owner = request.Alias,
                Template = "STS#3"
            };

            SpoOperation op = tenant.CreateSite(siteCreationProperties);
            tenantContext.Load(op, i => i.IsComplete);
            tenantContext.ExecuteQuery();

            while (!op.IsComplete)
            {
                await Task.Delay(30000);
                op.RefreshLoad();
                tenantContext.ExecuteQuery();
            }

            return $"Site '{request.SiteName}' with alias '{request.Alias}' is being provisioned.";
        }
    }

}
