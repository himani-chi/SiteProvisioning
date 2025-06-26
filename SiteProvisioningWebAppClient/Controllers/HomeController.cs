using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SiteProvisioningWebAppClient.Models;
using SiteProvisioningWebAppClient.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SiteProvisioningWebAppClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISiteProvisionService _siteProvisionService;

        public HomeController(ILogger<HomeController> logger, ISiteProvisionService siteProvisionService)
        {
            _logger = logger;
            _siteProvisionService = siteProvisionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SiteProvisionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _siteProvisionService.ProvisionSiteAsync(request);
                ViewBag.ApiResponse = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provisioning failed.");
                ViewBag.ApiResponse = "An error occurred while provisioning the site.";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
