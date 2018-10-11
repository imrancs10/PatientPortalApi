using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Routing;

namespace OnSolve.Mobile.Web.Controllers
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IMobileUserService _mobileUserService;

        public UserController(IUserService userService, IJwtService jwtService , IMobileUserService mobileUserServive)
        {
            _userService = userService;
            _jwtService = jwtService;
            _mobileUserService = mobileUserServive;
        }

        /// <summary>
        /// Creates/Registers a contact point user and returns the jwt token.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// 1. recipientId - Mandatory
        /// 2. emailAddress - Mandatory (should be valid email address)
        /// 3. accountId - Mandatory (and should match recipient id)
        /// 4. password - Required, if account settings need it. Should confirm to password requirements.
        /// </remarks>
        /// <response code="200">User created successfully and jwt token sent.</response>
        /// <response code="400">BadRequest(Password is empty or registration details are invalid.</response>
        /// <param name="userRequest">The user model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserRequest userRequest)
        {
            if (String.IsNullOrWhiteSpace(userRequest.Password))
            {
                return BadRequest();
            }
            if (!await _userService.AreRegistrationDetailsValid(userRequest))
            {
                return BadRequest();
            }
            await _userService.CreateUser(userRequest);
            var mobileUser = await _userService.GetMobileUser(userRequest.EmailAddress);
            var jwtToken = await _jwtService.GetJwtTokenForUser(mobileUser);

            if(String.IsNullOrEmpty(jwtToken))
            {
                return BadRequest();
            }

            var jwtResponse = await _jwtService.CreateJwtResponse(jwtToken,userRequest.AccountId);
            return Ok(jwtResponse);
        }

        /// <summary>
        /// Get user detail.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// 1. Provide JWT token
        /// </remarks>
        /// <response code="200">Successful result.</response>
        /// <response code="404">users not found.</response>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            UserInfo userInfo = new UserInfo(User);
            var mobileUser = await _mobileUserService.GetMobileUser(userInfo.MobileUserId);
            if(mobileUser == null)
            {
                return BadRequest();
            }
            var result = await _userService.GetMobileUserContactDetail(mobileUser.Username);
            var userdetailModel = new UserDetailModel()
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                EmailId = result.EmailId,
                PhoneNumber = result.PhoneNumber
            };
            return Ok(userdetailModel);
        }

        /// <summary>
        /// Login the ENS user and returns the details so that the mobile user can be registered.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// 1. Provide list of recipient id
        /// </remarks>
        /// <response code="200">Successful result along with the ENS user details returned.</response>
        /// <response code="400">Invalid credentials.</response>
        /// <param name="loginModel">Model containing the ENS user credentials.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ens")]
        public async Task<IActionResult> Post([FromBody] ENSLoginModel loginModel)
        {
            var isCredentialsValid = await _jwtService.ValidateEnsUser(loginModel.username, loginModel.password);
            if (!isCredentialsValid)
            {
                return BadRequest();
            }
            var ensUser = await _userService.GetENSUserDetails(loginModel.username);
            if (ensUser == null)
            {
                return BadRequest();
            }
            return Ok(ensUser);
        }

    }
}
