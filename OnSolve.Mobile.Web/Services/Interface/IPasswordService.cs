using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Swn402Entities;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services.Interface
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
