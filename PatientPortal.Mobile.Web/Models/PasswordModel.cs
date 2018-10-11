using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Mobile.Web.Models
{
    public class PasswordModel
    {
        string _password;

        [Required]
        [RegularExpression(@"(?=.*\d)([$@$!%*#?&]?)(?=.*[a-z])(?=.*[A-Z]).{8,}",
             ErrorMessage = "The password must be minimum 8 characters Long, 1 uppercase letter, 1 lowercase letter,1 numeric character")]
        public string Password
        {
            get => _password;
            set => _password = value?.Trim();
        }
    }
}
