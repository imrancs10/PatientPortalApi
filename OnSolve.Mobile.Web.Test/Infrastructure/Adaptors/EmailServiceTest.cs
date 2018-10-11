using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Models;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test
{
    /// <summary>
    /// Summary description for EmailServiceTest
    /// </summary>
    [TestClass]
    public class EmailServiceTest
    {
        const string _fromEmailConfiguration = "EmailSetting:Email:0:FromEmail";
        const string _fromNameConfiguration = "EmailSetting:Email:0:FromName";
        const string _apiKeyConfiguration = "EmailSetting:Email:0:ApiKey";

        const string _fromEmailValue = "sheikh.imran@nagarro.com";
        const string _fromNameValue = "onsolve mobile";
        const string _apiKeyValue = "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";

        readonly Mock<IConfiguration> _configuration;
        readonly Mock<IEmailService> _emailService;
        readonly EmailRequest _email;
        public EmailServiceTest()
        {
            _configuration = new Mock<IConfiguration>();

            _emailService = new Mock<IEmailService>();

            _email = new EmailRequest();
        }

        private EmailService ArrangeForTests()
        {
            // Arrange
            _configuration.As<IConfiguration>()
            .Setup(config => config[_fromEmailConfiguration])
            .Returns(_fromEmailValue);

            _configuration.As<IConfiguration>()
           .Setup(config => config[_fromNameConfiguration])
           .Returns(_fromNameValue);

            _configuration.As<IConfiguration>()
           .Setup(config => config[_apiKeyConfiguration])
           .Returns(_apiKeyValue);

            _email.EmailTo = "sheikh.imran@nagarro.com";
            _email.NameTo = "Imran Sheikh";
            _email.Subject = "Test";
            _email.Body = "This is a test email";

            return new EmailService(_configuration.Object);
        }

        [TestMethod]
        public async Task TestSendEmail_ReturnValue_Should_Be_True()
        {
            //Arrange
            EmailService service = ArrangeForTests();

            // Act
            var actualResult = await service.TrySendEmail(_email);

            //Assert
            actualResult.Should().BeTrue();
        }

        [TestMethod]
        public async Task TestSendEmail_ReturnValue_Should_Be_True_When_NameTo_IsEmpty()
        {
            //Arrange
            EmailService service = ArrangeForTests();
            _email.NameTo = string.Empty;
            // Act
            var actualResult = await service.TrySendEmail(_email);
            //Assert
            actualResult.Should().BeTrue();
        }

        [TestMethod]
        public async Task TestSendEmail_ReturnValue_Should_Be_False_When_EmailTo_IsEmpty()
        {
            //Arrange
            EmailService service = ArrangeForTests();
            _email.EmailTo = string.Empty;
            // Act
            var actualResult = await service.TrySendEmail(_email);
            //Assert
            actualResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task TestSendEmail_ReturnValue_Should_Be_False_When_Body_IsEmpty()
        {
            //Arrange
            EmailService service = ArrangeForTests();
            _email.Body = string.Empty;
            // Act
            var actualResult = await service.TrySendEmail(_email);
            //Assert
            actualResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task TestSendEmail_ReturnValue_Should_Be_False_When_Subject_IsEmpty()
        {
            //Arrange
            EmailService service = ArrangeForTests();
            _email.Subject = string.Empty;
            // Act
            var actualResult = await service.TrySendEmail(_email);
            //Assert
            actualResult.Should().BeFalse();
        }

    }
}
