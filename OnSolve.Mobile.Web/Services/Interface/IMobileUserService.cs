using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services.Interface
{
    public interface IMobileUserService
    {
        Task SetPassword(MobileUser user, string password);

        Task ChangePassword(MobileUser user, ChangePasswordModel changePasswordModel);

        Task<MobileUser> GetMobileUser(string username);

        Task<MobileUser> GetMobileUser(int mobileUserId);

    }
}