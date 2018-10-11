using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Mobile.Web.Models
{
    public class ENSLoginModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}