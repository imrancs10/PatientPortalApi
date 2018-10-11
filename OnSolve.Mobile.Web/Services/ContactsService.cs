using AsyncPoco;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using OnSolve.Mobile.Web.Swn402Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services
{
    public class ContactsService : IContactsService
    {
        private readonly Func<OnSolveMobileContext> _dbContextProvider;
        private readonly IDatabase _swn402Db;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IVerificationCodeGeneration _verificationService;
        private const int defaultExpiryTime = 3;

        public ContactsService(IDatabase swn402Db, IEmailService emailService, IConfiguration configuration
            , IVerificationCodeGeneration verificationService, Func<OnSolveMobileContext> dbContextProvider)
        {
            _swn402Db = swn402Db;
            _emailService = emailService;
            _configuration = configuration;
            _verificationService = verificationService;
            _dbContextProvider = dbContextProvider;
        }

        public async Task<List<ContactDetail>> GetContactList(string email)
        {
            return await GetContactListByEmail(email);
        }

        public async Task RequestMail(string emailId)
        {
            var code = await GetEmailVeificationCodeByEmail(emailId);
            if (String.IsNullOrEmpty(code))
            {
                code = _verificationService.GenerateRandomString();
                await SaveEmailVerificationCode(emailId, code);
            }
            var url = $"{_configuration["BaseApiUrl"]}/api/contact/openappforemailverification?code={code}";
            await _emailService.SendContactVerificationMessageAsync(emailId, url);
        }

        private async Task<string> GetEmailVeificationCodeByEmail(string emailId)
        {
            var expiryConfig = _configuration["EmailVerificationCodeTimeout"] != null ?
                                Convert.ToInt32(_configuration["EmailVerificationCodeTimeout"]) : defaultExpiryTime;
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.EmailVerificationCode
                                                 .Where(x => emailId == x.Email && DateTime.UtcNow < x.CreatedDateTime.AddHours(expiryConfig))
                                                 .Select(x => x.Code)
                                                 .FirstOrDefaultAsync();
            }
        }

        public async Task<EmailVerificationCode> GetEmailVeificationInfoByCode(string code)
        {

            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.EmailVerificationCode
                                                 .Where(x => code == x.Code)
                                                 .FirstOrDefaultAsync();
            }
        }

        public bool HasVerificationCodeExpired(EmailVerificationCode verificationCode)
        {
            var expiryConfig = _configuration.GetValue<double>("EmailVerificationCodeTimeout", defaultExpiryTime);
            return verificationCode.CreatedDateTime.AddHours(expiryConfig) < DateTime.UtcNow;
        }

        public async Task DeleteEmailVerificationCode(string email)
        {
            using (var dbContext = _dbContextProvider())
            {
                var mappings = dbContext.EmailVerificationCode.Where(b => b.Email == email);
                await mappings.ForEachAsync(x => dbContext.EmailVerificationCode.Remove(x));
                await dbContext.SaveChangesAsync();
            }
        }

        private async Task SaveEmailVerificationCode(string email, string code)
        {
            using (var dbContext = _dbContextProvider())
            {
                var verificationCodeInfo = await dbContext.EmailVerificationCode
                                          .FirstOrDefaultAsync(dt => dt.Email == email);
                if (verificationCodeInfo != null)
                {
                    verificationCodeInfo.Code = code;
                    verificationCodeInfo.CreatedDateTime = DateTime.UtcNow;
                }
                else
                {
                    var newVerificationCode = new EmailVerificationCode
                    {
                        Code = code,
                        Email = email,
                        CreatedDateTime = DateTime.UtcNow
                    };
                    dbContext.EmailVerificationCode.Add(newVerificationCode);
                }
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<ContactDetail>> GetENSContacts(string email)
        {
            return await GetENSContactsByEmail(email);
        }

        public async Task<bool> VerifyRecipientAccount(long recipientId, int accountId)
        {
            var sqlQuery = Sql.Builder.Select("Count(1)")
                                            .Append(GetBaseQueryForContacts())
                                            .Append("AND r.recipient_id = @0", recipientId)
                                            .Append("AND ua.account_id = @0", accountId);

            var result = await _swn402Db.ExecuteScalarAsync<int>(sqlQuery);
            return result == 1;
        }

        public async Task<bool> IsValidENSUser(long ensUserId, long recipientId)
        {
            var sqlQuery = Sql.Builder.Select("Count(1)")
                                            .Append(GetBaseQueryForENSContacts())
                                            .Append("AND UA.user_id = @0", ensUserId)
                                            .Append("AND R.recipient_id = @0", recipientId);

            var result = await _swn402Db.ExecuteScalarAsync<int>(sqlQuery);
            return result == 1;
        }

        private async Task<List<ContactDetail>> GetContactListByEmail(string email)
        {
            var fetchContactsQuery = Sql.Builder.Select("r.recipient_id, r.first_name, r.last_name, r.middle_name, ua.account_id, m.account_name, r.ClientsUniqueRecipientID")
                                            .Append(GetBaseQueryForContacts())
                                            .Append("AND cp.address = @0", email);

            return await _swn402Db.FetchAsync<ContactDetail>(fetchContactsQuery);
        }

        public async Task<ContactDetail> GetContactPointDetail(long recipientId)
        {
            var fetchContactsQuery = Sql.Builder.Select("r.recipient_id, r.first_name, r.last_name, r.middle_name, " +
                "ua.account_id, m.account_name,cp.address as email, m.member_id, r.ClientsUniqueRecipientID")
                                            .Append(GetBaseQueryForContacts())
                                            .Append("AND r.recipient_id = @0", recipientId);

            return await _swn402Db.FirstOrDefaultAsync<ContactDetail>(fetchContactsQuery);
        }

        private async Task<List<ContactDetail>> GetENSContactsByEmail(string email)
        {
            var fetchContactsQuery = Sql.Builder.Select("R.recipient_id,R.first_name, R.last_name,  R.middle_name,UA.account_id , M.account_name, UA.username,UA.user_id, R.ClientsUniqueRecipientID")
                                            .Append(GetBaseQueryForENSContacts())
                                            .Append("AND UA.email = @0", email);

            return await _swn402Db.FetchAsync<ContactDetail>(fetchContactsQuery);
        }

        public async Task<ContactDetail> GetENSUserByUsername(string username)
        {
            var fetchENSUsersQuery = Sql.Builder.Select("UA.user_id,UA.username, UR.recipient_id,UA.account_id , UA.username, UA.email, " +
                                                        "UA.member_id,UA.first_name,UA.last_name,UA.locked_out,UA.phone")
                                            .Append(GetBaseQueryForENSContacts())
                                            .Append("AND UA.username = @0", username);
            return await _swn402Db.FirstOrDefaultAsync<ContactDetail>(fetchENSUsersQuery);
        }

        public async Task<ContactDetail> GetENSUserByUserId(long userId)
        {
            var fetchENSUsersQuery = Sql.Builder.Select("UA.user_id,UA.username, UR.recipient_id,UA.account_id , UA.username, UA.email, " +
                                                        "UA.member_id,UA.first_name,UA.last_name,UA.locked_out,UA.phone")
                                            .Append(GetBaseQueryForENSContacts())
                                            .Append("AND UA.user_id = @0", userId);
            return await _swn402Db.FirstOrDefaultAsync<ContactDetail>(fetchENSUsersQuery);
        }

        public async Task<bool> IsContactDuplicatedWithinAccount(string email)
        {
            var checkDuplicateContactsQuery = Sql.Builder.Select("Count(1)")
                                            .Append(GetBaseQueryForContacts())
                                            .Append("AND cp.address = @0", email)
                                            .GroupBy("ua.account_id, cp.address")
                                            .Append("HAVING Count(*) > 1");

            var duplicateContactsList = await _swn402Db.FetchAsync<int>(checkDuplicateContactsQuery);
            return duplicateContactsList.Any();
        }

        private string GetBaseQueryForContacts()
        {
            return @"
                FROM swn402..member_group_recipient AS MGR WITH(nolock)
                INNER JOIN swn402..recipient AS R WITH(nolock) ON MGR.recipient_id = R.recipient_id
                INNER JOIN swn402..recipient_contact_point rcp with(nolock) ON r.recipient_id = rcp.recipient_id
                INNER JOIN swn402..contact_point cp with(nolock) ON rcp.contact_point_id = cp.contact_point_id
                INNER JOIN swn402..member_group AS MG WITH(nolock) ON MGR.member_group_id = MG.member_group_id
                INNER JOIN swn402..user_Access AS UAC WITH(nolock) ON MG.member_group_id = UAC.object_id
                INNER JOIN swn402..user_authenticate ua WITH(nolock) ON uac.user_id = ua.user_id
                INNER JOIN swn402..cfg_acct_Settings cas WITH(nolock) ON cas.member_id = ua.member_id AND cas.char_value = cp.label
                INNER JOIN swn402..member m WITH(nolock) ON m.account_id = ua.account_id
                WHERE r.status_id = 1
                           AND MG.status_id = 1
                           AND MG.type = 1
                           AND ua.user_status = 1
                           AND ua.role_id = 1
                           AND cas.key_value = 'SWNDIRECT_CONTACT_POINT'
            ";
        }

        private string GetBaseQueryForENSContacts()
        {
            return @"
                FROM swn402..user_Authenticate as UA WITH(nolock)
                INNER JOIN swn402..user_Recipient as UR WITH(nolock) on UA.user_id = UR.user_id
                INNER JOIN swn402..recipient as R WITH(nolock) on UR.recipient_id = R.recipient_id 
                INNER JOIN swn402..cfg_acct_Settings CAS WITH(nolock) ON CAS.member_id = UA.member_id
                INNER JOIN swn402..member M WITH(nolock) ON M.account_id = UA.account_id
                WHERE R.status_id = 1 
                  AND UA.user_status = 1
                  AND UA.enabled = 1
                  AND CAS.key_value = 'SWNDIRECT_CONTACT_POINT'
            ";
        }
    }
}
