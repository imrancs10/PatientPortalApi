using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace PatientPortal.Models
{
    public class UserInfo
    {
        public UserInfo(IPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;

            var claims = from c in identity.Claims
                         select new 
                         {
                             subject = c.Subject.Name,
                             type = c.Type,
                             value = c.Value
                         };

            //foreach (var claim in claims)
            //{
            //    switch (claim.Type)
            //    {
            //        case nameof(PatientId):
            //            PatientId = Convert.ToInt32(claim.Value);
            //            break;

            //        case nameof(RegistrationNumber):
            //            RegistrationNumber = Convert.ToString(claim.Value);
            //            break;
            //        case nameof(ValidUpTo):
            //            ValidUpTo = Convert.ToDateTime(claim.Value);
            //            break;
            //        case nameof(CRNumber):
            //            CRNumber = Convert.ToString(claim.Value);
            //            break;
            //    }
            //}
        }
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ValidUpTo { get; set; }
        public string CRNumber { get; set; }
    }
}