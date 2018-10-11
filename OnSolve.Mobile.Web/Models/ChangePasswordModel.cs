using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class ChangePasswordModel
    {
        const string passwordPattern = @"(?=.*\d)([$@$!%*#?&]?)(?=.*[a-z])(?=.*[A-Z]).{8,}";
        const string passwordValidationMessage = "The password must be minimum 8 characters Long, " +
                                                            "1 uppercase letter, 1 lowercase letter,1 numeric character";
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(passwordPattern, ErrorMessage = passwordValidationMessage)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [RegularExpression(passwordPattern, ErrorMessage = passwordValidationMessage)]
        public string ConfirmPassword { get; set; }
    }
}
