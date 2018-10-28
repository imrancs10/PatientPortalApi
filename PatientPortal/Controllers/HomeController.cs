using com.awl.MerchantToolKit;
using DataLayer;
using PatientPortal.BAL.Masters;
using PatientPortal.BAL.Patient;
using PatientPortal.Infrastructure;
using PatientPortal.Infrastructure.Adapter.WebService;
using PatientPortal.Infrastructure.Authentication;
using PatientPortal.Infrastructure.Utility;
using PatientPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using static PatientPortal.Global.Enums;

namespace PatientPortal.Controllers
{
    public class HomeController : CommonController
    {
        [CustomAuthorize]
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Index()
        {
            DepartmentDetails _details = new DepartmentDetails();
            var result = _details.DepartmentList();
            ViewData["Departments"] = result;
            var opdDetail = (new WebServiceIntegration()).GetPatientOPDDetail("0");
            ViewData["PDDetail"] = opdDetail;
            return View();
        }

        [HttpPost]
        public ActionResult GetPatientLogin(string username, string password)
        {
            PatientDetails _details = new PatientDetails();
            var result = _details.GetPatientDetail(username, password);
            var patientInfo = ((PatientInfo)result["data"]);
            var msg = (CrudStatus)result["status"];
            if (msg == CrudStatus.RegistrationExpired)
            {
                Session["PatientInfoRenewal"] = patientInfo;
                SetAlertMessage("Registration Expired, Kindly renew it.", "Login");

                string daysRemaning = Convert.ToString((patientInfo.ValidUpto.Value.Date - DateTime.Now.Date).TotalDays);
                if (Convert.ToInt32(daysRemaning) < 0)
                {
                    //expired registration
                    TempData["Expired"] = true;
                    return RedirectToAction("TransactionPayReNewalExpired");
                }
                else
                {
                    TempData["Expired"] = false;
                    return RedirectToAction("TransactionPayReNewal");
                }
            }
            if (patientInfo != null)
            {
                Session["PatientId"] = patientInfo.PatientId;
                setUserClaim(patientInfo);
                SaveLoginHistory(patientInfo.PatientId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                var registrationResult = _details.GetPatientDetailByRegistrationNumber(username);
                if (registrationResult == null)
                {
                    SetAlertMessage("User Not Found", "Login");
                }
                else
                {
                    PatientLoginEntry entry = new PatientLoginEntry
                    {
                        PatientId = registrationResult.PatientId,
                        LoginAttemptDate = DateTime.Now
                    };
                    var loginAttempt = _details.SavePatientLoginFailedHistory(entry);
                    if (loginAttempt.LoginAttempt == 4)
                    {
                        SetAlertMessage("You have reached the maximum attempt, your account is locked for a day.", "Login");
                    }
                    else
                    {
                        SetAlertMessage("Wrong Password, only " + (4 - loginAttempt.LoginAttempt).ToString() + " Attempt left, else your account will be locked for a day.", "Login");
                    }
                }

                return View("Index");
            }
        }

        public ActionResult Register(string actionName)
        {
            if (actionName == "getotpscreen")
            {
                if (Session["PatientInfo"] != null)
                {
                    ViewData["PatientData"] = Session["PatientInfo"] as PatientInfoModel;
                }

                ViewData["registerAction"] = "getotpscreen";
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetPatientOTP(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string state, string pincode, string religion, string department, string FatherHusbandName, string MaritalStatus, string title, string aadharNumber)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (mobilenumber.Trim().Length != 10)
            {
                SetAlertMessage("Please Enter correct Mobile Number", "Register");
                return RedirectToAction("Register");
            }
            else if (!Regex.IsMatch(email, emailRegEx, RegexOptions.IgnoreCase))
            {
                SetAlertMessage("Please Enter correct Email Address", "Register");
                return RedirectToAction("Register");
            }
            else
            {
                PatientDetails details = new PatientDetails();
                var patientInfo = details.GetPatientDetailByMobileNumberANDEmail(mobilenumber.Trim(), email.Trim());
                if (patientInfo != null)
                {
                    SetAlertMessage("Mobile Number or Email Id already in our database, kindly chhange it or reset your account.", "Register");
                    return RedirectToAction("Register");
                }
                string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                PatientInfoModel pateintModel = getPatientInfoModelForSession(firstname, middlename, lastname, DOB, Gender, mobilenumber, email, address, city, country, pincode, religion, department, verificationCode, state, FatherHusbandName, 0, null, MaritalStatus, title, aadharNumber);
                if (pateintModel != null)
                {
                    SendMailFordeviceVerification(firstname, middlename, lastname, email, verificationCode,mobilenumber);
                    Session["otp"] = verificationCode;
                    //Session["PatientId"] = ((PatientInfo)result["data"]).PatientId;
                    Session["PatientInfo"] = pateintModel;
                    return RedirectToAction("Register", new { actionName = "getotpscreen" });
                }
                else
                {
                    SetAlertMessage("User is already register", "Register");
                    return RedirectToAction("Register");
                }
            }
        }

        private async Task SendMailFordeviceVerification(string firstname, string middlename, string lastname, string email, string verificationCode,string mobilenumber)
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

        [HttpPost]
        public ActionResult verifyOTP(string OTP)
        {
            if (Convert.ToString(Session["otp"]) == OTP)
            {
                return PaymentTransaction();
            }
            else
            {
                SetAlertMessage("OTP not matched", "Register");
                return RedirectToAction("Register", new { actionName = "getotpscreen" });
            }
        }

        public ActionResult PaymentTransaction()
        {
            string MerchantId = Convert.ToString(ConfigurationManager.AppSettings["MerchantId"]);
            string EncryptKey = Convert.ToString(ConfigurationManager.AppSettings["EncryptKey"]);
            string TransactionAmount = Convert.ToString(ConfigurationManager.AppSettings["TransactionAmount"]);
            string ResponseUrl = Convert.ToString(ConfigurationManager.AppSettings["ResponseUrl"]);
            string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            try
            {
                ReqMsgDTO objReqMsgDTO;
                objReqMsgDTO = new ReqMsgDTO();
                objReqMsgDTO.OrderId = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                objReqMsgDTO.Mid = MerchantId;
                objReqMsgDTO.Enckey = EncryptKey;
                objReqMsgDTO.MeTransReqType = "S";
                objReqMsgDTO.TrnAmt = TransactionAmount;
                objReqMsgDTO.RecurrPeriod = "";
                objReqMsgDTO.RecurrDay = "";
                objReqMsgDTO.ResponseUrl = baseUrl + ResponseUrl;
                objReqMsgDTO.TrnRemarks = "Test";
                objReqMsgDTO.TrnCurrency = "INR";
                objReqMsgDTO.AddField1 = "";
                objReqMsgDTO.AddField2 = "";
                objReqMsgDTO.AddField3 = "";
                objReqMsgDTO.AddField4 = "";
                objReqMsgDTO.AddField5 = "";
                objReqMsgDTO.AddField6 = "";
                objReqMsgDTO.AddField7 = "";
                objReqMsgDTO.AddField8 = "";
                string Message;
                AWLMEAPI objawlmerchantkit = new AWLMEAPI();
                objawlmerchantkit.generateTrnReqMsg(objReqMsgDTO);
                Message = objReqMsgDTO.ReqMsg;
                Session["Message"] = Message;
                Session["MID"] = objReqMsgDTO.Mid;
                return RedirectToAction("TransactionPay");
            }
            catch (Exception ex)
            {
                SetAlertMessage("There Was Some Error Processing.....Please Check The Data you have Entered", "Transaction");
                return RedirectToAction("Register");
            }
        }

        public ActionResult TransactionPay()
        {
            return View();
        }
        public ActionResult TransactionPayReNewal()
        {
            return View();
        }
        public ActionResult TransactionPayReNewalExpired()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreatePassword(string registrationNumber)
        {
            PatientDetails _details = new PatientDetails();
            var result = _details.GetPatientDetailByRegistrationNumber(registrationNumber);
            if (result != null)
            {
                if (!string.IsNullOrEmpty(result.Password))
                {
                    SetAlertMessage("You have already created your Password, please Login or use forget password to re-generate password.", "password Create");
                    return RedirectToAction("Index");
                }
            }
            ViewData["registrationNumber"] = registrationNumber;
            return View();
        }

        [HttpPost]
        public ActionResult CreatePassword(string password, string confirmpassword, string registrationNumber)
        {
            if (password.Trim() != confirmpassword.Trim())
            {
                SetAlertMessage("Password and Confirm Password are not match", "password Create");
                return View();
            }
            else
            {
                PatientDetails _details = new PatientDetails();
                var result = _details.GetPatientDetailByRegistrationNumber(registrationNumber);
                if (result != null)
                {
                    result.Password = password.Trim();
                    _details.UpdatePatientDetail(result);
                    SetAlertMessage("Password Created Successfully, please login.", "Password Create");
                    return RedirectToAction("Index");
                }
                else
                {
                    SetAlertMessage("Your Registration Number is Incorrect,Kindly contact the administrator", "password Create");
                    return View();
                }
            }
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult ResetPassword(string resetCode)
        {
            ViewData["resetCode"] = resetCode;
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult ResetPassword(string password, string confirmpassword, string resetCode)
        {
            if (password.Trim() != confirmpassword.Trim())
            {
                SetAlertMessage("Password and Confirm Password are not match", "password Create");
                return View();
            }
            else
            {
                PatientDetails _details = new PatientDetails();
                var result = _details.GetPatientDetailByresetCode(resetCode);
                if (result != null)
                {
                    result.Password = password.Trim();
                    result.ResetCode = "";
                    _details.UpdatePatientDetail(result);
                    SetAlertMessage("Password Created Successfully, please login.", "Password Create");
                    return RedirectToAction("Index");
                }
                else
                {
                    SetAlertMessage("Your reset password link is already used,Kindly initiate another request for Forget Password.", "password Create");
                    return View();
                }
            }
        }
        public ActionResult TransactionResponse()
        {
            string EncryptKey = Convert.ToString(ConfigurationManager.AppSettings["EncryptKey"]);
            ResMsgDTO objResMsgDTO = new ResMsgDTO();
            if (Request.Form["merchantResponse"] != null)
            {
                string merchantResponse = Request.Form["merchantResponse"];
                AWLMEAPI transact = new AWLMEAPI();
                objResMsgDTO = transact.parseTrnResMsg(merchantResponse, EncryptKey);

                if (objResMsgDTO.ResponseCode == Convert.ToString(ConfigurationManager.AppSettings["TransactionFailedResponseCode"]))
                {
                    ViewData["FailTransaction"] = true;
                    return View();
                }
                //renewal 
                if (Session["PatientInfoRenewal"] != null)
                {
                    var info = (PatientInfo)Session["PatientInfoRenewal"];
                    PatientDetails _details = new PatientDetails();
                    info = _details.UpdatePatientValidity(info);
                    PatientTransaction transaction = new PatientTransaction()
                    {
                        PatientId = info.PatientId,
                        Amount = Convert.ToInt32(objResMsgDTO.TrnAmt),
                        OrderId = objResMsgDTO.OrderId,
                        ResponseCode = objResMsgDTO.ResponseCode,
                        StatusCode = objResMsgDTO.StatusCode,
                        TransactionDate = Convert.ToDateTime(objResMsgDTO.TrnReqDate),
                        TransactionNumber = objResMsgDTO.PgMeTrnRefNo
                    };
                    var transactionData = _details.SavePatientTransaction(transaction);
                    info.PatientTransactions.Add((PatientTransaction)transactionData["data"]);
                    SendMailTransactionResponseRegistrationRenewal(info.RegistrationNumber, info, transaction);
                    transaction.OrderId = info.RegistrationNumber;
                    Session["PatientInfoRenewal"] = null;
                    TempData["transaction"] = transaction;
                    //send patient renewal data to HIS Database
                    //HISPatientInfoInsertModel insertModel = setregistrationModelForHISPortal(info);
                    //WebServiceIntegration service = new WebServiceIntegration();
                    //string serviceResult = service.GetPatientInfoinsert(insertModel);
                    if (Convert.ToBoolean(TempData["Expired"]) == true)
                    {
                        return RedirectToAction("TransactionResponseRenewalExpired");
                    }
                    else if (Convert.ToBoolean(TempData["Expired"]) == false)
                    {
                        return RedirectToAction("TransactionResponseRenewal");
                    }
                }
                else
                {
                    PatientDetails _details = new PatientDetails();
                    string serialNumber = VerificationCodeGeneration.GetSerialNumber();
                    if (Session["PatientInfo"] != null)
                    {
                        PatientInfoModel model = Session["PatientInfo"] as PatientInfoModel;
                        Dictionary<string, object> result = SavePatientInfo(model.MaritalStatus, model.Title, model.FirstName, model.MiddleName, model.LastName, model.DOB.ToString(), model.Gender, model.MobileNumber, model.Email, model.Address, model.CityId, model.Country, model.PinCode.ToString(), model.Religion, model.DepartmentId.ToString(), "", model.StateId, model.FatherOrHusbandName, 0, null, model.AadharNumber);
                        if (result["status"].ToString() == CrudStatus.Saved.ToString())
                        {
                            int patientId = ((PatientInfo)result["data"]).PatientId;
                            PatientInfo info = new PatientInfo()
                            {
                                RegistrationNumber = serialNumber,
                                PatientId = patientId
                            };
                            info = _details.UpdatePatientDetail(info);
                            PatientTransaction transaction = new PatientTransaction()
                            {
                                PatientId = patientId,
                                Amount = Convert.ToInt32(objResMsgDTO.TrnAmt),
                                OrderId = objResMsgDTO.OrderId,
                                ResponseCode = objResMsgDTO.ResponseCode,
                                StatusCode = objResMsgDTO.StatusCode,
                                TransactionDate = Convert.ToDateTime(objResMsgDTO.TrnReqDate),
                                TransactionNumber = objResMsgDTO.PgMeTrnRefNo
                            };
                            var transactionData = _details.SavePatientTransaction(transaction);
                            info.PatientTransactions.Add((PatientTransaction)transactionData["data"]);
                            SendMailTransactionResponse(serialNumber, ((PatientInfo)result["data"]));
                            transaction.OrderId = serialNumber;
                            ViewData["TransactionSuccessResult"] = transaction;
                            Session["PatientInfo"] = null;
                            //send patient data to HIS portal
                            HISPatientInfoInsertModel insertModel = setregistrationModelForHISPortal(info);
                            WebServiceIntegration service = new WebServiceIntegration();
                            string serviceResult = service.GetPatientInfoinsert(insertModel);
                        }
                    }
                    else
                    {
                        SetAlertMessage("There Was Some Error in transaction Processing.....Please Check The Data you have Entered", "Transaction");
                    }
                }
                return View();
            }
            else
            {
                return View();
            }
        }
        public ActionResult TransactionResponseRenewal()
        {
            PatientTransaction transaction = TempData["transaction"] as PatientTransaction;
            ViewData["TransactionSuccessResult"] = transaction;
            TempData["Expired"] = null;
            return View();
        }
        public ActionResult TransactionResponseRenewalExpired()
        {
            PatientTransaction transaction = TempData["transaction"] as PatientTransaction;
            ViewData["TransactionSuccessResult"] = transaction;
            TempData["Expired"] = null;
            return View();
        }

        private static HISPatientInfoInsertModel setregistrationModelForHISPortal(PatientInfo info)
        {
            return new HISPatientInfoInsertModel()
            {
                Address = info.Address,
                City = info.City.CityName,
                CRNumber = info.CRNumber,
                DepartmentId = Convert.ToString(info.DepartmentId.Value),
                //DOB = Convert.ToString(info.DOB.ToString("yyyy-MM-dd")),
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
                State = info.State.StateName,
                Title = info.Title,
                ValidUpto = Convert.ToString(info.ValidUpto.Value.ToString("yyyy-MM-dd")),
                CreateDate = Convert.ToString(info.PatientTransactions.FirstOrDefault().TransactionDate.Value.ToString("yyyy-MM-dd")),
                Amount = Convert.ToString(info.PatientTransactions.FirstOrDefault().Amount),
                PatientTransactionId = Convert.ToString(info.PatientTransactions.FirstOrDefault().TransactionNumber)
            };
        }

        private async Task SendMailTransactionResponse(string serialNumber, PatientInfo info)
        {
            await Task.Run(() =>
            {
                string passwordCreateURL = "Home/CreatePassword?registrationNumber=" + serialNumber;
                string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));

                Message msg = new Message()
                {
                    MessageTo = info.Email,
                    MessageNameTo = info.FirstName + " " + info.MiddleName + (string.IsNullOrWhiteSpace(info.MiddleName) ? "" : " ") + info.LastName,
                    Subject = "Registration Created",
                    Body = EmailHelper.GetRegistrationSuccessEmail(info.FirstName, info.MiddleName, info.LastName, serialNumber, baseUrl + passwordCreateURL)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }

        private async Task SendMailTransactionResponseRegistrationRenewal(string serialNumber, PatientInfo info, PatientTransaction transaction)
        {
            await Task.Run(() =>
            {
                Message msg = new Message()
                {
                    MessageTo = info.Email,
                    MessageNameTo = info.FirstName + " " + info.MiddleName + (string.IsNullOrWhiteSpace(info.MiddleName) ? "" : " ") + info.LastName,
                    Subject = "Registration Renew",
                    Body = EmailHelper.GetRegistrationSuccessEmailRenew(info.FirstName, info.MiddleName, info.LastName, transaction)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }
        private void setUserClaim(PatientInfo info)
        {
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
            serializeModel.Id = info.PatientId;
            serializeModel.FirstName = string.IsNullOrEmpty(info.FirstName) ? string.Empty : info.FirstName;
            serializeModel.MiddleName = string.IsNullOrEmpty(info.MiddleName) ? string.Empty : info.MiddleName;
            serializeModel.LastName = string.IsNullOrEmpty(info.LastName) ? string.Empty : info.LastName;
            serializeModel.Email = string.IsNullOrEmpty(info.Email) ? string.Empty : info.Email;
            serializeModel.DOB = info.DOB == null ? DateTime.MinValue : info.DOB;
            serializeModel.Gender = string.IsNullOrEmpty(info.Gender) ? string.Empty : info.Gender;
            serializeModel.Mobile = string.IsNullOrEmpty(info.MobileNumber) ? string.Empty : info.MobileNumber;
            serializeModel.Address = string.IsNullOrEmpty(info.Address) ? string.Empty : info.Address;
            serializeModel.City = string.IsNullOrEmpty(Convert.ToString(info.City)) ? string.Empty : Convert.ToString(info.City);
            serializeModel.State = string.IsNullOrEmpty(Convert.ToString(info.State)) ? string.Empty : Convert.ToString(info.State);
            serializeModel.Country = string.IsNullOrEmpty(info.Country) ? string.Empty : info.Country;
            serializeModel.PINCode = string.IsNullOrEmpty(info.PinCode.ToString()) ? string.Empty : info.PinCode.ToString();
            serializeModel.RegistrationNo = info.RegistrationNumber;
            serializeModel.Religion = info.Religion;
            serializeModel.Department = info.Department != null ? info.Department.DepartmentName : "";

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(serializeModel);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     info.Email,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(15),
                     false,
                     userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [CustomAuthorize]
        public ActionResult MyProfile(string actionName)
        {
            if (!string.IsNullOrEmpty(actionName))
            {
                ViewData["Action"] = "Edit";
            }
            if (User == null)
            {
                SetAlertMessage("User has been logged out", "Update Profile");
                return RedirectToAction("Index");
            }
            var patient = GetPatientInfo(User.Id);
            if (patient != null)
            {
                User.FirstName = patient.FirstName;
                User.MiddleName = patient.MiddleName;
                User.LastName = patient.LastName;
                User.Email = patient.Email;
                ViewData["PatientData"] = patient;
            }
            else
            {
                SetAlertMessage("User not found", "Update Profile");
                return RedirectToAction("Index");
            }
            return View();
        }

        private PatientInfoModel GetPatientInfo(int userId)
        {
            PatientDetails _details = new PatientDetails();
            var result = _details.GetPatientDetailById(userId);
            PatientInfoModel model = new PatientInfoModel
            {
                RegistrationNumber = result.RegistrationNumber,
                Address = result.Address,
                CityId = Convert.ToString(result.CityId),
                Country = result.Country,
                Department = result.Department.DepartmentName,
                DOB = result.DOB,
                Email = result.Email,
                FirstName = result.FirstName,
                Gender = result.Gender,
                LastName = result.LastName,
                MiddleName = result.MiddleName,
                MobileNumber = result.MobileNumber,
                PinCode = result.PinCode,
                Religion = result.Religion,
                StateId = Convert.ToString(result.StateId),
                Photo = result.Photo,
                FatherOrHusbandName = result.FatherOrHusbandName,
                MaritalStatus = result.MaritalStatus,
                ValidUpto = result.ValidUpto,
                Title = result.Title,
                AadharNumber = result.AadharNumber
            };
            return model;
        }
        private static Dictionary<string, object> SavePatientInfo(string MaritalStatus, string Title, string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string pincode, string religion, string department, string verificationCode, string state, string FatherHusbandName, int patientId, byte[] image, string aadharNumber)
        {
            PatientDetails _details = new PatientDetails();
            int pinResult = 0;
            PatientInfo info = new PatientInfo()
            {
                AadharNumber = aadharNumber,
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                DOB = Convert.ToDateTime(DOB),
                Gender = Gender,
                MobileNumber = mobilenumber.Trim(),
                Email = email.Trim(),
                Address = address,
                CityId = Convert.ToInt32(city),
                Country = country,
                PinCode = int.TryParse(pincode, out pinResult) ? pinResult : 0,
                Religion = religion,
                OTP = verificationCode,
                DepartmentId = Convert.ToInt32(department),
                StateId = Convert.ToInt32(state),
                FatherOrHusbandName = FatherHusbandName,
                MaritalStatus = MaritalStatus,
                Title = Title
            };
            if (patientId > 0)
                info.PatientId = patientId;
            if (image != null && image.Length > 0)
                info.Photo = image;
            var result = _details.CreateOrUpdatePatientDetail(info);
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
        [CustomAuthorize]
        public ActionResult EditProfile()
        {
            ViewData["Action"] = "Edit";
            return RedirectToAction("MyProfile", new { actionName = "Edit" });
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult UpdateProfile(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string state, string pincode, string religion, string department, HttpPostedFileBase photo, string FatherHusbandName, string MaritalStatus, string title, string aadharNumber)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (mobilenumber.Trim().Length != 10)
            {
                SetAlertMessage("Please Enter correct Mobile Number", "Register");
                return RedirectToAction("Register");
            }
            else if (!Regex.IsMatch(email, emailRegEx, RegexOptions.IgnoreCase))
            {
                SetAlertMessage("Please Enter correct Email Address", "Register");
                return RedirectToAction("Register");
            }
            else
            {
                byte[] image = null;
                if (photo != null && photo.ContentLength > 0)
                {
                    image = new byte[photo.ContentLength];
                    photo.InputStream.Read(image, 0, photo.ContentLength);
                    var img = new WebImage(image).Resize(2000, 2000, true, true);
                    image = img.GetBytes();
                }
                Dictionary<string, object> result = SavePatientInfo(MaritalStatus, title, firstname, middlename, lastname, DOB, Gender, mobilenumber, email, address, city, country, pincode, religion, department, "", state, FatherHusbandName, Convert.ToInt32(User.Id), image, aadharNumber);
                if (result["status"].ToString() == CrudStatus.Saved.ToString())
                {
                    return RedirectToAction("MyProfile");
                }
                else
                {
                    SetAlertMessage("Profile Not updated", "MyProfile");
                    ViewData["Action"] = "Edit";
                    return RedirectToAction("MyProfile", new { actionName = "Edit" });
                }
            }
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string registernumber)
        {
            PatientDetails _detail = new PatientDetails();
            var patient = _detail.GetPatientDetailByRegistrationNumber(registernumber);
            if (patient == null)
            {
                SetAlertMessage("Registration number is not Correct.", "Forget Password");
                return View();
            }
            else
            {
                string resetCode = VerificationCodeGeneration.GetGeneratedResetCode();
                //udpate Patient with reset code
                patient.ResetCode = resetCode;
                _detail.UpdatePatientDetail(patient);
                SendMailForgetPassword(registernumber, patient, resetCode);
                ViewData["msg"] = "We have Sent you an Email for reset password link.kindly check your email";
                return View();
            }
        }

        private async Task SendMailForgetPassword(string registernumber, PatientInfo patient, string resetCode)
        {
            await Task.Run(() =>
            {
                string passwordCreateURL = "Home/ResetPassword?resetCode=" + resetCode;
                string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));

                Message msg = new Message()
                {
                    MessageTo = patient.Email,
                    MessageNameTo = patient.FirstName + " " + patient.MiddleName + (string.IsNullOrWhiteSpace(patient.MiddleName) ? "" : " ") + patient.LastName,
                    Subject = "Forget Password",
                    Body = EmailHelper.GetForgetPasswordEmail(patient.FirstName, patient.MiddleName, patient.LastName, registernumber, baseUrl + passwordCreateURL)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }

        public ActionResult ForgetUserID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetUserID(string emailmobile)
        {
            PatientDetails _detail = new PatientDetails();
            var patient = _detail.GetPatientDetailByMobileNumberOrEmail(emailmobile);
            if (patient == null)
            {
                SetAlertMessage("Mobile number Or Email is not Correct.", "Forget User Id");
                return View();
            }
            else
            {
                SendMailForgetUserId(patient);
                ViewData["msg"] = "We have Sent you an Email that refering your registration number.kindly check your email";
                return View();
            }
        }

        private async Task SendMailForgetUserId(PatientInfo patient)
        {
            await Task.Run(() =>
            {
                Message msg = new Message()
                {
                    MessageTo = patient.Email,
                    MessageNameTo = patient.FirstName + " " + patient.MiddleName + (string.IsNullOrWhiteSpace(patient.MiddleName) ? "" : " ") + patient.LastName,
                    Subject = "Forget UserID",
                    Body = EmailHelper.GetForgetUserIdEmail(patient.FirstName, patient.MiddleName, patient.LastName, patient.RegistrationNumber)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }
        [CustomAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult MakePayment()
        {
            PatientDetails _details = new PatientDetails();
            var result = _details.GetPatientDetailById(User.Id);
            if (result != null)
            {
                Session["PatientInfoRenewal"] = result;
                return RedirectToAction("TransactionPayReNewal");
            }
            else
            {
                return View("Logout");
            }

        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult ChangePassword(string oldpassword, string newpassword, string confirmnewpassword)
        {
            if (newpassword.Trim() != confirmnewpassword.Trim())
            {
                SetAlertMessage("Password and Confirm Password are not match", "password Reset");
                return View();
            }
            else
            {
                PatientDetails _details = new PatientDetails();
                var result = _details.GetPatientDetailById(User.Id);
                if (result != null)
                {
                    if (result.Password == oldpassword)
                    {
                        result.Password = newpassword.Trim();
                        _details.UpdatePatientDetail(result);
                        ViewData["msg"] = "Password reset Successfully, please login again.";
                        return View();
                    }
                    else
                    {
                        SetAlertMessage("given Password is not correct", "Password Reset");
                        return View();
                    }

                }
                else
                {
                    SetAlertMessage("User Not found", "password Reset");
                    return View();
                }
            }
        }

        private void SaveLoginHistory(int patientId)
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            PatientLoginHistory history = new PatientLoginHistory
            {
                PatientId = patientId,
                LoginDate = DateTime.Now,
                IPAddress = ipAddress
            };
            PatientDetails detail = new PatientDetails();
            detail.SavePatientLoginHistory(history);
        }

        [HttpGet]
        public ActionResult CRIntegrate(bool? successMSG)
        {
            if (successMSG != null && successMSG == true)
            {
                ViewData["success"] = true;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CRIntegrate(string CRNumber)
        {
            PatientDetails details = new PatientDetails();
            var patientInfo = details.GetPatientDetailByRegistrationNumberOrCRNumber(CRNumber);

            if (patientInfo != null)
            {
                SetAlertMessage("CR Number is already exist in our database, choose another one.", "CR Integrate");
                return View();
            }

            WebServiceIntegration service = new WebServiceIntegration();
            var patient = service.GetPatientInfoBYCRNumber(CRNumber);
            if (patient != null)
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
                    Department = Convert.ToString(patient.deptid),
                    StateId = patient.State != "N/A" ? GetStateIdByStateName(patient.State) : string.Empty,
                    FatherOrHusbandName = patient.FatherOrHusbandName != "N/A" ? patient.FatherOrHusbandName : string.Empty,
                    CRNumber = patient.Registrationnumber != "N/A" ? patient.Registrationnumber : string.Empty,
                    Title = patient.Title != "N/A" ? patient.Title : string.Empty,
                    AadharNumber = patient.AadharNo != "N/A" ? patient.AadharNo : string.Empty,
                    MaritalStatus = patient.MaritalStatus != "N/A" ? patient.MaritalStatus : string.Empty,
                    DoR = patient.DoR != "N/A" ? patient.DoR : string.Empty,
                    ValidUpto = patient.ValidUpto != "N/A" ? Convert.ToDateTime(patient.ValidUpto) : Convert.ToDateTime(patient.DoR).AddMonths(Convert.ToInt32(ConfigurationManager.AppSettings["RegistrationValidityInMonth"])),
                };
                TimeSpan ageDiff = DateTime.Now.Subtract(Convert.ToDateTime(patient.DoR));
                crData.DOB = crData.DOB.Value.Add(ageDiff);
                if (crData.LastName == string.Empty && !string.IsNullOrEmpty(crData.MiddleName))
                {
                    crData.LastName = crData.MiddleName;
                    crData.MiddleName = string.Empty;
                }
                ViewData["CRData"] = crData;
                Session["crData"] = crData;
                return View();
            }
            else
            {
                SetAlertMessage("CR Number not found or expire, Kindly contact to hospital.", "CR Integrate");
                return View();
            }
        }

        [HttpPost]
        public ActionResult SubmitCRDetail(string firstname, string middlename, string lastname, string DOB, string Gender, string mobilenumber, string email, string address, string city, string country, string state, string pincode, string religion, string department, string FatherHusbandName, string title, string MaritalStatus, string aadharNumber)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (mobilenumber.Trim().Length != 10)
            {
                SetAlertMessage("Please Enter correct Mobile Number", "Register");
            }
            else if (!Regex.IsMatch(email, emailRegEx, RegexOptions.IgnoreCase))
            {
                SetAlertMessage("Please Enter correct Email Address", "Register");
            }
            else
            {
                Dictionary<string, object> result = SavePatientInfo(MaritalStatus, title, firstname, middlename, lastname, DOB, Gender, mobilenumber, email, address, city, country, pincode, religion, department, "", state, FatherHusbandName, 0, null, aadharNumber);
                if (result["status"].ToString() == CrudStatus.Saved.ToString())
                {
                    string serialNumber = VerificationCodeGeneration.GetSerialNumber();
                    var crData = (PatientInfoModel)Session["crData"];
                    PatientInfo info = new PatientInfo()
                    {
                        RegistrationNumber = serialNumber,
                        CRNumber = !string.IsNullOrEmpty(Convert.ToString(crData.CRNumber)) ? Convert.ToString(crData.CRNumber) : string.Empty,
                        PatientId = ((PatientInfo)result["data"]).PatientId,
                        ValidUpto = crData.ValidUpto
                    };
                    PatientDetails _details = new PatientDetails();
                    info = _details.UpdatePatientDetail(info);
                    SendMailTransactionResponse(serialNumber, info);
                    Session["crData"] = null;
                }
                else
                {
                    SetAlertMessage("User is not saved, might be of Email Id or Mobile No is already taken.", "Submit CR Detail");
                }
            }
            return RedirectToAction("CRIntegrate", new { successMSG = true });
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult GetSates()
        {
            PatientDetails _details = new PatientDetails();
            return Json(_details.GetStates());
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult GetCities(int stateId)
        {
            PatientDetails _details = new PatientDetails();
            return Json(_details.GetCities(stateId));
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult GetStateByStateId(int stateId)
        {
            PatientDetails _details = new PatientDetails();
            return Json(_details.GetStateByStateId(stateId));
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult GetCitieByCItyId(int citiId)
        {
            PatientDetails _details = new PatientDetails();
            return Json(_details.GetCitieByCItyId(citiId));
        }

        private string GetStateIdByStateName(string stateName)
        {
            PatientDetails _details = new PatientDetails();
            return Convert.ToString(_details.GetStateIdByStateName(stateName).StateId);
        }

        private string GetCityIdByCItyName(string cityName)
        {
            PatientDetails _details = new PatientDetails();
            return Convert.ToString(_details.GetCityIdByCItyName(cityName).CityId);
        }
    }
}