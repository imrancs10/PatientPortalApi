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
        [Route("sendemailforotp")]
        public IHttpActionResult SendEmailForOTP([FromBody]PatientRegisterModel model)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (model.mobilenumber.Trim().Length != 10)
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InCorrectMobileNumber];
                Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                return Ok(response);
            }
            else if (!Regex.IsMatch(model.email, emailRegEx, RegexOptions.IgnoreCase))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InCorrectEmailId];
                Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                return Ok(response);
            }
            else
            {
                PatientDetails details = new PatientDetails();
                var patientInfo = details.GetPatientDetailByMobileNumberANDEmail(model.mobilenumber.Trim(), model.email.Trim());
                if (patientInfo != null)
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.MobileOrEmailExists];
                    Response<PatientInfo> response = new Response<PatientInfo>(errorDetail, null);
                    return Ok(response);
                }
                else
                {
                    string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                    SendMailFordeviceVerification(model.firstname, model.middlename, model.lastname, model.email, verificationCode, model.mobilenumber);
                    return Ok(verificationCode);
                }
            }
        }

        [HttpPost]
        [Route("saveregistration")]
        public IHttpActionResult Register([FromBody]PatientRegisterModel model)
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
            bool parseDateFrom = DateTime.TryParse(DOB, out DateTime dtFrom);
            info.DOB = parseDateFrom ? dtFrom : DateTime.Now;
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
                msg.Body = "Hello " + string.Format("{0} {1}", firstname, lastname) + "\nAs you requested, here is a OTP " + verificationCode + " you can use it to verify your mobile number before 15 minutes.\n Regards:\n Patient Portal(RMLHIMS)";
                msg.MessageTo = mobilenumber;
                msg.MessageType = MessageType.OTP;
                sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                sendMessageStrategy.SendMessages();
            });
        }
        [Route("logout")]
        [Authorize]
        public IHttpActionResult Logout()
        {
            return Ok();
        }

        [HttpPost]
        [Route("crintegrate")]
        public IHttpActionResult CRIntegrate(string CRNumber)
        {
            PatientDetails details = new PatientDetails();
            var patientInfo = details.GetPatientDetailByRegistrationNumberOrCRNumber(CRNumber);

            if (patientInfo != null)
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.CRNumberExists];
                Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                return Ok(response);
            }

            var patientInfoClone = details.GetPatientCloneDetailByCRNumber(CRNumber);
            if (patientInfoClone != null)
            {
                PatientInfoModel crData = GetPatientInfoModelClone(patientInfoClone);
                return Ok(crData);
            }
            else
            {
                WebServiceIntegration service = new WebServiceIntegration();
                var patient = service.GetPatientInfoBYCRNumber(CRNumber);
                if (patient != null)
                {
                    PatientInfoModel crData = GetPatientInfoModel(patient);
                    TimeSpan ageDiff = DateTime.Now.Subtract(Convert.ToDateTime(patient.DoR));
                    crData.DOB = crData.DOB.Value.Add(ageDiff);
                    if (crData.LastName == string.Empty && !string.IsNullOrEmpty(crData.MiddleName))
                    {
                        crData.LastName = crData.MiddleName;
                        crData.MiddleName = string.Empty;
                    }
                    //Save CR Patient Data to Patient Clone table when data comes from web service
                    Dictionary<string, object> result = SavePatientInfo(crData.MaritalStatus, crData.Title, crData.FirstName, crData.MiddleName, crData.LastName, Convert.ToDateTime(crData.DOB).ToShortDateString(), crData.Gender, crData.MobileNumber, crData.Email, crData.Address, crData.CityId, crData.Country, Convert.ToString(crData.PinCode), crData.Religion, Convert.ToString(crData.DepartmentId), "", crData.StateId, crData.FatherOrHusbandName, 0, null, crData.AadharNumber, true, crData.Pid, crData.Location);
                    if (result["status"].ToString() == CrudStatus.Saved.ToString())
                    {
                        string serialNumber = VerificationCodeGeneration.GetSerialNumber();
                        PatientInfoCRClone info = new PatientInfoCRClone()
                        {
                            RegistrationNumber = serialNumber,
                            CRNumber = !string.IsNullOrEmpty(Convert.ToString(crData.CRNumber)) ? Convert.ToString(crData.CRNumber) : string.Empty,
                            PatientId = ((PatientInfoCRClone)result["data"]).PatientId,
                            ValidUpto = crData.ValidUpto
                        };
                        PatientDetails _details = new PatientDetails();
                        info = _details.UpdatePatientDetailClone(info);
                    }
                    else if (result["status"].ToString() == CrudStatus.DataAlreadyExist.ToString())
                    {
                        ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.EmailIdExists];
                        Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                        return Ok(response);
                    }
                    return Ok(crData);
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.CRNumberNotFoundOrExpire];
                    Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                    return Ok(response);
                }
            }
        }
        [HttpPost]
        [Route("savecrintegrate")]
        public IHttpActionResult SubmitCRDetail([FromBody]PatientInfoModelCR crData)//(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string state, string pincode, string religion, string department, string FatherHusbandName, string title, string MaritalStatus, string aadharNumber)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(crData.Email, emailRegEx, RegexOptions.IgnoreCase))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.WrongEmailAddress];
                Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                return Ok(response);
            }
            else
            {
                //var crData = (PatientInfoModel)Session["crData"];
                PatientDetails _details = new PatientDetails();
                var existingPatient = _details.GetPatientDetailByMobileNumberANDEmail(crData.MobileNumber, crData.Email);
                if (existingPatient != null)
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.EmailOrMobileNoExists];
                    Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                    return Ok(response);
                }
                Dictionary<string, object> result = SavePatientInfo(crData.MaritalStatus, crData.Title, crData.FirstName, crData.MiddleName, crData.LastName, Convert.ToString(crData.DOB), crData.Gender, crData.MobileNumber, crData.Email, crData.Address, crData.CityId, crData.Country, Convert.ToString(crData.PinCode), crData.Religion, Convert.ToString(crData.DepartmentId), "", crData.StateId, crData.FatherOrHusbandName, 0, null, crData.AadharNumber, false, crData.Pid, crData.Location);
                if (result["status"].ToString() == CrudStatus.Saved.ToString())
                {
                    string serialNumber = VerificationCodeGeneration.GetSerialNumber();
                    bool parseDateFrom = DateTime.TryParse(crData.ValidUpto, out DateTime dtFrom);
                    PatientInfo info = new PatientInfo()
                    {
                        RegistrationNumber = serialNumber,
                        CRNumber = !string.IsNullOrEmpty(Convert.ToString(crData.CRNumber)) ? Convert.ToString(crData.CRNumber) : string.Empty,
                        PatientId = ((PatientInfo)result["data"]).PatientId,
                        ValidUpto = parseDateFrom ? dtFrom : DateTime.Now.AddMonths(6)
                    };

                    info = _details.UpdatePatientDetail(info);
                    SendMailTransactionResponse(serialNumber, info, true);
                    _details.DeletePatientInfoCRData(crData.CRNumber);
                    return Ok("Succesfully Save");
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.UserNotSaved];
                    Response<JwtResponse> response = new Response<JwtResponse>(errorDetail, null);
                    return Ok(response);
                }
            }
        }

        private PatientInfoModel GetPatientInfoModel(HISPatientInfoModel patient)
        {
            int pin = 0;
            var crData = new PatientInfoModel()
            {
                FirstName = patient.Firstname != "N/A" ? patient.Firstname : string.Empty,
                MiddleName = patient.Middlename != "N/A" ? patient.Middlename : string.Empty,
                LastName = patient.Lastname != "N/A" ? patient.Lastname : string.Empty,
                DOB = !string.IsNullOrEmpty(patient.Age) ? DateTime.Now.AddYears(-Convert.ToInt32(patient.Age)) : DateTime.Now,
                Gender = patient.Gender == "F" ? "Female" : "Male",
                MobileNumber = patient.Mobileno != "N/A" ? patient.Mobileno : string.Empty,
                Email = patient.Email != "N/A" ? patient.Email : string.Empty,
                Address = patient.Address != "N/A" ? patient.Address : string.Empty,
                CityId = patient.City != "N/A" ? GetCityIdByCItyName(patient.City) : string.Empty,
                Country = patient.Country != "N/A" ? patient.Country : string.Empty,
                PinCode = int.TryParse(patient.Pincode, out pin) ? pin : 0,
                Religion = patient.Religion != "N/A" ? patient.Religion : string.Empty,
                DepartmentId = patient.deptid,
                StateId = patient.State != "N/A" ? GetStateIdByStateName(patient.State) : string.Empty,
                FatherOrHusbandName = patient.FatherOrHusbandName != "N/A" ? patient.FatherOrHusbandName : string.Empty,
                CRNumber = patient.Registrationnumber != "N/A" ? patient.Registrationnumber : string.Empty,
                Title = patient.Title != "N/A" ? patient.Title : string.Empty,
                AadharNumber = patient.AadharNo != "N/A" ? patient.AadharNo : string.Empty,
                MaritalStatus = patient.MaritalStatus != "N/A" ? patient.MaritalStatus : string.Empty,
                DoR = patient.DoR != "N/A" ? patient.DoR : string.Empty,
                ValidUpto = patient.ValidUpto != "N/A" ? Convert.ToDateTime(patient.ValidUpto) : Convert.ToDateTime(patient.DoR).AddMonths(Convert.ToInt32(ConfigurationManager.AppSettings["RegistrationValidityInMonth"])),
                Pid = patient.Pid != "N/A" ? patient.Pid : string.Empty,
                Location = patient.Location != "N/A" ? patient.Location : string.Empty,
                CityName = patient.City,
                StateName = patient.State
            };
            return crData;
        }
        private PatientInfoModel GetPatientInfoModelClone(PatientInfoCRClone patient)
        {
            int pin = 0;
            var crData = new PatientInfoModel()
            {
                CityName = patient.City.CityName,
                StateName = patient.State.StateName,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                DOB = patient.DOB,
                Gender = patient.Gender,
                MobileNumber = patient.MobileNumber,
                Email = patient.Email,
                Address = patient.Address,
                CityId = Convert.ToString(patient.CityId),
                Country = patient.Country,
                PinCode = patient.PinCode,
                Religion = patient.Religion,
                DepartmentId = patient.DepartmentId.Value,
                StateId = Convert.ToString(patient.StateId),
                FatherOrHusbandName = patient.FatherOrHusbandName,
                CRNumber = patient.CRNumber,
                Title = patient.Title,
                AadharNumber = patient.AadharNumber,
                MaritalStatus = patient.MaritalStatus,
                ValidUpto = patient.ValidUpto,
                Pid = Convert.ToString(patient.pid),
                Location = patient.Location
            };
            return crData;
        }

        private string GetStateIdByStateName(string stateName)
        {
            PatientDetails _details = new PatientDetails();
            return Convert.ToString(_details.GetStateIdByStateName(stateName)?.StateId);
        }

        private string GetCityIdByCItyName(string cityName)
        {
            PatientDetails _details = new PatientDetails();
            return Convert.ToString(_details.GetCityIdByCItyName(cityName)?.CityId);
        }
    }

}
