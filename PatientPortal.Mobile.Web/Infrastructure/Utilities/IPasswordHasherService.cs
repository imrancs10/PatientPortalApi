using PatientPortal.Mobile.Web.Models;

namespace PatientPortal.Mobile.Web.Infrastructure.Utilities
{
    public interface IPasswordHasherService
    {
        string GetPasswordHash(string password, string salt);
        string GetSalt();
        bool IsValidPassword(ChangePasswordModel passwordModel, string hashedPassword, string saltString);
    }
}
