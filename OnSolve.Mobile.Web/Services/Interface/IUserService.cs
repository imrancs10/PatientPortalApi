using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Swn402Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services.Interface
{
    public interface IUserService
    {
        Task<MobileUser> GetMobileUser(string username);
        Task<ContactDetail> GetMobileUserContactDetail(string username);
        Task<CreateUserResponse> CreateUser(UserRequest userRequest);
        Task<bool> AreRegistrationDetailsValid(UserRequest userRequest);
        Task<List<int>> GetMobileUserIdList(long recipientId);
        Task<ENSLoginResponse> GetENSUserDetails(string username);
        Task<AccountDetails> GetAccountDetails(int accountId);
    }
}