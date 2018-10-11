using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Mobile.Web.Models
{
    public class FCMTokenRequest
    {
        [Required]
        public string FCMToken { get; set; }
    }
}
