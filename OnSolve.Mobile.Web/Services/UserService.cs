using AsyncPoco;
using Microsoft.EntityFrameworkCore;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
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
    public class UserService : IUserService
    {
        private readonly IDatabase _swn402Db;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly Func<OnSolveMobileContext> _dbContextProvider;
        private readonly IContactsService _contactsService;
        private const int ContactType_ExpressText = 1002;
        private const int ContactType_MobileText = 2002;
        private const int ContactType_MobileVoice = 2001;
        private const string ContactTypeLabel_ExpressText = "Express Messenger";
        private const string ContactTypeLabel_MobileText = "Mobile Text";
        private const string ContactTypeLabel_MobileVoice = "Mobile Voice";

        public UserService(IDatabase swn402Db, IPasswordHasherService passwordHasherService
            , Func<OnSolveMobileContext> dbContextProvider, IContactsService contactsService)
        {
            _swn402Db = swn402Db;
            _passwordHasherService = passwordHasherService;
            _dbContextProvider = dbContextProvider;
            _contactsService = contactsService;
        }

        public async Task<ContactDetail> GetMobileUserContactDetail(string userEmailId)
        {
            ContactDetail contactDetail = null;
            using (var dbContext = _dbContextProvider())
            {
                var mobileUser = await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Username == userEmailId);
                if (mobileUser != null)
                {
                    if (mobileUser.ENSUserId.HasValue)
                    {
                        contactDetail = await _contactsService.GetENSUserByUserId(mobileUser.ENSUserId.Value);
                    }
                    else
                    {
                        contactDetail = await _contactsService.GetContactPointDetail(mobileUser.RecipientId);
                    }
                }

            }
            return contactDetail;
        }

        public async Task<MobileUser> GetMobileUser(string username)
        {
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Username == username);
            }
        }

        public async Task<CreateUserResponse> CreateUser(UserRequest userRequest)
        {
            var response = new CreateUserResponse();
            using (var dbContext = _dbContextProvider())
            {
                var mobileUser = new MobileUser()
                {
                    AccountId = userRequest.AccountId,
                    RecipientId = userRequest.RecipientId,
                    Username = userRequest.EmailAddress,
                    ENSUserId = userRequest.ENSUserId,
                    CreatedOn = DateTime.Now
                };

                mobileUser.Salt = _passwordHasherService.GetSalt();
                mobileUser.Password = _passwordHasherService.GetPasswordHash(userRequest.Password, mobileUser.Salt);

                dbContext.MobileUser.Add(mobileUser);
                await dbContext.SaveChangesAsync();
                response.CreatedAtId = mobileUser.Id;
                await CreateContactPoints(userRequest);
            }
            return response;
        }

        private async Task CreateContactPoints(UserRequest userRequest)
        {
            await CreateContactPoint(userRequest.RecipientId, userRequest.EmailAddress, ContactType_ExpressText, ContactTypeLabel_ExpressText);
            await CreateContactPoint(userRequest.RecipientId, userRequest.EmailAddress, ContactType_MobileText, ContactTypeLabel_MobileText);
            await CreateContactPoint(userRequest.RecipientId, userRequest.EmailAddress, ContactType_MobileVoice, ContactTypeLabel_MobileVoice);
        }

        public async Task<ENSLoginResponse> GetENSUserDetails(string username)
        {
            var ensUser = await _contactsService.GetENSUserByUsername(username);
            if (ensUser != null)
            {
                return new ENSLoginResponse
                {
                    AccountId = ensUser.AccountId,
                    Email = ensUser.EmailId,
                    RecipientId = ensUser.RecipientId,
                    Username = ensUser.ENSUsername,
                    ENSUserId = ensUser.ENSUserId.Value
                };
            }
            return null;
        }

        public async Task<bool> AreRegistrationDetailsValid(UserRequest userRequest)
        {
            using (var dbContext = _dbContextProvider())
            {
                if (await IsExistingRecipient(userRequest.RecipientId, dbContext))
                {
                    return false;
                }
                else if (await IsExistingEmail(userRequest.EmailAddress, dbContext))
                {
                    return false;
                }
                else if (!await _contactsService.VerifyRecipientAccount(userRequest.RecipientId, userRequest.AccountId))
                {
                    return false;
                }
                else if (userRequest.ENSUserId.HasValue && !await _contactsService.IsValidENSUser(userRequest.ENSUserId.Value, userRequest.RecipientId))
                {
                    return false;
                }
                return true;
            }
        }

        private async Task<bool> IsExistingRecipient(long recipientId, OnSolveMobileContext dbContext)
        {
            return await dbContext.MobileUser
                            .AnyAsync(x => x.RecipientId == recipientId && !x.ENSUserId.HasValue);
        }

        private async Task<bool> IsExistingEmail(string email, OnSolveMobileContext dbContext)
        {
            return await dbContext.MobileUser
                         .AnyAsync(x => x.Username == email);
        }
        public async Task<List<int>> GetMobileUserIdList(long recipientId)
        {
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.MobileUser
                                                 .Where(x => recipientId == x.RecipientId)
                                                 .Select(x => x.Id)
                                                 .ToListAsync();
            }
        }

        public async Task<AccountDetails> GetAccountDetails(int accountId)
        {
            var sqlQuery = Sql.Builder.Select("m.account_name,m.company")
                                            .Append("from swn402..member as m where m.account_id = @0", accountId);

            return await _swn402Db.FirstAsync<AccountDetails>(sqlQuery);
        }
        private async Task CreateContactPoint(long recipientId, string emailId, int contactPointTypeId,
            string contactPointTypeLabel)
        {
            if (!await CheckIfContactPointExists(recipientId, emailId, contactPointTypeId))
            {
                await AddContactPoint(recipientId, emailId, contactPointTypeId, contactPointTypeLabel);
            }
        }

        private async Task<bool> CheckIfContactPointExists(long recipientId, string email, int contactPointTypeId)
        {
            var checkExpressContactsQuery = Sql.Builder.Select("CP.contact_point_id")
                                            .Append("FROM swn402..contact_point as CP")
                                            .InnerJoin("swn402..recipient_contact_point RCP")
                                            .On("CP.contact_point_id = RCP.contact_point_id")
                                            .Append("WHERE CP.type_id = @0 AND CP.address = @1 AND RCP.recipient_id = @2", contactPointTypeId, email, recipientId);

            var expressContact = await _swn402Db.FirstOrDefaultAsync<int>(checkExpressContactsQuery);

            return expressContact > 0;
        }

        private async Task AddContactPoint(long recipientId, string emailId, int contactPointTypeId, string contactPointTypeLabel)
        {
            var sqlQuery1 = Sql.Builder
                 .Append($@"INSERT INTO contact_point 
                 (type_id,label,address,modified_by,modified_date,country_id,country_code,sort_order,cascade_order,rule_profile_id,extension,importID,opting_status)
                 VALUES(@0, @1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12)"
                 , contactPointTypeId, contactPointTypeLabel, emailId, "SYSTEM UI", DateTime.Now, null, null, null, null, null, null, null, "IN");

            await _swn402Db.ExecuteAsync(sqlQuery1);

            var fetchContactPointIDQuery = Sql.Builder.Select("Max(contact_point_id) FROM swn402..contact_point")
                                                  .Append("WHERE address = @0", emailId);

            var contactPointID = await _swn402Db.ExecuteScalarAsync<int>(fetchContactPointIDQuery);

            var sqlQuery2 = Sql.Builder
                 .Append($@"INSERT INTO recipient_contact_point
                (recipient_id, contact_point_id, status_id)
                 VALUES(@0,@1,@2)", recipientId, contactPointID, 1);

            await _swn402Db.ExecuteAsync(sqlQuery2);
        }

    }
}
