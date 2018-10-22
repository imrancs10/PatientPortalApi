using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models.User
{
    public class UserClaims
    {
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ValidUpTo { get; set; }
        public string CRNumber { get; set; }
    }
}