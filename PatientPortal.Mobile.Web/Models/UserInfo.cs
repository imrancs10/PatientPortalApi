using System;
using System.Security.Claims;

namespace PatientPortal.Mobile.Web.Models
{
    public class UserInfo
    {
        public UserInfo(ClaimsPrincipal claimsPrincipal)
        {
            foreach (var claim in claimsPrincipal.Claims)
            {
                switch (claim.Type)
                {
                    case nameof(MobileUserId):
                        MobileUserId = Convert.ToInt32(claim.Value);
                        break;

                    case nameof(AccountId):
                        AccountId = Convert.ToInt32(claim.Value);
                        break;
                    case nameof(UserId):
                        UserId = Convert.ToInt64(claim.Value);
                        break;

                }
            }
        }

        public int MobileUserId { get; }

        public int AccountId { get; }

        public long UserId { get; }

    }
}
