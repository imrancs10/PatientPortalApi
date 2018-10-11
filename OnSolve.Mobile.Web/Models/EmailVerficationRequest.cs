using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Models
{
    public class EmailVerificationRequest
    {
        [FromQuery]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
