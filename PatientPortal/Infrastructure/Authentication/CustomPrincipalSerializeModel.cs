using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Infrastructure.Authentication
{
    public class CustomPrincipalSerializeModel
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PINCode { get; set; }
        public string Religion { get; set; }
        public string Department { get; set; }

    }
}