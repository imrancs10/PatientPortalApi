using System;
using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Data.Entites
{
    public class EmailVerificationCode
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; }

    }
}
