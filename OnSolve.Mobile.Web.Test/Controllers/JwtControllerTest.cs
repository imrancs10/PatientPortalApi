using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Controllers
{
    [TestClass]
    public class JwtControllerTest
    {
        Mock<IUserService> _userService;
        Mock<IJwtService> _jwtService;
        const string username = "sheik.imran@onsolve.com";
        const string password = "Swn1234567";
        const string deviceToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        const string FCMToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        const string JWTToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        JwtController jwtController;

        [TestInitialize]
        public void TestInitialize()
        {
            _userService = new Mock<IUserService>();
            _jwtService = new Mock<IJwtService>();
            jwtController = new JwtController(_jwtService.Object,
                                                _userService.Object);
        }

        [TestMethod]
        public async Task CreateJWTToken_ReturnsOK_WhenInputIsValid()
        {
            JwtRequest request = new JwtRequest()
            {
                FCMToken = FCMToken,
                Password = password
            };
            JwtResponse response = new JwtResponse()
            {
                JwtToken = JWTToken
            };
            Errorable<JwtResponse> errable = new Errorable<JwtResponse>(response);
            MobileUser mobileUser = new MobileUser()
            {
                Id = 1,
                AccountId = 1,
            };

            _userService.Setup(x => x.GetMobileUser(It.IsAny<string>())).ReturnsAsync(mobileUser);

            _jwtService.Setup(x => x.Login(It.IsAny<MobileUser>(), It.IsAny<JwtRequest>())).ReturnsAsync(errable);

            var actualResult = await jwtController.Post(username,request);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task CreateJWTToken_ReturnsBadRequest_WhenLoginFail()
        {
            JwtRequest request = new JwtRequest()
            {
                FCMToken = FCMToken,
                Password = password
            };
            Errorable<JwtResponse> errable = new Errorable<JwtResponse>(ErrorCode.InvalidUsernameOrPassword);
            MobileUser mobileUser = new MobileUser()
            {
                Id = 1,
                AccountId = 1,
            };

            _userService.Setup(x => x.GetMobileUser(It.IsAny<string>())).ReturnsAsync(mobileUser);

            _jwtService.Setup(x => x.Login(It.IsAny<MobileUser>(), It.IsAny<JwtRequest>())).ReturnsAsync(errable);

            var actualResult = await jwtController.Post(username, request);

            actualResult.Should().BeOfType<ObjectResult>();

            ((ObjectResult)actualResult).Value.ToString().Contains(ErrorCode.InvalidUsernameOrPassword.ToString()).Should().Be(true);
        }

    }
}
