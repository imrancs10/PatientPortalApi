using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Services;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services
{
    [TestClass]
    public class FCMTokenServiceTest
    {
        private Func<OnSolveMobileContext> _dbContextProvider;
        private FCMTokenService fCMTokenService;
        private const string fcmToken = "ASDFGHwwa23456gdf";


        [TestInitialize]
        public void Arrange()
        {
            _dbContextProvider = GetContextWithData;
            fCMTokenService = new FCMTokenService(_dbContextProvider);

        }

        [TestMethod]
        public async Task FCMTokenService_VerifyPostFCMToken()
        {
            //string fcmToken = "eWy4akW_hDM:APA91bFPv5tx8l04xb-5th_rfFjdpV2bEaN9MYBT-u3wJGppyh5-MVCwevFWh5OGnJ3eUdeQDi5ztDmQ4mIpMYOM_E2GWvZlECv4cnpVeDVNl15mkJQT3jgDCmIjYrRNEBdF6Yzg1xN1";
            //await fCMTokenService.UpdateFCMToken("TestDeviceToken", fcmToken);
        }

        [TestMethod]
        public async Task FCMTokenService_VerifyDeleteFCMToken()
        {
            await fCMTokenService.DeleteFCMTokenInfo(10,"TestDeviceToken");
        }

        private OnSolveMobileContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            var mobileUser = new MobileUser { Username = "nidhi@nagarro.com", Id = 1, RecipientId = 1, AccountId = 1, Password = string.Empty, Salt = "test" };

            dbContext.MobileUser.Add(mobileUser);

            //var device =
            //  new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "TestDeviceToken", MobileUser = mobileUser,FCMToken = fcmToken };
           
            //dbContext.AuthorizedDevices.Add(device);   

            dbContext.SaveChanges();

            return dbContext;
        }
    }
}