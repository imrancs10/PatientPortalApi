using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Models;
using PatientPortal.Mobile.Web.Swn402Entities;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Services.Interface
{
    public interface IPasswordService
    {
        Task RequestPasswordResetCode(ContactDetail contactDetail);
        Task ResetUserPassword(MobileUser user, string password);
        Task<ResetPasswordCode> GetResetCodeInfo(string resetcode);
        bool HasResetCodeExpired(ResetPasswordCode resetCode);
        Task UpdateResetCode(MobileUser mobileUser);
        Task DeleteResetCode(MobileUser mobileUser);
    }
}
