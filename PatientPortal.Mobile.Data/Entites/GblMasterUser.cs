using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class GblMasterUser
    {
        public GblMasterUser()
        {
            GblMasterLogin = new HashSet<GblMasterLogin>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DoB { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string IsdCode { get; set; }
        public bool IsSync { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public ICollection<GblMasterLogin> GblMasterLogin { get; set; }
    }
}
