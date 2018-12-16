using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Masters;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
using PatientPortalApi.Models.User;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using static PatientPortalApi.Global.Enums;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/login")]
    public class LoginAPIController : ApiController
    {
        /// <summary>
        /// Get Patient List
        /// </summary>
        /// <returns>List of Patientinfo</returns>
        [Authorize]
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

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string state, string pincode, string religion, string department, string FatherHusbandName, string MaritalStatus, string title, string aadharNumber)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (mobilenumber.Trim().Length != 10)
            {
                return BadRequest(ErrorCode.InCorrectMobileNumber.ToString());
            }
            else if (!Regex.IsMatch(email, emailRegEx, RegexOptions.IgnoreCase))
            {
                return BadRequest(ErrorCode.InCorrectMobileNumber.ToString());
            }
            else
            {
                PatientDetails details = new PatientDetails();
                var patientInfo = details.GetPatientDetailByMobileNumberANDEmail(mobilenumber.Trim(), email.Trim());
                if (patientInfo != null)
                {
                    return BadRequest(ErrorCode.MobileOrEmailExists.ToString());
                }
                string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                PatientInfoModel pateintModel = getPatientInfoModelForSession(firstname, middlename, lastname, DOB, Gender, mobilenumber, email, address, city, country, pincode, religion, department, verificationCode, state, FatherHusbandName, 0, null, MaritalStatus, title, aadharNumber);
                if (pateintModel != null)
                {
                    SendMailFordeviceVerification(firstname, middlename, lastname, email, verificationCode, mobilenumber);
                    pateintModel.OTP = verificationCode;
                    return Ok(pateintModel);
                }
                else
                {
                    return BadRequest(ErrorCode.UserAlreadyExists.ToString());
                }
            }
        }

        private async Task SendMailFordeviceVerification(string firstname, string middlename, string lastname, string email, string verificationCode, string mobilenumber)
        {
            await Task.Run(() =>
            {
                //Send Email
                Message msg = new Message()
                {
                    MessageTo = email,
                    MessageNameTo = firstname + " " + middlename + (string.IsNullOrWhiteSpace(middlename) ? "" : " ") + lastname,
                    OTP = verificationCode,
                    Subject = "Verify Mobile Number",
                    Body = EmailHelper.GetDeviceVerificationEmail(firstname, middlename, lastname, verificationCode)
                };
                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();

                //Send SMS
                msg.Body = "Hello " + string.Format("{0} {1}", firstname, lastname) + "\nAs you requested, here is a OTP " + verificationCode + " you can use it to verify your mobile number.\n Regards:\n Patient Portal(RMLHIMS)";
                msg.MessageTo = mobilenumber;
                msg.MessageType = MessageType.OTP;
                sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                sendMessageStrategy.SendMessages();
            });
        }

        private static PatientInfoModel getPatientInfoModelForSession(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string pincode, string religion, string department, string verificationCode, string state, string FatherHusbandName, int patientId, byte[] image, string MaritalStatus, string title, string aadharNumber)
        {
            DepartmentDetails detail = new DepartmentDetails();
            var dept = detail.GetDeparmentById(Convert.ToInt32(department));
            int pinResult = 0;
            PatientInfoModel model = new PatientInfoModel
            {
                AadharNumber = aadharNumber,
                Address = address,
                CityId = city,
                Country = country,
                Department = dept != null ? dept.DepartmentName : string.Empty,
                DOB = Convert.ToDateTime(DOB),
                Email = email,
                FirstName = firstname,
                Gender = Gender,
                LastName = lastname,
                MiddleName = middlename,
                MobileNumber = mobilenumber,
                PinCode = int.TryParse(pincode, out pinResult) ? pinResult : 0,
                Religion = religion,
                StateId = state,
                FatherOrHusbandName = FatherHusbandName,
                DepartmentId = Convert.ToInt32(department),
                MaritalStatus = MaritalStatus,
                Title = title
            };
            return model;
        }

        [Route("logout")]
        [Authorize]
        public IHttpActionResult Logout()
        {
            return Ok();
        }
    }

}
