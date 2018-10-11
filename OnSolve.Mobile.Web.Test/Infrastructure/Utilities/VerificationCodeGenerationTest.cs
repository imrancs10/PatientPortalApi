using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Web.Infrastructure.Utilities;

namespace OnSolve.Mobile.Web.Test
{
    /// <summary>
    /// Summary description for VerificationCodeGenerationTest
    /// </summary>
    [TestClass]
    public class VerificationCodeGenerationTest
    {
        private const string _deviceVerification = "VerificationCode.CodeLength.DeviceVerification";
        private const string _securityCode = "VerificationCode.CodeLength.ResetCode";
        IVerificationCodeGeneration _verificationCodeGeneration;
        private Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        public VerificationCodeGenerationTest()
        {
            _configuration.As<IConfiguration>()
                .Setup(config => config[_deviceVerification])
                .Returns(6.ToString());
            _configuration.As<IConfiguration>()
                 .Setup(config => config[_securityCode])
                 .Returns(20.ToString());
            _verificationCodeGeneration = new VerificationCodeGeneration(_configuration.Object);
        }

        [TestMethod]
        public void VerificationCodeNCharacters_LengthShouldBeGreaterThanOrEqual6()
        {
            var verificationCode = _verificationCodeGeneration.GenerateRandomString();

            verificationCode.Length.Should().BeGreaterOrEqualTo(6);
        }

        [TestMethod]
        public void VerificationCode6Characters_NoTwoShouldBeSame()
        {
            var verificationCode = _verificationCodeGeneration.GenerateRandomString();

            var otherVerificationCode = _verificationCodeGeneration.GenerateRandomString();

            verificationCode.Should().NotMatch(otherVerificationCode);
        }

        [TestMethod]
        public void SecurityCodeAuthentication_NoTwoShouldBeSame()
        {
            var code = _verificationCodeGeneration.GenerateResetCode();

            var otherCode = _verificationCodeGeneration.GenerateResetCode();

            code.Should().NotMatch(otherCode);
        }



        [TestMethod]
        public void DeviceTokenCodeAuthentication_NoTwoShouldBeSame()
        {
            var code = _verificationCodeGeneration.GenerateRandomString();

            var otherCode = _verificationCodeGeneration.GenerateRandomString();

            code.Should().NotMatch(otherCode);
        }
    }
}
