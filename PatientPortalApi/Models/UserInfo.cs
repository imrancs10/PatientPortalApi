using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace PatientPortalApi.Models
{
    public class UserInfo
    {
        public UserInfo(IPrincipal user)
        {
            //var identity = user.Identity as ClaimsIdentity;

            var claims = ((ClaimsPrincipal)user).Claims;

            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case nameof(PatientId):
                        PatientId = Convert.ToInt32(claim.Value);
                        break;

                    case nameof(RegistrationNumber):
                        RegistrationNumber = Convert.ToString(claim.Value);
                        break;
                    case nameof(ValidUpTo):
                        ValidUpTo = Convert.ToDateTime(claim.Value);
                        break;
                    case nameof(CRNumber):
                        CRNumber = Convert.ToString(claim.Value);
                        break;
                }
            }
        }
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ValidUpTo { get; set; }
        public string CRNumber { get; set; }
    }
}