using OnSolve.Mobile.Web.Models;

namespace OnSolve.Mobile.Web.Infrastructure.Utilities
{
    public interface IPasswordHasherService
    {
        string GetPasswordHash(string password, string salt);
        string GetSalt();
        bool IsValidPassword(ChangePasswordModel passwordModel, string hashedPassword, string saltString);
    }
}
