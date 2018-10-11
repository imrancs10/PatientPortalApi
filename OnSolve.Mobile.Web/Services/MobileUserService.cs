using Microsoft.EntityFrameworkCore;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services
{
    public class MobileUserService : IMobileUserService
    {
        readonly Func<OnSolveMobileContext> _dbContextProvider;
        readonly IPasswordHasherService _passwordHasherService;

        public MobileUserService(Func<OnSolveMobileContext> dbContextProvider,
            IPasswordHasherService passwordHasherService)
        {
            _dbContextProvider = dbContextProvider;
            _passwordHasherService = passwordHasherService;
        }


        public async Task SetPassword(MobileUser mobileUser, string password)
        {
            mobileUser.Salt = _passwordHasherService.GetSalt();
            mobileUser.Password = _passwordHasherService.GetPasswordHash(password, mobileUser.Salt);
            using (var dbContext = _dbContextProvider())
            {
                dbContext.MobileUser.Update(mobileUser);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<MobileUser> GetMobileUser(string username)
        {
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Username == username);
            }
        }

        public async Task<MobileUser> GetMobileUser(int mobileUserId)
        {
            using (var dbContext = _dbContextProvider())
            {
                return await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Id == mobileUserId);
            }
        }

        public async Task ChangePassword(MobileUser mobileUser, ChangePasswordModel changePasswordModel)
        {
            await SetPassword(mobileUser, changePasswordModel.NewPassword);
        }
    }
}