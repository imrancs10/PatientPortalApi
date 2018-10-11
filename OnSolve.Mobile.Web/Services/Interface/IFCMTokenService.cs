using OnSolve.Mobile.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services.Interface
{
    public interface IFCMTokenService
    {
        Task PostFCMTokenInfo(int MobileUserId, string fcmToken);
        Task DeleteFCMTokenInfo(int MobileUserId, string fcmToken);
        Task UpdateFCMTokenInfo(int MobileUserId, FCMTokenUpdateModel fcmTokenUpdateModel);
    }
}