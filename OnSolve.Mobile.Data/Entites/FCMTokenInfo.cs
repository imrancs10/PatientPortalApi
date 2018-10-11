using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Data.Entites
{
    public class FCMTokenInfo
    {
        public int Id { get; set; }

        [Required]
        public string FCMToken { get; set; }

        [Required]
        public MobileUser MobileUser { get; set; }

    }

}