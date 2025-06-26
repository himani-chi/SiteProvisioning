using System.ComponentModel.DataAnnotations;

namespace SiteProvisioningWebAppClient.Models
{
    public class SiteProvisionRequest
    {
        [Required(ErrorMessage = "Site Name is required")]
        [MaxLength(100, ErrorMessage = "Site Name cannot exceed 100 characters")]
        public required string SiteName { get; set; }

        [Required(ErrorMessage = "Alias is required")]
        [MaxLength(50, ErrorMessage = "Alias cannot exceed 50 characters")]
        public required string Alias { get; set; }
    }
}
