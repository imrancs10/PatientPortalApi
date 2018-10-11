using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Models;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Services.Interface
{
    public interface IMobileUserService
    {
        Task SetPassword(MobileUser user, string password);

        Task ChangePassword(MobileUser user, ChangePasswordModel changePasswordModel);

        Task<MobileUser> GetMobileUser(string username);

        Task<MobileUser> GetMobileUser(int mobileUserId);

    }
}