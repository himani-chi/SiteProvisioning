using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Identity.Web.Resource;
using SiteProvisioningWebAppClient.Models;
using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using System.Net.Http.Json;
using SiteProvisioningWebAppClient.Configuration;
using SiteProvisioningWebAppClient.Helpers;
using Microsoft.Extensions.Options;

namespace SiteProvisioningWebAppClient.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class SiteProvisionController(IOptions<CustomSettings> customSettings) : Controller
    {
        private readonly CustomSettings _customSettings = customSettings.Value;

        [HttpPost]
        public async Task<IActionResult> ProvisionSite([FromBody] SiteProvisionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(request.SiteName) || string.IsNullOrEmpty(request.Alias))
            {
                return BadRequest("SiteName and Alias are required.");
            }

            var scopes = new string[] { _customSettings.TenantScopes };
            string adminSiteUrl = _customSettings.AdminURL;
            string siteUrl = _customSettings.SiteURL;
            string accessToken = await SharePointAuthHelper.GetAppPermAccessTokenCSOM(_customSettings, scopes);
            if (string.IsNullOrEmpty(accessToken))
            {
                return StatusCode(500, "Provision error: Failed to acquire access token.");
            }
            ClientContext tenantContext = SharePointAuthHelper.GetClientContextWithAccessToken(adminSiteUrl, accessToken);

            var tenant = new Tenant(tenantContext);
            var siteCreationProperties = new SiteCreationProperties()
            {
                Url = $"{siteUrl}/{request.SiteName}",
                Title = request.SiteName,
                Owner = request.Alias,
                Template = "STS#3"
            };

            SpoOperation op = tenant.CreateSite(siteCreationProperties);
            tenantContext.Load(tenant);
            tenantContext.Load(op, i => i.IsComplete);
            tenantContext.ExecuteQuery();

            while (!op.IsComplete)
            {
                //Wait for 30 seconds and then try again
                await Task.Delay(30000);
                op.RefreshLoad();
                tenantContext.ExecuteQuery();
            }
            return Ok($"Site '{request.SiteName}' with alias '{request.Alias}' is being provisioned.");

        }
    }
}
