using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using SessionManager;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services.Login
{
    /// <summary>
    /// Summary description for PasswordServiceTest
    /// </summary>
    [TestClass]
    public class PasswordServiceTest
    {
        Func<OnSolveMobileContext> _dbContextProvider;
        Mock<IConfiguration> _configuration;
        readonly Mock<IEmailService> _emailService;
        readonly Mock<IPasswordHasherService> _passwordHasherService;
        readonly VerificationCodeGeneration _verificationCodeGeneration;
        readonly Mock<ILogger<PasswordService>> _logger;
        Mock<IUserService> _userService;
        Mock<IMobileUserService> _mobileUserService;

        readonly Mock<SessionManagerSoap> _sessionManagerClient;
        PasswordService _passwordService;
        public PasswordServiceTest()
        {
            _configuration = new Mock<IConfiguration>();
            _emailService = new Mock<IEmailService>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _verificationCodeGeneration = new VerificationCodeGeneration(_configuration.Object);
            _logger = new Mock<ILogger<PasswordService>>();
            _userService = new Mock<IUserService>();
            _mobileUserService = new Mock<IMobileUserService>();
            _sessionManagerClient = new Mock<SessionManagerSoap>();

        }

        private void ArrangeElements()
        {
            _dbContextProvider = GetContextWithData;
            _emailService.Setup(x => x.SendForgotPasswordEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _emailService.Setup(x => x.SendForgotPasswordEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _passwordService = new PasswordService(
                _configuration.Object,
                _dbContextProvider,
                _emailService.Object,
                _passwordHasherService.Object,
                _verificationCodeGeneration,
                _logger.Object,
                _userService.Object,
                _mobileUserService.Object,
                _sessionManagerClient.Object
                );

        }

        private OnSolveMobileContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            //var device =
            //  new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "123456" };


            //var resetCode =
            //    new ResetPasswordCode { Id = 10, ResetCode = "123456", AuthorizedDevice = device, CreationTime = DateTime.Now };

            //dbContext.AuthorizedDevices.Add(device);
            //dbContext.ResetPasswordCode.Add(resetCode);
            dbContext.SaveChanges();

            return dbContext;
        }


        [TestMethod]
        public async Task CreatePassword_TestIfReseponseIsInvalidUser()
        {
            //Arrange
            ArrangeElements();

            //Act
            //var result = await _passwordService.IsResetCodeValid("nidhi@onsolve.com", "nidhi", "123456", "123456");

            ////Assert
            //result.Should().BeFalse();

        }

        [TestMethod]
        public async Task Put_ReturnsHTTPNotFound_ForInvalidUser()
        {
            var _mockPasswordService = new Mock<IPasswordService>();
            _userService = new Mock<IUserService>();
            _mobileUserService = new Mock<IMobileUserService>();
            _configuration = new Mock<IConfiguration>();

            var changePasswordModel = new ChangePasswordModel()
            {
                OldPassword = "test123",
                NewPassword = "Passw0rd",
                ConfirmPassword = "Passw0rd"
            };
            var controller = new PasswordController(_mockPasswordService.Object, _configuration.Object, _mobileUserService.Object, _userService.Object, _passwordHasherService.Object);
            var result = await controller.Put("testUser", changePasswordModel);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public void ResetUserPassword_TestForAnENSUser()
        {
            var _mockPasswordService = new Mock<IPasswordService>();
            string deviceToken = "123456";
            string password = "fakePassword";
            MobileUser testMobileUser = new MobileUser
            {
                ENSUserId = 1,
                Salt = "abcd"
            };

            //Assert.ThrowsException<Exception>(async () => await _mockPasswordService.Object
            //    .ResetUserPassword(testMobileUser, password, deviceToken));
        }

        [TestMethod]
        public async Task ResetUserPassword_TestForANonENSUser()
        {
            //Arrange
            ArrangeElements();

            //Act
            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            string deviceToken = "123456";
            string password = "fakePassword";
            MobileUser testMobileUser = new MobileUser
            {
                Salt = "abcd"

            };
            //await _passwordService.ResetUserPassword(testMobileUser, password, deviceToken);

            //Assert
            //res.Should().BeTrue(); // Todo
        }

        private OnSolveMobileContext GetContextWithDataRetries()
        {
            var options = new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var dbContext = new OnSolveMobileContext(options);

            //var device =
            //    new AuthorizedDevice { Id = 1, DeviceIdentifier = "123456", DeviceToken = "123456" };


            //var resetCode =
            //    new ResetPasswordCode { Id = 10, ResetCode = "123456", AuthorizedDevice = device, CreationTime = DateTime.Now };

            //dbContext.AuthorizedDevices.Add(device);
            //dbContext.ResetPasswordCode.Add(resetCode);
            dbContext.SaveChanges();

            return dbContext;
        }

        private void ArrangeElementsRetries()
        {
            _dbContextProvider = GetContextWithDataRetries;

            _emailService.Setup(x => x.SendForgotPasswordEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            _passwordService = new PasswordService(_configuration.Object, _dbContextProvider
                , _emailService.Object, _passwordHasherService.Object
                , _verificationCodeGeneration, _logger.Object,
                _userService.Object,
                _mobileUserService.Object,
                _sessionManagerClient.Object);
        }

    }
}
