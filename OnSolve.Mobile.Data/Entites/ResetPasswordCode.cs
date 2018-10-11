using System;
using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Data.Entites
{
    public class ResetPasswordCode
    {
        public int Id { get; set; }

        [Required]
        public string ResetCode { get; set; }

        [Required]
        public MobileUser MobileUser { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }
    }
}
