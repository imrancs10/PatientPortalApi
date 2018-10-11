using OnSolve.Mobile.Web.Swn402Entities;
using System.Collections.Generic;

namespace OnSolve.Mobile.Web.Models
{
    public class ContactListResponse
    {
        public string Email { get; set; }
        public List<ContactDetailModel> ContactList { get; set; }
    }
}
