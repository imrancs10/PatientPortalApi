using Microsoft.Extensions.Configuration;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Infrastructure.Adaptors
{
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration { get; }
        private const string _fromEmailConfiguration = "EmailSetting:Email:0:FromEmail";
        private const string _fromNameConfiguration = "EmailSetting:Email:0:FromName";
        private const string _apiKeyConfiguration = "EmailSetting:Email:0:ApiKey";
        private const string _twoFaMailSubject = "Onsolve Two Factor Authentication";
        private const string _deviceVerificationMailSubject = "Onsolve Device Verification";
        private const string _forgetPasswordMailSubject = "Onsolve Forget Password";
        private const string _emailVerificationMailSubject = "Verify Email Address";
        private const string _baseApiUrl= "__BaseApiUrl__";

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> TrySendEmail(EmailRequest email)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(
                _configuration[_fromEmailConfiguration],
                _configuration[_fromNameConfiguration]));

            msg.AddTo(new EmailAddress(email.EmailTo, email.NameTo));

            msg.SetSubject(email.Subject);
            msg.AddContent(MimeType.Html, email.Body);
            var apiKey = _configuration[_apiKeyConfiguration];
            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }

        private string CreateMessageBody(string name, string otp, string url, string[] configSection)
        {
            var mailBody = new StringBuilder(string.Join("", configSection));
            mailBody.Replace("lg;", "<").Replace("gt;", ">");
            mailBody.Replace("#_name", name);
            if (!string.IsNullOrWhiteSpace(otp))
                mailBody.Replace("#_otp", otp);
            if (!string.IsNullOrWhiteSpace(url))
                mailBody.Replace("#_url", url);
            return mailBody.ToString();
        }
        private string CreateContactVerificationMessageBody(string name, string url)
        {
            return CreateMessageBody(name, string.Empty, url, _configuration.GetSection("EmailVerificationMessageTemplate").Get<string[]>());
        }
        private string CreateTwoFAMessageBody(string name, string otp)
        {
            return CreateMessageBody(name, otp, string.Empty, _configuration.GetSection("TwoFAAuthenticationMessageTemplate").Get<string[]>());
        }
        private string CreateDeviceVerificationMessageBody(string name, string url)
        {
            return CreateMessageBody(name, string.Empty, url, _configuration.GetSection("DeviceVerificationMessageTemplate").Get<string[]>());
        }
        private string CreateForgotPasswordMessageBody(string name, string url)
        {
            return CreateMessageBody(name, string.Empty, url, _configuration.GetSection("ForgotPasswordMessageTemplate").Get<string[]>());
        }
        public async Task SendTwoFAEmailMessageAsync(string receiver, string name, string otp)
        {
            EmailRequest emailRequest = new EmailRequest
            {
                EmailTo = receiver,
                NameTo = name,
                Subject = _twoFaMailSubject,
                Body = CreateTwoFAMessageBody(name, otp)
            };
            if (!await TrySendEmail(emailRequest))
            {
                throw new Exception($"TwoFA Email request failed for receiver: {receiver}");
            }
        }
        public async Task SendDeviceVerificationMessageAsync(string receiver, string name, string url)
        {
            EmailRequest emailRequest = new EmailRequest
            {
                EmailTo = receiver,
                NameTo = name,
                Subject = _deviceVerificationMailSubject,
                Body = CreateDeviceVerificationMessageBody(name, url)
            };
            if (!await TrySendEmail(emailRequest))
            {
                throw new Exception($"Device verification message Email request failed for receiver: {receiver}");
            }
        }
        public async Task SendContactVerificationMessageAsync(string receiver, string url)
        {
            EmailRequest emailRequest = new EmailRequest
            {
                EmailTo = receiver,
                Subject = _emailVerificationMailSubject,
                Body = CreateContactVerificationMessageBody("", url).Replace(_baseApiUrl, _configuration["BaseApiUrl"])
            };
            if (!await TrySendEmail(emailRequest))
            {
                throw new Exception($"Contact verification message Email request failed for receiver: {receiver}");
            }
        }
        public async Task SendForgotPasswordEmailAsync(string receiver, string name, string url)
        {
            EmailRequest emailRequest = new EmailRequest
            {
                EmailTo = receiver,
                NameTo = name,
                Subject = _forgetPasswordMailSubject,
                Body = CreateForgotPasswordMessageBody(name, url)
            };
            if (!await TrySendEmail(emailRequest))
            {
                throw new Exception($"Forgot password Email request failed for receiver: {receiver}");
            }
        }
    }
}
