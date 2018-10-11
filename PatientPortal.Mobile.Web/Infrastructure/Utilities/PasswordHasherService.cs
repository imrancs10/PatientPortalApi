using PatientPortal.Mobile.Web.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PatientPortal.Mobile.Web.Infrastructure.Utilities
{
    public class PasswordHasherService : IPasswordHasherService
    {

        public string GetPasswordHash(string password, string salt)
        {
            var encoding = new UnicodeEncoding();
            using (var provider = new SHA256CryptoServiceProvider())
            {
                return encoding.GetString(provider.ComputeHash(encoding.GetBytes(password + salt)));
            }
        }

        public string GetSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public bool IsValidPassword(ChangePasswordModel passwordModel, string hashedPassword, string saltString)
        {
            return hashedPassword == GetPasswordHash(passwordModel.OldPassword, saltString) && passwordModel.NewPassword != passwordModel.OldPassword;
        }
    }
}
