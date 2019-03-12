using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Models;
using PatientPortalApi.Models.User;
using System.Web.Http;
using static PatientPortalApi.Global.Enums;
using System.Linq;
using System.Threading.Tasks;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.BAL.Commom;

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
                var patients = detail.GetPatientLoginDetail(loginPin, deviceIdentifier);
                if (patients.Count > 1)
                {
                    return Ok(patients);
                }

                var result = detail.GetPatientDetailByPinAndDeviceId(patients.FirstOrDefault());
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

        [HttpGet]
        [Route("Loginwithregistration/{registrationNumber}")]
        public IHttpActionResult LoginWithRegistrationNumber(string registrationNumber)
        {
            if (!string.IsNullOrEmpty(registrationNumber))
            {
                //logger.Debug("checking user");
                PatientDetails detail = new PatientDetails();
                var patient = detail.GetPatientDetailByRegistrationNumber(registrationNumber);
                var result = detail.GetPatientDetailByPinAndDeviceId(patient);
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
                //same mpin now used 
                //var pinExists = detail.GetPatientByPinAndDeviceId(loginPin, deviceIdentifier);
                //if (pinExists != null)
                //{
                //    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.SamePinUse];
                //    Response<object> response = new Response<object>(errorDetail, null);
                //    return Ok(response);
                //}
                PatientInfo info = new PatientInfo()
                {
                    PatientId = userInfo.PatientId,
                    LoginPin = loginPin,
                    DeviceIdentityfier = deviceIdentifier
                };
                var result = detail.UpdatePatientDetail(info);
                return Ok("Login Pin has been Saved.");
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("adduserprofile/{registrationNumber}")]
        [Authorize]
        public IHttpActionResult AddUserProfile(string registrationNumber)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                PatientDetails detail = new PatientDetails();
                var patient = detail.GetPatientDetailByRegistrationNumber(registrationNumber);
                string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                PatientInfo info = new PatientInfo
                {
                    PatientId = patient.PatientId,
                    OTP = verificationCode
                };
                var updatedPatient = detail.UpdatePatientDetail(info);
                SendMailForAppointment(updatedPatient);
                return Ok("OTP Sent to register mobile/email.");
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("verifyadduserotp/{registrationNumber}/{otp}")]
        [Authorize]
        public IHttpActionResult VerifyAddUserProfile(string registrationNumber, string otp)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                PatientDetails detail = new PatientDetails();
                var patient = detail.GetPatientDetailByRegistrationNumber(registrationNumber);
                var loginUser = detail.GetPatientDetailByRegistrationNumber(userInfo.RegistrationNumber);
                if (patient.OTP == otp)
                {
                    PatientInfo info = new PatientInfo
                    {
                        PatientId = patient.PatientId,
                        OTP = "NULL",
                        DeviceIdentityfier = loginUser.DeviceIdentityfier,
                        LoginPin = loginUser.LoginPin
                    };
                    var updatedPatient = detail.UpdatePatientDetail(info);
                    return Ok("User added successfully.");
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.OTPNotMatched];
                    Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                    return Ok(response);
                }
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("userprofilelist/{deviceIdentifier}")]
        [Authorize]
        public IHttpActionResult UserProfileList(string deviceIdentifier)
        {
            if (!string.IsNullOrEmpty(deviceIdentifier))
            {
                UserInfo userInfo = new UserInfo(User);
                if (userInfo != null)
                {
                    PatientDetails detail = new PatientDetails();
                    var patient = detail.GetPatientDetailByRegistrationNumber(userInfo.RegistrationNumber);
                    var profileusers = detail.GetPatientLoginDetail(patient.LoginPin, patient.DeviceIdentityfier);
                    if (profileusers.Any())
                    {
                        return Ok(profileusers);
                    }
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                    Response<object> response = new Response<object>(errorDetail, null);
                    return Ok(response);
                }
            }

            return BadRequest();
        }
        private async Task SendMailForAppointment(PatientInfo info)
        {
            await Task.Run(() =>
            {
                //Send Email
                Message msg = new Message()
                {
                    MessageTo = info.Email,
                    MessageNameTo = info.FirstName + " " + info.MiddleName + (string.IsNullOrWhiteSpace(info.MiddleName) ? "" : " ") + info.LastName,
                    OTP = info.OTP,
                    Subject = "Verify Mobile Number",
                    Body = EmailHelper.GetDeviceVerificationEmail(info.FirstName, info.MiddleName, info.LastName, info.OTP)
                };
                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();

                //Send SMS
                msg.Body = "Hello " + string.Format("{0} {1}", info.FirstName, info.LastName) + "\nAs you requested, here is a OTP " + info.OTP + " you can use it to verify your mobile number before 15 minutes.\n Regards:\n Patient Portal(RMLHIMS)";
                msg.MessageTo = info.MobileNumber;
                msg.MessageType = MessageType.OTP;
                sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                sendMessageStrategy.SendMessages();
            });
        }
    }

}
