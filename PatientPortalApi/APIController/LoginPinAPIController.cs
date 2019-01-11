using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Models;
using PatientPortalApi.Models.User;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using static PatientPortalApi.Global.Enums;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/loginpin")]
    public class LoginPinAPIController : ApiController
    {
        [HttpGet]
        [Route("Loginwithpin/{loginPin}/{deviceIdentifier}")]
        public IHttpActionResult LoginWithPin(string loginPin, string deviceIdentifier)
        {
            if (!string.IsNullOrEmpty(loginPin) && !string.IsNullOrEmpty(deviceIdentifier))
            {
                //logger.Debug("checking user");
                PatientDetails detail = new PatientDetails();
                var result = detail.GetPatientDetailByPinAndDeviceId(loginPin, deviceIdentifier);
                //logger.Debug("checked user");
                var patientInfo = ((PatientInfo)result["data"]);
                var msg = (CrudStatus)result["status"];
                if (msg == CrudStatus.RegistrationExpired)
                {
                    //logger.Error("user expired");
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.RegistrationExpired];
                    Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                    return Ok(response);
                }
                if (patientInfo != null)
                {
                    //return token
                    if (result != null && patientInfo != null)
                    {
                        UserClaims claims = new UserClaims()
                        {
                            CRNumber = patientInfo.CRNumber,
                            PatientId = patientInfo.PatientId,
                            RegistrationNumber = patientInfo.RegistrationNumber,
                            ValidUpTo = patientInfo.ValidUpto.Value
                        };
                        string token = (new JWTTokenService()).CreateToken(claims);
                        //logger.Debug("Token generated " + token);
                        //return the token
                        JwtResponse jwtResponse = new JwtResponse()
                        {
                            Name = patientInfo.FirstName,
                            JwtToken = token
                        };
                        return Ok<JwtResponse>(jwtResponse);
                    }
                    else
                    {
                        //logger.Error("user not found");
                        // if credentials are not valid send unauthorized status code in response
                        ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                        Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                        return Ok(response);
                    }
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                    Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                    return Ok(response);
                }
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("savepin/{loginPin}/{deviceIdentifier}")]
        [Authorize]
        public IHttpActionResult SaveLoginPin(string loginPin, string deviceIdentifier)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                PatientDetails detail = new PatientDetails();

                var pinExists = detail.GetPatientByPinAndDeviceId(loginPin, deviceIdentifier);
                if (pinExists != null)
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.SamePinUse];
                    Response<object> response = new Response<object>(errorDetail, null);
                    return Ok(response);
                }
                PatientInfo info = new PatientInfo()
                {
                    PatientId = userInfo.PatientId,
                    LoginPin = loginPin,
                    DeviceIdentityfier = deviceIdentifier
                };
                var result = detail.UpdatePatientDetail(info);
                return Ok();
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
    }

}
