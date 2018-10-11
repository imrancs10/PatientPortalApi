using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class FCMTokenRequest
    {
        [Required]
        public string FCMToken { get; set; }
    }
}
