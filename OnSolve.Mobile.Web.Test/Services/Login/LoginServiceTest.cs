using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using SessionManager;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services.Login
{
    [TestClass]
    public class JwtServiceTest
    {
        private Func<OnSolveMobileContext> _dbContextProvider;
        private Mock<IPasswordHasherService> _passwordHasherService;
        private Mock<SessionManagerSoap> _sessionManagerService;
        private Mock<IUserService> _userService;
        private Mock<IConfiguration> _configuration;
        private Mock<IFCMTokenService> _fcmTokenService;
        private Mock<IContactsService> _contactsService;

        [TestInitialize]
        public void ArrangeElements()
        {
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _sessionManagerService = new Mock<SessionManagerSoap>();
            _userService = new Mock<IUserService>();
            _configuration = new Mock<IConfiguration>();
            _fcmTokenService = new Mock<IFCMTokenService>();
            _contactsService = new Mock<IContactsService>();
        }

        [TestMethod]
        public void ValidateEnsUser_ShouldReturnsValidResult_WhenProvidedValidUserPassword()
        {
            _dbContextProvider = GetContextWithData;

            _sessionManagerService.Setup(x => x.CheckAuthenticationAsync(It.IsAny<SessionManager.LoginRequest>()))
                   .ReturnsAsync(true);
            var jwtService
               = new JwtService(_dbContextProvider, _passwordHasherService.Object, _sessionManagerService.Object, 
               _userService.Object, _fcmTokenService.Object, _configuration.Object, _contactsService.Object);

            var response = jwtService.ValidateEnsUser("test@nagarro.com", "TestPassword");

            response.Result.Should().BeTrue();
        }
        [TestMethod]
        public void ValidateEnsUser_ShouldReturnsErrorResult_WhenInvalidUserPassword()
        {
            _dbContextProvider = GetContextWithData;

            _sessionManagerService.Setup(x => x.CheckAuthenticationAsync(It.IsAny<SessionManager.LoginRequest>()))
                   .ReturnsAsync(false);
            var jwtService
               = new JwtService(_dbContextProvider, _passwordHasherService.Object, _sessionManagerService.Object, 
               _userService.Object, _fcmTokenService.Object, _configuration.Object,_contactsService.Object);

            var response = jwtService.ValidateEnsUser("test@nagarro.com", "wrong password");

            response.Result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateContactPointUser_ShouldReturnsValidResult_WhenProvidedValidUserPassword()
        {
            _dbContextProvider = GetContextWithData;

            _passwordHasherService.Setup(a => a.GetPasswordHash(It.IsAny<string>(), It.IsAny<string>())).Returns("test");

            var jwtService
               = new JwtService(_dbContextProvider, _passwordHasherService.Object, _sessionManagerService.Object, 
               _userService.Object, _fcmTokenService.Object, _configuration.Object, _contactsService.Object);
            var mobileUser = new MobileUser { Id = 1, Username = "test@nagarro.com", RecipientId = 1, AccountId = 1, Password = "test", Salt = "test", CreatedOn = DateTime.Now };

            var response = jwtService.ValidateMobileUser("TestPassword", mobileUser);

            response.Should().BeTrue();
        }
        [TestMethod]
        public async Task ValidateContactPointUser_ShouldReturnsErrorResult_WhenInvalidUserPassword()
        {
            _dbContextProvider = GetContextWithData;

            _passwordHasherService.Setup(a => a.GetPasswordHash(It.IsAny<string>(), It.IsAny<string>())).Returns("wrongpwd");

            var jwtService
               = new JwtService(_dbContextProvider, _passwordHasherService.Object, _sessionManagerService.Object, 
               _userService.Object, _fcmTokenService.Object, _configuration.Object, _contactsService.Object);
            var mobileUser = new MobileUser { Id = 1, Username = "test@nagarro.com", RecipientId = 1, AccountId = 1, Password = "wrongpwd", Salt = "test", CreatedOn = DateTime.Now };

            var response = jwtService.ValidateMobileUser("wrongpwd", mobileUser);

            response.Should().BeTrue();
        }

        private OnSolveMobileContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            var mobileUser = new MobileUser { Id = 1, RecipientId = 1, AccountId = 1, Password = "test", Salt = "test", CreatedOn = DateTime.Now };

            dbContext.MobileUser.Add(mobileUser);

            //var device =
            //  new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "TestDeviceToken", MobileUser = mobileUser };

            //dbContext.AuthorizedDevices.Add(device);

            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
