using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Data.Enum;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using OnSolve.Mobile.Web.Swn402Entities;
using SessionManager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services
{
    public class PasswordService : IPasswordService
    {

        readonly Func<OnSolveMobileContext> _dbContextProvider;
        readonly IConfiguration _configuration;
        readonly IEmailService _emailService;
        readonly IPasswordHasherService _passwordHasherService;
        readonly IVerificationCodeGeneration _verificationCodeGeneration;
        readonly ILogger<PasswordService> _logger;
        readonly IUserService _userService;
        readonly IMobileUserService _mobileUserService;
        readonly SessionManagerSoap _sessionManagerClient;
        private const int defaultExpiryTime = 3;

        public PasswordService(IConfiguration configuration,
            Func<OnSolveMobileContext> dbContextProvider,
            IEmailService emailService,
            IPasswordHasherService passwordHasherService,
            IVerificationCodeGeneration verificationCodeGeneration,
            ILogger<PasswordService> logger,
            IUserService userService,
            IMobileUserService mobileUserService,
            SessionManagerSoap sessionManagerClient
            )
        {
            _dbContextProvider = dbContextProvider;
            _configuration = configuration;
            _emailService = emailService;
            _passwordHasherService = passwordHasherService;
            _verificationCodeGeneration = verificationCodeGeneration;
            _logger = logger;
            _userService = userService;
            _mobileUserService = mobileUserService;
            _sessionManagerClient = sessionManagerClient;
        }

        private async Task SaveResetPasswordCode(string resetCode, int mobileUserId)
        {
            using (var dbContext = _dbContextProvider())
            {
                var resetCodeInfo = await dbContext.ResetPasswordCode.Include(x => x.MobileUser)
                                          .FirstOrDefaultAsync(dt => dt.MobileUser.Id == mobileUserId);
                if (resetCodeInfo != null)
                {
                    resetCodeInfo.ResetCode = resetCode;
                    resetCodeInfo.CreationTime = DateTime.UtcNow;
                }
                else
                {
                    var mobileUser = await dbContext.MobileUser.Where(x => x.Id == mobileUserId).FirstOrDefaultAsync();
                    var newResetPasswordCode = new ResetPasswordCode
                    {
                        ResetCode = resetCode,
                        CreationTime = DateTime.UtcNow,
                        MobileUser = mobileUser

                    };
                    dbContext.ResetPasswordCode.Add(newResetPasswordCode);
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task RequestPasswordResetCode(ContactDetail contactDetail)
        {
            MobileUser mobileUser = await _userService.GetMobileUser(contactDetail.EmailId);
            var code = await GetResetCode(contactDetail.EmailId);
            if (String.IsNullOrEmpty(code))
            {
                code = _verificationCodeGeneration.GenerateResetCode();
                await SaveResetPasswordCode(code, mobileUser.Id);
            }
            await SendPasswordResetLink(contactDetail.EmailId, $"{contactDetail.FirstName} {contactDetail.LastName}", code);
        }
        private async Task<string> GetResetCode(string username)
        {
            var expiryConfig = _configuration.GetValue<double>("PasswordResetCodeTimeout", defaultExpiryTime);
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.ResetPasswordCode
                                                 .Include(x => x.MobileUser)
                                                 .Where(x => x.MobileUser.Username == username && DateTime.UtcNow < x.CreationTime.AddHours(expiryConfig))
                                                 .Select(x => x.ResetCode)
                                                 .FirstOrDefaultAsync();
            }
        }
        private async Task SendPasswordResetLink(string email, string name, string resetCode)
        {
            var url = $"{_configuration["BaseApiUrl"]}/api/user/openappforpasswordreset?resetcode={resetCode}";
            await _emailService.SendForgotPasswordEmailAsync(email, name, url);
            _logger.LogInformation($"PasswordRestLink to user {name} email address : {email} is : successful");
        }
        public async Task<ResetPasswordCode> GetResetCodeInfo(string resetcode)
        {
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.ResetPasswordCode.Include(x => x.MobileUser)
                                            .FirstOrDefaultAsync(x => x.ResetCode == resetcode);
            }
        }
        public async Task ResetUserPassword(MobileUser mobileUser, string password)
        {
            await UpdateENSUserPassword(mobileUser, password);
            await UpdateResetCode(mobileUser);
        }
        private async Task UpdateENSUserPassword(MobileUser user, string password)
        {
            var pwdRequest = new SetMemberUserPasswordRequest() { Username = user.Username, Password = password };
            var ensUserPwdChangeResult = await _sessionManagerClient.SetMemberUserPasswordAsync(pwdRequest);
            if (!ensUserPwdChangeResult.IsSuccess)
            {
                throw new Exception($"ENS User password update failed for {user.Username}");
            }
        }
        public async Task UpdateResetCode(MobileUser mobileUser)
        {
            using (var dbContext = _dbContextProvider())
            {
                var pwdResetCode = await dbContext.ResetPasswordCode.Include(x => x.MobileUser)
                    .FirstOrDefaultAsync(x => x.MobileUser.Id == mobileUser.Id);
                var expiryConfig = _configuration.GetValue<double>("PasswordResetCodeTimeout", defaultExpiryTime);
                pwdResetCode.CreationTime = pwdResetCode.CreationTime.AddHours(-expiryConfig);        
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task DeleteResetCode(MobileUser mobileUser)
        {
            using (var dbContext = _dbContextProvider())
            {
                var pwdResetCode = await dbContext.ResetPasswordCode.Include(x => x.MobileUser)
                    .FirstOrDefaultAsync(x => x.MobileUser.Id == mobileUser.Id);
                dbContext.ResetPasswordCode.Remove(pwdResetCode);
                await dbContext.SaveChangesAsync();
            }
        }
        public bool HasResetCodeExpired(ResetPasswordCode resetCode)
        {
            var expiryConfig = _configuration.GetValue<double>("PasswordResetCodeTimeout", defaultExpiryTime);
            return resetCode.CreationTime.AddHours(expiryConfig) < DateTime.UtcNow;
        }

    }
}
