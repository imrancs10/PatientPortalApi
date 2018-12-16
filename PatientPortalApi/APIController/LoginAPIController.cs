using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Models;
using PatientPortalApi.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;
using static PatientPortalApi.Global.Enums;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/login")]
    public class LoginAPIController : ApiController
    {
        /// <summary>
        /// Get Patient List
        /// </summary>
        /// <returns>List of Patientinfo</returns>
        [System.Web.Http.Authorize]
        [Route("Testpatient")]
        public PatientInfo GetPatientInfo()
        {
            UserInfo userInfo = new UserInfo(User);
            PatientDetails detail = new PatientDetails();
            return detail.GetPatientDetailById(userInfo.PatientId);
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(string registrationNo, string password)
        {
            //logger.Debug("Authenticate start");
            if (!string.IsNullOrEmpty(registrationNo) && !string.IsNullOrEmpty(password))
            {
                //logger.Debug("checking user");
                PatientDetails detail = new PatientDetails();
                var result = detail.GetPatientDetail(registrationNo, password);
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
                    var registrationResult = detail.GetPatientDetailByRegistrationNumber(registrationNo);
                    if (registrationResult == null)
                    {
                        //logger.Debug("Invalid user");
                        ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                        Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                        return Ok(response);
                    }
                    else
                    {
                        PatientLoginEntry entry = new PatientLoginEntry
                        {
                            PatientId = registrationResult.PatientId,
                            LoginAttemptDate = DateTime.Now
                        };
                        var loginAttempt = detail.SavePatientLoginFailedHistory(entry);
                        if (loginAttempt.LoginAttempt == 4)
                        {
                            //logger.Debug("user locked for a day");
                            ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.AccountLocked];
                            Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                            return Ok(response);
                        }
                        else
                        {
                            //logger.Debug("login attempt counted");
                            ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.AccountFailAttempt];
                            errorDetail.Message = errorDetail.Message.Replace(Regex.Match(errorDetail.Message, @"\d+").Value, (4 - loginAttempt.LoginAttempt).ToString());
                            Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                            return Ok(response);
                        }
                    }
                }
            }

            return BadRequest();
        }
    }

}
