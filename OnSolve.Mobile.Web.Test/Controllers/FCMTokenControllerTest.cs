using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Services.Interface;
using System.Security.Claims;
using System.Threading.Tasks;
using OnSolve.Mobile.Web.Models;

namespace OnSolve.Mobile.Web.Test.Controllers
{
    [TestClass]
    public class FCMTokenControllerTest
    {
        Mock<IFCMTokenService> _fcmTokenService;
        private const string username = "sheik.imran@onsolve.com";
        private const string password = "Swn1234567";
        private const string newPassword = "onsolve123";
        private const string deviceToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        private const string FCMToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        private const string JWTToken = "zPzEVC]vialCUwsvW5|oR{WO!B$COBw(AX@mE]q}[@g4D`BpqqWOO!cQTkP4Fd0F";
        FCMTokenController fcmController;

        [TestInitialize]
        public void TestInitialize()
        {
            _fcmTokenService = new Mock<IFCMTokenService>();
            fcmController = new FCMTokenController(_fcmTokenService.Object);
        }

        [TestMethod]
        public async Task CreateFCMToken_ReturnsOK_WhenInputIsValid()
        {
            _fcmTokenService.Setup(x => x.PostFCMTokenInfo(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            SetUserIdentity();
            var FCMTokenRequest = new FCMTokenRequest() { FCMToken = FCMToken };
            var actualResult = await fcmController.Post(FCMTokenRequest);

            actualResult.Should().BeOfType<OkResult>();
        }

        private void SetUserIdentity()
        {
            fcmController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                   {
                        new Claim(ClaimTypes.Name, username)
                   }, "testAuthType"))
                }
            };
        }
    }

}
