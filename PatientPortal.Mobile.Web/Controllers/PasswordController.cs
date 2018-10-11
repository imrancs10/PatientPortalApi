using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Data.Enum;
using PatientPortal.Mobile.Web.Filters;
using PatientPortal.Mobile.Web.Infrastructure.Utilities;
using PatientPortal.Mobile.Web.Models;
using PatientPortal.Mobile.Web.Services.Interface;
using PatientPortal.Mobile.Web.Swn402Entities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Controllers
{
    [Route("api/user")]
    public class PasswordController : ControllerBase
    {
        readonly IPasswordService _passwordService;
        readonly IMobileUserService _mobileUserService;
        readonly IConfiguration _configuration;
        readonly IUserService _userService;
        readonly IPasswordHasherService _passwordHasherService;

        public PasswordController(IPasswordService passwordService
            , IConfiguration configuration
            , IMobileUserService mobileUserService
            , IUserService userService,
              IPasswordHasherService passwordHasherService)
        {
            _passwordService = passwordService;
            _configuration = configuration;
            _mobileUserService = mobileUserService;
            _userService = userService;
            _passwordHasherService = passwordHasherService;
        }


        /// <summary>
        /// Sends the reset password link to user
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// Device of user must be verified before reaching this step.
        /// An email/message with reset password link will be sent to user.
        /// </remarks>
        /// <response code="200">OK - If reset password link is sent.</response>
        /// <response code="400">If user is not valid.</response>
        /// <response code="403">If user is restricted user.</response>
        /// <param name="username">Username for which we want to reset password.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{username}/passwordresetchallenge")]
        [ValidMobileUser] 
        public async Task<IActionResult> Post(string username)
        {
            ContactDetail contactDetail = await _userService.GetMobileUserContactDetail(username);
            if (contactDetail.Locked)
            {
                return Forbid();

            }
            await _passwordService.RequestPasswordResetCode(contactDetail);
            return Ok();
        }

        /// <summary>
        /// Returns a reset password link based on IOS and Android device
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// This is API returns a url for opening app.
        /// It won't verify security code and user, that would be done in later step.
        /// If user-agent contains android, then this API will return android url, for all other cases, it returns IOS url.
        /// </remarks>
        /// <response code="200">OK - If reset password link is sent.</response>
        /// <response code="400">If username or resetcode is not provided</response>
        /// <param name="resetCode">To Get from request header.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("openappforpasswordreset")]
        public async Task<IActionResult> Get([FromQuery] string resetCode)
        {
            if (String.IsNullOrWhiteSpace(resetCode))
            {
                return BadRequest();
            }
            
            ResetPasswordCode info = await _passwordService.GetResetCodeInfo(resetCode);
            
            var urlTemplate = Request.Headers["User-Agent"].ToString().ToLower().Contains("android")
                                        ? _configuration["ResetPasswordAndroidUrl"]
                                        : _configuration["ResetPasswordIosUrl"];

            string url = String.Format(urlTemplate, resetCode, true, string.Empty);
            if (info != null)
            {
                url = String.Format(urlTemplate, resetCode, !_passwordService.HasResetCodeExpired(info), info.MobileUser.Username);
            }
            return Redirect(url);
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// Device of user must be verified before reaching this step.
        /// An authorized email/message that was sent initiates this request.
        /// 
        /// </remarks>
        /// <response code="200">OK - If password created.</response>
        /// <response code="400">If request is not valid for password reset.</response>
        /// <response code="403">When reset code is invalid.</response>
        /// <param name="passwordModel">New Password</param>
        /// <param name="resetcode">reset Code to be veified</param>
        /// <returns></returns>
        [HttpPost]
        [Route("password/{resetcode}")]
        public async Task<IActionResult> Create(string resetcode
            , [FromBody] PasswordModel passwordModel)
        {
            ResetPasswordCode info = await _passwordService.GetResetCodeInfo(resetcode);
            if (info == null)
            {
                return BadRequest();
            }
            if (_passwordService.HasResetCodeExpired(info))
            {
                await _passwordService.DeleteResetCode(info.MobileUser);
                return StatusCode((int)HttpStatusCode.Forbidden, new
                {
                    email = info.MobileUser.Username
                });
            }
            await _mobileUserService.SetPassword(info.MobileUser, passwordModel.Password);
            await _passwordService.DeleteResetCode(info.MobileUser);
            return Ok();
        }


        /// <summary>
        /// Change user password
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// User should be authorized for password change.    
        /// 
        /// </remarks>
        /// <response code="200">OK - If password changed.</response>
        /// <response code="400">If User name or password do not matched.</response>
        /// <response code="404">If user is not found.</response>
        /// <response code="403">If user is locked.</response>
        /// <param name="username">Username for which you want to change password.</param>
        /// <param name="changePasswordModel">Change password Details</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{username}/password")]
        [Authorize]
        public async Task<IActionResult> Put(string username
            , [FromBody] ChangePasswordModel changePasswordModel)
        {
            MobileUser mobileUser = await _mobileUserService.GetMobileUser(username);
            if (mobileUser == null)
            {
                return NotFound();
            }

            ContactDetail contactDetail = await _userService.GetMobileUserContactDetail(username);
            if (contactDetail.Locked)
            {
                return Forbid();
            }

            bool isPasswordValid = _passwordHasherService.IsValidPassword(changePasswordModel, mobileUser.Password, mobileUser.Salt);
            if (!isPasswordValid)
            {
                return BadRequest();
            }

            await _mobileUserService.SetPassword(mobileUser, changePasswordModel.NewPassword);
            return Ok();
        }
    }
}