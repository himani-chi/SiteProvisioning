using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using SiteProvisioningWebAppClient.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace SiteProvisioningWebAppClient.Helpers
{
    public static class SharePointAuthHelper
    {
        static X509Certificate2 GetAppOnlyCertificate(string thumbPrint)
        {
            X509Certificate2 appOnlyCertificate = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                certStore.Close();

                return appOnlyCertificate;
            }
        }
        static async Task<X509Certificate2> GetAppOnlyCertificateFromKeyVault(string vaultUrl, string certName)
        {
            var client = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());

            KeyVaultSecret secret = await client.GetSecretAsync(certName);
            byte[] privateKeyBytes = Convert.FromBase64String(secret.Value);
            return new X509Certificate2(privateKeyBytes, (string?)null, X509KeyStorageFlags.MachineKeySet);
        }
        public static async Task<string> GetAppPermAccessTokenCSOM(CustomSettings customSettings, string[] scopes)
        {
            string tenantID = customSettings.TenantId;
            string clientId = customSettings.ClientId;

            //X509Certificate2 certificate = GetAppOnlyCertificate(customSettings.CertThumprint);
            // Fetch cert from AKV
            X509Certificate2 certificate = await GetAppOnlyCertificateFromKeyVault(
                customSettings.KeyVaultUrl, customSettings.CertName);

            string accessToken = null;

            try
            {
                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                .Create(clientId)
                                .WithCertificate(certificate)
                                .WithTenantId(tenantID)
                                .Build();

                AuthenticationResult authResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();
                accessToken = authResult.AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return accessToken;
        }
        public static ClientContext GetClientContextWithAccessToken(string targetUrl, string accessToken)
        {
            ClientContext clientContext = new(targetUrl);

            clientContext.ExecutingWebRequest += (sender, eventArgs) =>
            {
                eventArgs.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
            };
            return clientContext;
        }
    }
}
