using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services
{
    /// <summary>
    /// Summary description for MobileUserTestService
    /// </summary>
    [TestClass]
    public class MobileUserServiceTest
    {
        private MobileUserService _mobileService;
        private Func<OnSolveMobileContext> _dbContextProvider;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<IUserService> _userService;

        [TestInitialize]
        public void Arrange()
        {
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _userService = new Mock<IUserService>();
        }

        [TestMethod]
        public async Task IsPasswordNotRequired_ReturnsResultAsync()
        {
            _dbContextProvider = GetContextWithData;
            _mobileService
                = new MobileUserService(_dbContextProvider, _passwordHasherService.Object, _userService.Object);
            var newMobileUser = new MobileUser
            {
                Username = "nidhi@nagarro.com",
                Id = 1,
                RecipientId = 1,
                AccountId = 1,
                ENSUserId = 1,
                CreatedOn = DateTime.Now
            };
            await _mobileService.SetPassword(newMobileUser, "TestPassword");

            //response.Should().BeFalse();
        }

        [TestMethod]
        public async Task IsENSUser_ReturnsResultAsync()
        {
            Arrange();

            var hash = "pASS5%*%4672134FHHWG8878";
            _dbContextProvider = GetContextWithDataENSUserNotNull;
            _passwordHasherService.Setup(x => x.GetPasswordHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(hash);
            _mobileService
                = new MobileUserService(_dbContextProvider, _passwordHasherService.Object, _userService.Object);
            var newMobileUser = new MobileUser
            {
                Username = "nidhi@nagarro.com",
                Id = 1,
                RecipientId = 1,
                AccountId = 1,
                ENSUserId = 1,
                CreatedOn = DateTime.Now
            };
            await _mobileService.SetPassword(newMobileUser, "TestPassword");

            //response.Should().BeFalse();
        }

        [TestMethod]
        public async Task UserAlreadyHasPassword_ReturnsResultAsync()
        {
            _dbContextProvider = GetContextWithDataAlreadyHasPassword;
            _mobileService
                = new MobileUserService(_dbContextProvider, _passwordHasherService.Object, _userService.Object);
            var newMobileUser = new MobileUser
            {
                Username = "nidhi@nagarro.com",
                Id = 1,
                RecipientId = 1,
                AccountId = 1,
                ENSUserId = 1,
                CreatedOn = DateTime.Now
            };
            await _mobileService.SetPassword(newMobileUser, "TestPassword");

            //response.Should().BeFalse();
        }

        private OnSolveMobileContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            var mobileUser = new MobileUser { Username = "nidhi@nagarro.com", Id = 1, RecipientId = 1, AccountId = 1,
                Password = string.Empty, Salt = "test", CreatedOn = DateTime.Now };

            dbContext.MobileUser.Add(mobileUser);

            var device =
              new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "TestDeviceToken", MobileUser = mobileUser };

            dbContext.AuthorizedDevices.Add(device);

            dbContext.SaveChanges();

            return dbContext;
        }

        private OnSolveMobileContext GetContextWithDataAlreadyHasPassword()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            var mobileUser = new MobileUser { Username = "nidhi@nagarro.com", Id = 1, RecipientId = 1, AccountId = 1,
                Password = "test", Salt = "test", CreatedOn = DateTime.Now
            };

            dbContext.MobileUser.Add(mobileUser);

            var device =
              new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "TestDeviceToken", MobileUser = mobileUser };

            dbContext.AuthorizedDevices.Add(device);

            dbContext.SaveChanges();

            return dbContext;
        }

        private OnSolveMobileContext GetContextWithDataENSUserNotNull()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            var mobileUser = new MobileUser { Username = "nidhi@nagarro.com", Id = 1, RecipientId = 1, AccountId = 1,
                Password = "test", Salt = "test", ENSUserId = 1, CreatedOn = DateTime.Now
            };

            dbContext.MobileUser.Add(mobileUser);

            var device =
              new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "TestDeviceToken", MobileUser = mobileUser };

            dbContext.AuthorizedDevices.Add(device);

            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
