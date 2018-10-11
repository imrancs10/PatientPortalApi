using AsyncPoco;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using OnSolve.Mobile.Web.Swn402Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services
{
    [TestClass]
    public class ContactsServiceTest
    {
        private readonly IContactsService _contactsService;
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IVerificationCodeGeneration> _verificationService;
        private readonly Func<OnSolveMobileContext> dbContextProvider;
        Mock<IDatabase> _swn402Db;

        public ContactsServiceTest()
        {
            _swn402Db = new Mock<IDatabase>();
            _emailService = new Mock<IEmailService>();
            _configuration = new Mock<IConfiguration>();
            _verificationService = new Mock<IVerificationCodeGeneration>();
            _contactsService = new ContactsService(_swn402Db.Object, _emailService.Object, _configuration.Object, _verificationService.Object, dbContextProvider);
        }

        [TestMethod]
        public async Task GetContactsByEmail_ReturnsContactList()
        {
            _swn402Db.Setup(x => x.FetchAsync<ContactDetail>(It.IsAny<Sql>()))
                .ReturnsAsync(new List<ContactDetail>()
                {
                    new ContactDetail()
                    {
                        AccountId = 1
                        , AccountName = "testAccount"
                        , RecipientId = 1
                        , FirstName = "firstName"
                    }
                });

            _swn402Db.Setup(x => x.FetchAsync<int>(It.IsAny<Sql>()))
                .ReturnsAsync(new List<int>());

            var actualResult = await _contactsService.GetContactList("testuser@gmail.com");

            actualResult.Count().Should().Be(1);
            actualResult[0].AccountName.Should().Be("testAccount");
        }

        [TestMethod]
        public async Task GetContactsByEmail_ReturnsErrorCode_DuplicateContactsWithinAccount()
        {
            _swn402Db.Setup(x => x.FetchAsync<int>(It.IsAny<Sql>()))
                .ReturnsAsync(new List<int>() { 1 });

            var actualResult = await _contactsService.GetContactList("testuser@gmail.com");

            actualResult.Should().BeNullOrEmpty();
        }
    }
}