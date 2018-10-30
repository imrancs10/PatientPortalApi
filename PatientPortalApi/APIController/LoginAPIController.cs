using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Models;
using PatientPortalApi.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using static PatientPortalApi.Global.Enums;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace PatientPortalApi.APIController
{
    public class LoginAPIController : ApiController
    {
        /// <summary>
        /// Get Patient List
        /// </summary>
        /// <returns>List of Patientinfo</returns>
        [System.Web.Http.Authorize]
        public List<PatientInfo> GetPatientInfo()
        {
            UserInfo userInfo = new UserInfo(User);
            List<PatientInfo> list = new List<PatientInfo>() { new PatientInfo() { PatientId = 1 } };
            return list;
        }

        [HttpPost]
        public IHttpActionResult Authenticate([FromBody] string registrationNo, string password)
        {
            IHttpActionResult response;
            HttpResponseMessage responseMsg = new HttpResponseMessage();
            bool isUsernamePasswordValid = false;

            if (!string.IsNullOrEmpty(registrationNo) && !string.IsNullOrEmpty(password))
            {
                PatientDetails detail = new PatientDetails();
                var result = detail.GetPatientDetail(registrationNo, password);
                var patientInfo = ((PatientInfo)result["data"]);
                var msg = (CrudStatus)result["status"];
                if (msg == CrudStatus.RegistrationExpired)
                {
                    //Session["PatientInfoRenewal"] = patientInfo;
                    //SetAlertMessage("Registration Expired, Kindly renew it.", "Login");
                    return Ok(ErrorCode.RegistrationExpired);
                    //string daysRemaning = Convert.ToString((patientInfo.ValidUpto.Value.Date - DateTime.Now.Date).TotalDays);
                    //if (Convert.ToInt32(daysRemaning) < 0)
                    //{
                    //    //expired registration
                    //    TempData["Expired"] = true;
                    //    return RedirectToAction("TransactionPayReNewalExpired");
                    //}
                    //else
                    //{
                    //    TempData["Expired"] = false;
                    //    return RedirectToAction("TransactionPayReNewal");
                }
                if (patientInfo != null)
                {
                    //Session["PatientId"] = patientInfo.PatientId;
                    //setUserClaim(patientInfo);
                    //SaveLoginHistory(patientInfo.PatientId);
                    //return RedirectToAction("Dashboard");

                    //return token
                }
                else
                {
                    var registrationResult = detail.GetPatientDetailByRegistrationNumber(registrationNo);
                    if (registrationResult == null)
                    {
                        return BadRequest(ErrorCode.InvalidUser.ToString());
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
                            //SetAlertMessage("You have reached the maximum attempt, your account is locked for a day.", "Login");
                            return Ok(ErrorCode.RegistrationExpired);
                        }
                        else
                        {
                            return Ok(ErrorCode.AccountFailAttempt.ToString().Replace("#1004", (4 - loginAttempt.LoginAttempt).ToString()));
                        }
                    }
                    //var patientInfo = ((PatientInfo)result["data"]);
                    //if (result != null && patientInfo != null)
                    //{
                    //    UserClaims claims = new UserClaims()
                    //    {
                    //        CRNumber = patientInfo.CRNumber,
                    //        PatientId = patientInfo.PatientId,
                    //        RegistrationNumber = patientInfo.RegistrationNumber,
                    //        ValidUpTo = patientInfo.ValidUpto.Value
                    //    };
                    //    string token = (new JWTTokenService()).CreateToken(claims);
                    //    //return the token
                    //    JwtResponse jwtResponse = new JwtResponse()
                    //    {
                    //        Name = patientInfo.FirstName,
                    //        JwtToken = token
                    //    };
                    //    return Ok<JwtResponse>(jwtResponse);
                    //}
                    //else
                    //{
                    //    // if credentials are not valid send unauthorized status code in response
                    //    //loginResponse.responseMsg.StatusCode = HttpStatusCode.Unauthorized;
                    //    responseMsg.StatusCode = HttpStatusCode.NotFound;
                    //    response = ResponseMessage(responseMsg);
                    //    return response;
                    //}
                }
                return BadRequest();
            }
        }
    }
