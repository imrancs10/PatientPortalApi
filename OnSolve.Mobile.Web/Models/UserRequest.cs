using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class UserRequest
    {
        [Required]
        public long RecipientId { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public int AccountId { get; set; }

        public long? ENSUserId { get; set; }

        [RegularExpression(@"(?=.*\d)([$@$!%*#?&]?)(?=.*[a-z])(?=.*[A-Z]).{8,}",
            ErrorMessage = "The password must be minimum 8 characters Long, 1 uppercase letter, 1 lowercase letter,1 numeric character")]
        public string Password { get; set; }
    }
}
