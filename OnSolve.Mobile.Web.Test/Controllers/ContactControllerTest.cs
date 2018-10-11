using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Controllers;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using OnSolve.Mobile.Web.Swn402Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Controllers
{
    [TestClass]
    public class ContactControllerTest
    {
        Mock<IContactsService> _contactService;
        Mock<IMobileUserService> _mobileUserService;
        Mock<IConfiguration> _configuration;
        Mock<IMapper> _mapper;
        ContactController controller;
        [TestInitialize]
        public void TestInitialize()
        {
            _configuration = new Mock<IConfiguration>();
            _contactService = new Mock<IContactsService>();
            _mobileUserService = new Mock<IMobileUserService>();
            _mapper = new Mock<IMapper>();
            controller = new ContactController(_contactService.Object, _mobileUserService.Object, _configuration.Object, _mapper.Object);
        }

        [TestMethod]
        public void ContactListRequestModel_ReturnFalse_WhenValidateModelFail()
        {

            ContactListRequest contactListRequest = new ContactListRequest
            {
                Code = null
            };
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(contactListRequest, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contactListRequest, context, result, true);
            isValid.Should().BeFalse();
        }
        [TestMethod]
        public void ContactListRequestModel_ReturnTrue_WhenValidateModelPass()
        {

            ContactListRequest contactListRequest = new ContactListRequest
            {
                Code = "abcde@gmail.com"
            };
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(contactListRequest, null, null);
            var result = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contactListRequest, context, result, true);
            isValid.Should().BeTrue();
        }

        [TestMethod]
        public async Task Get_ThrowsBadRequest_WithDuplicateContactWithinAccount()
        {
            _contactService.Setup(repo => repo.IsContactDuplicatedWithinAccount(It.IsAny<string>()))
                .ReturnsAsync(true);
            var result = await controller.Get(
                new ContactListRequest
                {
                    Code = "emailtest@onsolve.com"
                });
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Get__ThrowsBadRequest_WhenUnknownEmail()
        {

            _contactService.Setup(repo => repo.IsContactDuplicatedWithinAccount(It.IsAny<string>()))
                .ReturnsAsync(false);
            List<ContactDetail> contactListResponse = null;
            _contactService.Setup(repo => repo.GetContactList(It.IsAny<string>()))
                .ReturnsAsync(contactListResponse);
            var result = await controller.Get(
                new ContactListRequest { Code = "emailtest@onsolve.com" });
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Get__ReturnOkResult_WithContactListWhenValidUser()
        {
            EmailVerificationCode info = new EmailVerificationCode
            {
                Email = "emailtest@onsolve.com"
            };
            _contactService.Setup(repo => repo.GetEmailVeificationInfoByCode(It.IsAny<string>()))
                .ReturnsAsync(info);
            _contactService.Setup(repo => repo.HasVerificationCodeExpired(It.IsAny<EmailVerificationCode>()))
                .Returns(false);
            _contactService.Setup(repo => repo.IsContactDuplicatedWithinAccount(It.IsAny<string>()))
                .ReturnsAsync(false);
            _contactService.Setup(repo => repo.GetContactList(It.IsAny<string>()))
                .ReturnsAsync(GetContacts("emailtest@onsolve.com"));
            _contactService.Setup(repo => repo.GetENSContacts(It.IsAny<string>()))
                .ReturnsAsync(GetContacts("emailtest@onsolve.com"));
            var result = await controller.Get(
                new ContactListRequest { Code = "emailtest@onsolve.com" });
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeOfType<List<ContactDetail>>();
        }

        private List<ContactDetail> GetContacts(string v)
        {
            return new List<ContactDetail>()
                {
                    new ContactDetail
                    {
                        AccountId= 1,
                        AccountName = "ABC",
                        FirstName = "Jonathan",
                        LastName = "Trott",
                        RecipientId = 4
                    }
            };
        }
    }
}
