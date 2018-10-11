using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Data.Entites
{
    public class MobileRecipient
    {
        public long Id { get; set; }
        [Required]
        public long MobileUserId { get; set; }
    }
}