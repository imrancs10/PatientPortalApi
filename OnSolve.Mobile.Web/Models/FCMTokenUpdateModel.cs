using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class FCMTokenUpdateModel
    {
        [Required]
        public string FCMToken { get; set; }

        [Required]
        public string OldFCMToken { get; set; }
    }
}
