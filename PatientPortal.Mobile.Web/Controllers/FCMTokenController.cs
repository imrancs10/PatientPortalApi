using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Mobile.Web.Models;
using PatientPortal.Mobile.Web.Services.Interface;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Controllers
{
    [Authorize]
    public class FCMTokenController : ControllerBase
    {
        readonly IFCMTokenService _fcmTokenService;
        
        public FCMTokenController(IFCMTokenService fcmTokenService)
        {
            _fcmTokenService = fcmTokenService;
        }

        /// <summary>
        ///  creates fcm token for user corresponding to a device.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// User should be logged in and have valid jwt token
        /// </remarks>
        /// <response code="200">Ok - on successful creation of fcmToken on server</response>
        /// <param name="fcmTokenRequestModel"></param>
        /// <returns></returns>
        [Route("api/fcmtoken")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FCMTokenRequest fcmTokenRequestModel)
        {
            UserInfo userInfo = new UserInfo(User);
            await _fcmTokenService.PostFCMTokenInfo(userInfo.MobileUserId, fcmTokenRequestModel.FCMToken);
            return Ok();
        }

        /// <summary>
        ///  Update fcm token for user corresponding to a device.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// User should be logged in and have valid jwt token
        /// </remarks>
        /// <response code="201">Ok - on successful updation of fcmToken</response>
        /// <param name="fcmTokenUpdateModel"></param>
        /// <returns></returns>

        [Route("api/fcmtoken")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]FCMTokenUpdateModel fcmTokenUpdateModel)
        {
            UserInfo userInfo = new UserInfo(User);
            await _fcmTokenService.UpdateFCMTokenInfo(userInfo.MobileUserId,fcmTokenUpdateModel);
            return Ok();
        }

        /// <summary>
        ///  Delete fcm token for user corresponding to a device.
        /// </summary>
        /// <remarks>
        /// ### Usage Notes ###
        /// User should be logged in and have valid jwt token
        /// </remarks>
        /// <response code="201">Ok - on successful deletion of fcmToken</response>
        /// <param name="fcmTokenRequestModel"></param>
        /// <returns></returns>

        [Route("api/fcmtoken")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]FCMTokenRequest fcmTokenRequestModel)
        {
            UserInfo userInfo = new UserInfo(User);
            await _fcmTokenService.DeleteFCMTokenInfo(userInfo.MobileUserId,fcmTokenRequestModel.FCMToken);
            return Ok();
        }
    }
}
