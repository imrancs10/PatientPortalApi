using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Swn402Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services.Interface
{
    public interface IContactsService
    {
        Task<List<ContactDetail>> GetContactList(string email);
        Task<List<ContactDetail>> GetENSContacts(string email);
        Task<bool> VerifyRecipientAccount(long recipientId, int accountId);
        Task<bool> IsContactDuplicatedWithinAccount(string email);
        Task RequestMail(string emailId);
        Task<bool> IsValidENSUser(long ensUserId, long recipientId);
        Task<ContactDetail> GetENSUserByUsername(string username);
        Task<ContactDetail> GetENSUserByUserId(long userId);
        Task<EmailVerificationCode> GetEmailVeificationInfoByCode(string code);
        Task DeleteEmailVerificationCode(string email);
        bool HasVerificationCodeExpired(EmailVerificationCode verificationCode);
        Task<ContactDetail> GetContactPointDetail(long recipientId);
    }
}
