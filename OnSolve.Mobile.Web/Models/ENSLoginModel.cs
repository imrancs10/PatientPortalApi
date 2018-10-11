using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class ENSLoginModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}