using OnSolve.Mobile.Web.Models;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Infrastructure.Adaptors
{
    public interface IEmailService
    {
        Task SendDeviceVerificationMessageAsync(string receiver, string name, string url);
        Task SendContactVerificationMessageAsync(string receiver, string url);
        Task SendTwoFAEmailMessageAsync(string receiver, string name, string otp);
        Task SendForgotPasswordEmailAsync(string receiver, string name, string url);
        Task<bool> TrySendEmail(EmailRequest email);
    }
}
