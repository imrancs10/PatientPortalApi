using Microsoft.AspNetCore.Mvc;
using OnSolve.Mobile.Web.Filters;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services.Interface;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Controllers
{
    public class JwtController : ControllerBase
    {
        readonly IJwtService _jwtService;
        readonly IUserService _userService;

        public JwtController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        /// <summary>
        ///  Provides the Jwt token for the user.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// Device of user must be verified before reaching this step.
        /// </remarks>
        /// <response code="200">OK - If able to login.</response>
        /// <response code="401">Unauthorized if username or password is incorrect.</response>
        /// <param name="username"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("api/user/{username}/jwt")]
        [HttpPost]
        [ValidMobileUser]
        public async Task<IActionResult> Post(string username, [FromBody] JwtRequest request)
        {
            var user = await _userService.GetMobileUser(username);
            var result = await _jwtService.Login(user, request);
            return result.BuildResult(Ok);
        }

    }
}
