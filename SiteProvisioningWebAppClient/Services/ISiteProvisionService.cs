using SiteProvisioningWebAppClient.Models;

namespace SiteProvisioningWebAppClient.Services
{
    public interface ISiteProvisionService
    {
        Task<string> ProvisionSiteAsync(SiteProvisionRequest request);
    }
}
