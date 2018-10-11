using System.Collections.Generic;

namespace PatientPortal.Mobile.Web.Models
{
    public class ContactListResponse
    {
        public string Email { get; set; }
        public List<ContactDetailModel> ContactList { get; set; }
    }
}
