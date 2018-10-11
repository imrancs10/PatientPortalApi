using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using OnSolve.Mobile.Web.Swn402Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Controllers
{
    [TestClass]
    public class PasswordControllerTest
    {
        Mock<IConfiguration> _configuration;
        Mock<IMobileUserService> _mobileUserService;
        Mock<IUserService> _userService;
        Mock<IPasswordService> _passwordService;
        Mock<IPasswordHasherService> _passwordHasherService;
        private const string username = "sheik.imran@onsolve.com";
        private const string password = "Swn1234567";
        private const string newPassword = "onsolve123";
        private const string deviceToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        private const string resetCode = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        PasswordController passwordController;
        [TestInitialize]
        public void TestInitialize()
        {
            _configuration = new Mock<IConfiguration>();
            _mobileUserService = new Mock<IMobileUserService>();
            _userService = new Mock<IUserService>();
            _passwordService = new Mock<IPasswordService>();
            _passwordHasherService = new Mock<IPasswordHasherService>();

            passwordController = new PasswordController(_passwordService.Object,
                                                        _configuration.Object,
                                                        _mobileUserService.Object,
                                                        _userService.Object,
                                                        _passwordHasherService.Object);
        }

        [TestMethod]
        public void PasswordChangeModel_ReturnTrue_WhenValidateModelPass()
        {

            ChangePasswordModel changePasswordModel = new ChangePasswordModel
            {
                OldPassword = "abPa@sw1c",
                NewPassword = "Pas@1aabcd",
                ConfirmPassword = "Pas@1aabcd"
            };
            var context = new ValidationContext(changePasswordModel, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(changePasswordModel, context, result, true);
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void PasswordChangeModel_ReturnFalse_WhenValidateModelFail()
        {

            ChangePasswordModel changePasswordModel = new ChangePasswordModel
            {
                OldPassword = "abPa@sw1c",
                NewPassword = "Pas@1aabcd",
                ConfirmPassword = "Pasabcd"
            };
            var context = new ValidationContext(changePasswordModel, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(changePasswordModel, context, result, true);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task OpenAppForPasswordReset_OpensAndroidApp_ForAndroidUserAgent()
        {
            _configuration.Setup(x => x["ResetPasswordAndroidUrl"]).Returns("TestAndroidUrl");
            passwordController.ControllerContext.HttpContext = new DefaultHttpContext();
            passwordController.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "AndroidBrowser";

            ResetPasswordCode info = new ResetPasswordCode
            {
                Id = 1,
                ResetCode = resetCode,
                MobileUser = new MobileUser() { Id = 1, Username = username }
            };

            _passwordService.Setup(x => x.GetResetCodeInfo(It.IsAny<string>())).ReturnsAsync(info);

            var actualResult = await passwordController.Get("resetCode");

            actualResult.Should().BeOfType<RedirectResult>();
            var url = ((RedirectResult)actualResult).Url;
            url.Should().NotBeNullOrWhiteSpace();
            url.Should().StartWith("TestAndroidUrl");
        }

        [TestMethod]
        public async Task OpenAppForPasswordReset_ReturnsHttpBadRequest_WhenResetCodeNotProvided()
        {
            var actualResult = await passwordController.Get(string.Empty);

            Assert.IsInstanceOfType(actualResult, typeof(BadRequestResult));
        }
        [TestMethod]
        public async Task PasswordResetChallenge_ReturnsOK_WhenInputIsValid()
        {
            ContactDetail user = new ContactDetail()
            {
                RecipientId = 1,
                AccountId = 1,
                EmailId = "sheikh.imran@nagarro.com",
                Locked = false
            };

            _userService.Setup(x => x.GetMobileUserContactDetail(It.IsAny<string>())).ReturnsAsync(user);

            _passwordService.Setup(x => x.RequestPasswordResetCode(It.IsAny<ContactDetail>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Post(username);

            Assert.IsInstanceOfType(actualResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task PasswordResetChallenge_ReturnsForbid_WhenUserIsLocked()
        {
            ContactDetail user = new ContactDetail()
            {
                RecipientId = 1,
                AccountId = 1,
                EmailId = username,
                Locked = true
            };

            _userService.Setup(x => x.GetMobileUserContactDetail(It.IsAny<string>())).ReturnsAsync(user);

            _passwordService.Setup(x => x.RequestPasswordResetCode(It.IsAny<ContactDetail>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Post(username);

            Assert.IsInstanceOfType(actualResult, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task ResetUserpassword_ReturnsOK_WhenInputIsValid()
        {
            PasswordModel model = new PasswordModel()
            {
                Password = password
            };

            ResetPasswordCode info = new ResetPasswordCode
            {
                Id = 1,
                ResetCode = resetCode
            };

            _passwordService.Setup(x => x.GetResetCodeInfo(It.IsAny<string>())).ReturnsAsync(info);

            _passwordService.Setup(x => x.HasResetCodeExpired(It.IsAny<ResetPasswordCode>())).Returns(false);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _passwordService.Setup(x => x.UpdateResetCode(It.IsAny<MobileUser>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Create(resetCode, model);

            Assert.IsInstanceOfType(actualResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task ResetUserpassword_ReturnsBadRequest_WhenResetCodeIsNotValid()
        {
            PasswordModel model = new PasswordModel()
            {
                Password = password
            };

            ResetPasswordCode info = null;

            _passwordService.Setup(x => x.GetResetCodeInfo(It.IsAny<string>())).ReturnsAsync(info);

            _passwordService.Setup(x => x.HasResetCodeExpired(It.IsAny<ResetPasswordCode>())).Returns(false);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _passwordService.Setup(x => x.UpdateResetCode(It.IsAny<MobileUser>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Create(resetCode, model);

            Assert.IsInstanceOfType(actualResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task ResetUserpassword_ReturnsForbid_WhenResetCodeExpired()
        {
            PasswordModel model = new PasswordModel()
            {
                Password = password
            };

            ResetPasswordCode info = new ResetPasswordCode
            {
                Id = 1,
                ResetCode = resetCode,
                MobileUser = new MobileUser() { Username = username }
            };

            _passwordService.Setup(x => x.GetResetCodeInfo(It.IsAny<string>())).ReturnsAsync(info);

            _passwordService.Setup(x => x.HasResetCodeExpired(It.IsAny<ResetPasswordCode>())).Returns(true);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _passwordService.Setup(x => x.UpdateResetCode(It.IsAny<MobileUser>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Create(resetCode, model);

            Assert.AreEqual(403, ((ObjectResult)actualResult).StatusCode);
        }


        [TestMethod]
        public async Task ChangePassword_ReturnsOKResult_WhenInputIsValid()
        {
            MobileUser mobileUser = new MobileUser()
            {
                Id = 1,
                AccountId = 1,
                Password = password
            };

            ContactDetail user = new ContactDetail()
            {
                RecipientId = 1,
                AccountId = 1,
                Locked = false
            };

            ChangePasswordModel model = new ChangePasswordModel()
            {
                OldPassword = password,
                NewPassword = newPassword
            };

            _mobileUserService.Setup(x => x.GetMobileUser(It.IsAny<string>())).ReturnsAsync(mobileUser);

            _userService.Setup(x => x.GetMobileUserContactDetail(It.IsAny<string>())).ReturnsAsync(user);

            _passwordHasherService.Setup(x => x.IsValidPassword(It.IsAny<ChangePasswordModel>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Put(username, model);

            Assert.IsInstanceOfType(actualResult, typeof(OkResult));
        }

        [TestMethod]
        public async Task ChangePassword_ReturnsNotFound_WhenUserIsNotFound()
        {
            MobileUser mobileUser = null;

            ContactDetail user = new ContactDetail()
            {
                RecipientId = 1,
                AccountId = 1,
                Locked = false
            };

            ChangePasswordModel model = new ChangePasswordModel()
            {
                OldPassword = password,
                NewPassword = newPassword
            };

            _mobileUserService.Setup(x => x.GetMobileUser(It.IsAny<string>())).ReturnsAsync(mobileUser);

            _userService.Setup(x => x.GetMobileUserContactDetail(It.IsAny<string>())).ReturnsAsync(user);

            _passwordHasherService.Setup(x => x.IsValidPassword(It.IsAny<ChangePasswordModel>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Put(username, model);

            Assert.IsInstanceOfType(actualResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task ChangePassword_ReturnsForbidResult_WhenUserIsLocked()
        {
            MobileUser mobileUser = new MobileUser()
            {
                Id = 1,
                AccountId = 1,
                Password = password
            };

            ContactDetail user = new ContactDetail()
            {
                RecipientId = 1,
                AccountId = 1,
                Locked = true
            };

            ChangePasswordModel model = new ChangePasswordModel()
            {
                OldPassword = password,
                NewPassword = newPassword
            };

            _mobileUserService.Setup(x => x.GetMobileUser(It.IsAny<string>())).ReturnsAsync(mobileUser);

            _userService.Setup(x => x.GetMobileUserContactDetail(It.IsAny<string>())).ReturnsAsync(user);

            _passwordHasherService.Setup(x => x.IsValidPassword(It.IsAny<ChangePasswordModel>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _mobileUserService.Setup(x => x.SetPassword(It.IsAny<MobileUser>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var actualResult = await passwordController.Put(username, model);

            Assert.IsInstanceOfType(actualResult, typeof(ForbidResult));
        }
    }
}
