using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Mobile.Web.Models
{
    public class ContactListRequest
    {
        [FromQuery]
        [Required]
        public string Code { get; set; }
    }
}
