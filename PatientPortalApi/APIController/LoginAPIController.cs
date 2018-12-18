using DataLayer;
using PatientPortal.Infrastructure.Utility;
using PatientPortalApi.BAL.Masters;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Adapter.WebService;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
using PatientPortalApi.Models.Patient;
using PatientPortalApi.Models.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        [Route("CheckRegisterValidation")]
        public IHttpActionResult CheckRegistration(string mobilenumber, string email)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (mobilenumber.Trim().Length != 10)
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InCorrectMobileNumber];
                Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                return Ok(response);
            }
            else if (!Regex.IsMatch(email, emailRegEx, RegexOptions.IgnoreCase))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InCorrectEmailId];
                Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                return Ok(response);
            }
            else
            {
                PatientDetails details = new PatientDetails();
                var patientInfo = details.GetPatientDetailByMobileNumberANDEmail(mobilenumber.Trim(), email.Trim());
                if (patientInfo != null)
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.MobileOrEmailExists];
                    Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                    return Ok(response);
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("saveregistration")]
        public IHttpActionResult Register(PatientRegisterModel model)
        {
            string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
            Dictionary<string, object> result = SavePatientInfo(model.MaritalStatus, model.title, model.firstname, model.middlename, model.lastname, model.DOB.ToString(), model.Gender, model.mobilenumber, model.email, model.address, model.city, model.country, model.pincode.ToString(), model.religion, model.department.ToString(), "", model.state, model.FatherHusbandName, 0, null, model.aadharNumber);
            if (result["status"].ToString() == CrudStatus.Saved.ToString())
            {
                int patientId = ((PatientInfo)result["data"]).PatientId;
                string serialNumber = VerificationCodeGeneration.GetSerialNumber();
                PatientDetails _details = new PatientDetails();
                PatientInfo info = new PatientInfo()
                {
                    RegistrationNumber = serialNumber,
                    PatientId = patientId
                };
                info = _details.UpdatePatientDetail(info);
                PatientTransaction transaction = new PatientTransaction()
                {
                    PatientId = patientId,
                    Amount = Convert.ToInt32(model.Amount),
                    OrderId = model.OrderId,
                    ResponseCode = model.ResponseCode,
                    StatusCode = model.StatusCode,
                    TransactionDate = Convert.ToDateTime(model.TransactionDate),
                    TransactionNumber = model.TransactionNumber,
                    Type = TransactionType.Register.ToString()
                };
                var transactionData = _details.SavePatientTransaction(transaction);
                info.PatientTransactions.Add((PatientTransaction)transactionData["data"]);
                SendMailTransactionResponse(serialNumber, ((PatientInfo)result["data"]));
                transaction.OrderId = serialNumber;
                //send patient data to HIS portal
                HISPatientInfoInsertModel insertModel = setregistrationModelForHISPortal(info);
                insertModel.Type = Convert.ToInt32(TransactionType.Register);
                WebServiceIntegration service = new WebServiceIntegration();
                string serviceResult = service.GetPatientInfoinsert(insertModel);

                if (serviceResult.Contains("-"))
                {
                    var pidLocation = serviceResult.Split('-');
                    if (pidLocation.Length == 2)
                    {
                        int pId = Convert.ToInt32(pidLocation[0]);
                        string location = Convert.ToString(pidLocation[1]);
                        PatientInfo infoPatient = new PatientInfo()
                        {
                            pid = pId,
                            Location = location,
                            PatientId = patientId
                        };
                        info = _details.UpdatePatientDetail(infoPatient);
                    }
                }

                //save status to DB
                PatientInfo user = new PatientInfo()
                {
                    PatientId = patientId,
                    RegistrationStatusHIS = serviceResult.Contains("-") ? "S" : serviceResult,


                };
                _details.UpdatePatientHISSyncStatus(info);
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.DataAlreadyExist];
                Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                return Ok(response);
            }

            return Ok();
        }


        private static Dictionary<string, object> SavePatientInfo(string MaritalStatus, string Title, string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string pincode, string religion, string department, string verificationCode, string state, string FatherHusbandName, int patientId, byte[] image, string aadharNumber, bool IsClone = false, string pid = null, string location = null)
        {
            PatientDetails _details = new PatientDetails();
            int pinResult = 0;
            dynamic info;
            if (IsClone == false)
                info = new PatientInfo();
            else
                info = new PatientInfoCRClone();

            info.AadharNumber = aadharNumber;
            info.FirstName = firstname;
            info.MiddleName = middlename;
            info.LastName = lastname;
            if (!string.IsNullOrEmpty(DOB))
                info.DOB = Convert.ToDateTime(DOB);
            info.Gender = Gender;
            info.MobileNumber = mobilenumber;
            info.Email = email;
            info.Address = address;
            info.Country = country;
            info.PinCode = int.TryParse(pincode, out pinResult) ? pinResult : 0;
            info.Religion = religion;
            info.OTP = verificationCode;
            info.FatherOrHusbandName = FatherHusbandName;
            info.MaritalStatus = MaritalStatus;
            info.Title = Title;
            info.pid = Convert.ToDecimal(pid);
            info.Location = location;

            if (!string.IsNullOrEmpty(city))
                info.CityId = Convert.ToInt32(city);
            else
                info.CityId = null;
            if (!string.IsNullOrEmpty(state))
                info.StateId = Convert.ToInt32(state);
            else
                info.StateId = null;
            if (!string.IsNullOrEmpty(department))
                info.DepartmentId = Convert.ToInt32(department);
            else
                info.DepartmentId = null;

            if (patientId > 0)
                info.PatientId = patientId;
            if (image != null && image.Length > 0)
                info.Photo = image;
            Dictionary<string, object> result;
            if (IsClone == false)
                result = _details.CreateOrUpdatePatientDetail(info);
            else
                result = _details.CreateOrUpdatePatientDetailClone(info);
            return result;
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

        private async Task SendMailTransactionResponse(string serialNumber, PatientInfo info, bool isclone = false)
        {
            await Task.Run(() =>
            {
                string passwordCreateURL = "Home/CreatePassword?id=" + CryptoEngine.Encrypt(serialNumber);
                string baseUrl = Convert.ToString(ConfigurationManager.AppSettings["PatientPortaWebsiteUrl"]);

                Message msg = new Message()
                {
                    MessageTo = info.Email,
                    MessageNameTo = info.FirstName + " " + info.MiddleName + (string.IsNullOrWhiteSpace(info.MiddleName) ? "" : " ") + info.LastName,
                    Subject = "Registration Created",
                    Body = EmailHelper.GetRegistrationSuccessEmail(info.FirstName, info.MiddleName, info.LastName, serialNumber, baseUrl + passwordCreateURL)
                };

                if (isclone)
                    msg.Body = EmailHelper.GetRegistrationCRSuccessEmail(info.FirstName, info.MiddleName, info.LastName, serialNumber, baseUrl + passwordCreateURL);

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }

        public static HISPatientInfoInsertModel setregistrationModelForHISPortal(PatientInfo info)
        {
            return new HISPatientInfoInsertModel()
            {
                Address = info.Address,
                City = info.City != null ? info.City.CityName : string.Empty,
                CRNumber = info.CRNumber,
                DepartmentId = info.DepartmentId != null ? Convert.ToString(info.DepartmentId.Value) : null,
                DOB = info.DOB != null ? info.DOB.Value.ToString("yyyy-MM-dd") : string.Empty,
                Email = info.Email,
                FatherOrHusbandName = info.FatherOrHusbandName,
                FirstName = info.FirstName,
                Gender = info.Gender,
                LastName = info.LastName,
                MaritalStatus = info.MaritalStatus,
                MiddleName = info.MiddleName,
                MobileNumber = info.MobileNumber,
                Password = info.Password,
                PatientId = info.PatientId,
                PinCode = Convert.ToString(info.PinCode),
                RegistrationNumber = info.RegistrationNumber,
                Religion = info.Religion,
                State = info.State != null ? info.State.StateName : string.Empty,
                Title = info.Title,
                ValidUpto = Convert.ToString(info.ValidUpto.Value.ToString("yyyy-MM-dd")),
                CreateDate = Convert.ToString(info.PatientTransactions.FirstOrDefault().TransactionDate.Value.ToString("yyyy-MM-dd")),
                Amount = Convert.ToString(info.PatientTransactions.FirstOrDefault().Amount),
                PatientTransactionId = Convert.ToString(info.PatientTransactions.FirstOrDefault().PatientTransactionId),
                TransactionNumber = Convert.ToString(info.PatientTransactions.FirstOrDefault().TransactionNumber)
            };
        }

        [Route("logout")]
        [Authorize]
        public IHttpActionResult Logout()
        {
            return Ok();
        }
    }

}
