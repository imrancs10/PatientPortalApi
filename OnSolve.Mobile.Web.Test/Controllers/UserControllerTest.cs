using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        Mock<IUserService> _userService;
        Mock<IJwtService> _jwtService;
        UserController controller;
        [TestInitialize]
        public void TestInitialize()
        {
            _userService = new Mock<IUserService>();
            _jwtService = new Mock<IJwtService>();
            controller = new UserController(_userService.Object, _jwtService.Object,null);
        }

        [TestMethod]
        public void UserRequestModel_ReturnTrue_WhenValidateModelPass()
        {

            UserRequest userRequest = new UserRequest
            {
                 AccountId = 1,
                 RecipientId = 1,
                 EmailAddress = "abcd@gmail.com",
                 Password = null
            };
            var context = new ValidationContext(userRequest, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(userRequest, context, result, true);
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public void UserRequestModel_ReturnFalse_WhenValidateModelFail()
        {

            UserRequest userRequest = new UserRequest
            {
                AccountId = 1,
                RecipientId = 1,
                EmailAddress = null,
                Password = null
            };
            var context = new ValidationContext(userRequest, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(userRequest, context, result, true);
            isValid.Should().BeFalse();
        }

        [TestMethod]
        public async Task Post_ThrowsBadRequest_WithBlankPasswordWhenItsRequiredforAccount()
        {
            IActionResult result = await controller.Post(GenerateRequest(""));
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_ThrowsBadRequest_WhenRegistrationDetailsarenotValid()
        {
            _userService.Setup(repo => repo.AreRegistrationDetailsValid(It.IsAny<UserRequest>()))
                .ReturnsAsync(false);
            var result = await controller.Post(GenerateRequest("pass"));
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_ReturnsOkwithCreatedAtId_WhenRegistrationDetailsareValid()
        {
            _userService.Setup(repo => repo.AreRegistrationDetailsValid(It.IsAny<UserRequest>()))
                .ReturnsAsync(true);
            _userService.Setup(repo => repo.CreateUser(It.IsAny<UserRequest>()))
                .ReturnsAsync(new CreateUserResponse
                {
                    CreatedAtId = 2
                });

            MobileUser user = new MobileUser
            {
                Id=1,
                AccountId = 123,
                Username = "emailtest@onsolve.com"
            };
            _userService.Setup(repo => repo.GetMobileUser(It.IsAny<string>())).ReturnsAsync(user);
            string jwtToken = "token";
            _jwtService.Setup(repo => repo.GetJwtTokenForUser(It.IsAny<MobileUser>())).ReturnsAsync(jwtToken);
            var jwtResponse = new JwtResponse();
            jwtResponse.JwtToken = jwtToken;
            jwtResponse.AccountName = "test";
            jwtResponse.CompanyName = "test";
            _jwtService.Setup(repo => repo.CreateJwtResponse(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(jwtResponse);
            var result = await controller.Post(GenerateRequest("pass"));
            result.Should().BeOfType<OkObjectResult>();            
            ((OkObjectResult)result).Value.Should().BeOfType<JwtResponse>();
        }
               
        
        private static UserRequest GenerateRequest(string password)
        {
            return new UserRequest
            {
                AccountId = 123,
                EmailAddress = "emailtest@onsolve.com",
                Password = password,
                RecipientId = 12
            };
        }
    }
}
