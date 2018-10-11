using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnSolve.Mobile.Web.Infrastructure.Adaptors;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Controllers
{
    [Route("api/contact")]
    public class ContactController : ControllerBase
    {
        private readonly IContactsService _contactsService;
        private readonly IConfiguration _configuration;
        readonly IMobileUserService _mobileUserService;
        private const int defaultExpiryHours = 2;
        private readonly IMapper _mapper;

        public ContactController(IContactsService contactsService
            , IMobileUserService mobileUserService
            , IConfiguration configuration
            ,IMapper mapper)
        {
            _contactsService = contactsService;
            _configuration = configuration;
            _mobileUserService = mobileUserService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns contacts details
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// 1. A valid verification code must be provided as a querystring
        /// </remarks>
        /// <response code="400">If contactlist email is not matched with any contact or Contact is duplicated within account</response>
        /// <response code="200">OK - with a list of contacts and account details</response>
        /// <param name="contactListRequest">The contact list request consisting the code from the openApp api for android and IOS.</param>
        /// <returns>
        /// List of contacts with account details
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Get(ContactListRequest contactListRequest)
        {
            var verificationCodeInfo = await _contactsService.GetEmailVeificationInfoByCode(contactListRequest.Code);
            if (verificationCodeInfo == null || String.IsNullOrEmpty(verificationCodeInfo.Email))
            {
                return BadRequest();
            }
            if (_contactsService.HasVerificationCodeExpired(verificationCodeInfo))
            {
                await _contactsService.DeleteEmailVerificationCode(verificationCodeInfo.Email);
                return Forbid();
            }
            var doesDuplicateExists = await _contactsService.IsContactDuplicatedWithinAccount(verificationCodeInfo.Email);
            if (doesDuplicateExists)
            {
                return BadRequest();
            }

            var contactPointDetails = await _contactsService.GetContactList(verificationCodeInfo.Email);

            var ensContacts = await _contactsService.GetENSContacts(verificationCodeInfo.Email);

            var requiredContacts = contactPointDetails.Where(x => !(ensContacts.Select(c => c.AccountId).Contains(x.AccountId)
                                          && ensContacts.Select(c => c.RecipientId).Contains(x.RecipientId)));
          
            ensContacts.AddRange(requiredContacts);
            var contactListResponse = new ContactListResponse()
            {
                ContactList = _mapper.Map<List<ContactDetailModel>>(ensContacts),
                Email = verificationCodeInfo.Email
            };

            await _contactsService.DeleteEmailVerificationCode(verificationCodeInfo.Email);
            return Ok(contactListResponse);
        }

        /// <summary>
        /// Returns a contact verification link based on IOS and Android device
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// This is api returns a url for opening app.
        /// If user-agent contains android, then this api will return android url, for all other cases, it returns Ios url.
        /// </remarks>
        /// <response code="200">OK - If reset password link is sent.</response>
        /// <response code="400">If username or resetcode is not provided</response>
        /// <param name="code">code received from the email verification email.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("openappforemailverification")]
        public IActionResult Get([FromQuery] string code)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            var urlTemplate = Request.Headers["User-Agent"].ToString().ToLower().Contains("android")
                                        ? _configuration["EmailVerificationAndroidUrl"]
                                        : _configuration["EmailVerificationIosUrl"];

            var url = String.Format(urlTemplate, code);
            return Redirect(url);
        }

        /// <summary>
        /// verifies the ownership of the email to the contact.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// 1. A valid email address must be provided as a querystring
        /// </remarks>
        /// <response code="400">If contactlist email is not matched with any contact or Contact is duplicated within account</response>
        /// <response code="200">OK - with a list of contact points and account details</response>
        /// <param name="emailVerificationRequest">The contact list request.</param>
        /// <returns>
        /// Ok and sends a verification email to the given email id.
        /// </returns>
        [HttpPost]
        [Route("emailverification")]
        public async Task<IActionResult> Post(EmailVerificationRequest emailVerificationRequest)
        {
            var mobileUser = await _mobileUserService.GetMobileUser(emailVerificationRequest.Email);
            if (mobileUser != null)
            {
                return BadRequest();
            }
            if (await _contactsService.IsContactDuplicatedWithinAccount(emailVerificationRequest.Email))
            {
                return BadRequest();
            }
            var result = await _contactsService.GetContactList(emailVerificationRequest.Email);
            var ensContacts = await _contactsService.GetENSContacts(emailVerificationRequest.Email);

            result.AddRange(ensContacts);
            if (!result.Any())
            {
                return BadRequest();
            }
            await _contactsService.RequestMail(emailVerificationRequest.Email);
            return Ok();
        }
    }
}
