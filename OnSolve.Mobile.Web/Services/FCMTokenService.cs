using Microsoft.EntityFrameworkCore;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Services
{
    public class FCMTokenService : IFCMTokenService
    {
        readonly Func<OnSolveMobileContext> _dbContextProvider;
        public FCMTokenService(Func<OnSolveMobileContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task PostFCMTokenInfo(int MobileUserId, string fcmToken)
        {
            using (var dbContext = _dbContextProvider())
            {                
                var fcmTokenInfo = await dbContext.FCMTokenInfo.Include(x => x.MobileUser)
                                                  .Where(x => x.MobileUser.Id == MobileUserId)
                                                  .FirstOrDefaultAsync(x => x.FCMToken == fcmToken);
                if (fcmTokenInfo == null)
                {
                    var mobileUser = await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Id == MobileUserId);
                    var newFCMTokenInfo = new FCMTokenInfo()
                    {
                        FCMToken = fcmToken,
                        MobileUser = mobileUser
                    };
                    dbContext.FCMTokenInfo.Add(newFCMTokenInfo);
                    await dbContext.SaveChangesAsync();
                }

                
            }
        }
       
        public async Task DeleteFCMTokenInfo(int MobileUserId, string fcmToken)
        {
            using (var dbContext = _dbContextProvider())
            {
                var fCMTokenInfo = await dbContext.FCMTokenInfo.Include(x => x.MobileUser)
                                                  .Where(x => x.MobileUser.Id == MobileUserId)
                                                  .FirstOrDefaultAsync(x => x.FCMToken == fcmToken);
                if (fCMTokenInfo != null)
                {
                    dbContext.FCMTokenInfo.Remove(fCMTokenInfo);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateFCMTokenInfo(int MobileUserId, FCMTokenUpdateModel fcmTokenUpdateModel)
        {
            using (var dbContext = _dbContextProvider())
            {
                var oldFCMTokenInfo = await dbContext.FCMTokenInfo.Include(x => x.MobileUser)
                                                  .Where(x => x.MobileUser.Id == MobileUserId)
                                                  .FirstOrDefaultAsync(x => x.FCMToken == fcmTokenUpdateModel.OldFCMToken);
                if (oldFCMTokenInfo != null)
                {
                    dbContext.FCMTokenInfo.Remove(oldFCMTokenInfo);
                }

                var fcmTokenInfo = await dbContext.FCMTokenInfo.Include(x => x.MobileUser)
                                                  .Where(x => x.MobileUser.Id == MobileUserId)
                                                  .FirstOrDefaultAsync(x => x.FCMToken == fcmTokenUpdateModel.FCMToken);
                if (fcmTokenInfo == null)
                {
                    var mobileUser = await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Id == MobileUserId);
                    var newFCMTokenInfo = new FCMTokenInfo()
                    {
                        FCMToken = fcmTokenUpdateModel.FCMToken,
                        MobileUser = mobileUser
                    };
                    dbContext.FCMTokenInfo.Add(newFCMTokenInfo);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
