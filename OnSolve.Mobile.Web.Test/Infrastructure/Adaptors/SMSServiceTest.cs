using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test
{
    /// <summary>
    /// Summary description for SMSServiceTest
    /// </summary>
    [TestClass]
    public class SMSServiceTest
    {
        private const string _url = "http://localhost:27699/api/Messages";
        private const string _body = "Sending OPT 567567";
        private const string _phoneNumber = "+919910780960";
        private const string _smsServiceConfiguration = "SMSService";
        private const string _secretKey = "TbcXuUsuuVPkSeRRV7zfjPLRcN78K7te9v3t2jLkQbvj2b2yTrD362vTrcSLaaQUvvXmDzg6mCeXq6nU8BMPdTyuVfQV2xF8Ruj6DcMAESqEznqhNNWSxmWhsDQ5L73t";

        private Mock<IConfiguration> _configuration;
        private Mock<IHttpNotificationClient> _httpClient;

        public SMSServiceTest()
        {
            _configuration = new Mock<IConfiguration>();
            _httpClient = new Mock<IHttpNotificationClient>();
        }

        private SMSService ArrangeForTests()
        {
            // Arrange
            _configuration.As<IConfiguration>()
                .Setup(config => config[_smsServiceConfiguration])
                .Returns(_url);

            _httpClient.As<IHttpNotificationClient>()
                .Setup(rsa => rsa.SendPost(It.IsAny<SMSMessageModel>(), _url, _secretKey))
                .ReturnsAsync(new HttpResponseMessage());

            return new SMSService(_configuration.Object, _httpClient.Object);
        }

        [TestMethod]
        public async Task TestSendMessage_ReturnIdShouldNotBeNull()
        {
            //Arrange
            SMSService service = ArrangeForTests();

            // Act
            var actualResult = await service.SendMessage(_body, _phoneNumber);

            //Assert
            actualResult.Should().NotBeNull();
            actualResult.StatusCode.Should().NotBeNull();
        }
    }
}
