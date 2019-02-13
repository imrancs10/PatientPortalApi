using DataLayer;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/patient")]
    public class PatientAPIController : ApiController
    {
        //Declaring Log4Net
        //ILog logger = LogManager.GetLogger(typeof(AppointmentAPIController));
        //private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Authorize]
        [Route("patientprofile")]
        public IHttpActionResult GetPatientInfo()
        {
            UserInfo userInfo = new UserInfo(User);
            PatientDetails detail = new PatientDetails();
            var result = detail.GetPatientDetailById(userInfo.PatientId);
            string regNumber = !string.IsNullOrEmpty(result.CRNumber) ? result.CRNumber : result.RegistrationNumber;
            result.RegistrationNumber = regNumber;
            return Ok(result);
        }
        [Authorize]
        [Route("changepassword")]
        public IHttpActionResult ChangePassword(string oldPassword,string password)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                PatientDetails detail = new PatientDetails();
                //check old password
                var patient = detail.GetPatientDetailById(userInfo.PatientId);
                if (patient.Password == oldPassword)
                {
                    PatientInfo info = new PatientInfo()
                    {
                        PatientId = userInfo.PatientId,
                        Password = password
                    };
                    var result = detail.UpdatePatientDetail(info);
                    return Ok("Password Change succesfuly");
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.OldPasswordWrong];
                    Response<object> response = new Response<object>(errorDetail, null);
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
        [Route("forgetuserid")]
        [HttpPost]
        public IHttpActionResult ForgetUserId([FromBody]string emailormobile)
        {
            PatientDetails _detail = new PatientDetails();
            var patient = _detail.GetPatientDetailByMobileNumberOrEmail(emailormobile);
            if (patient != null)
            {
                SendMailForgetUserId(patient);
                return Ok("Mail sent.");
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.MobileEmailInCorrect];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
        [Route("forgetpassword")]
        [HttpPost]
        public IHttpActionResult ForgetPassword(string registrationNumber,string mobileNumber)
        {
            PatientDetails _detail = new PatientDetails();
            var patient = _detail.GetPatientDetailByRegistrationNumberAndMobileNumber(registrationNumber, mobileNumber);
            if (patient != null)
            {
                string verificationCode = VerificationCodeGeneration.GenerateDeviceVerificationCode();
                SendMailForgetPassword(patient.RegistrationNumber, patient, verificationCode);
                return Ok(verificationCode);
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.RegistrationorMobileInCorrect];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
        [Route("resetpassword")]
        [HttpPost]
        public IHttpActionResult ResetPassword(string registrationNumber,string password)
        {
            if (password.Trim().Length < 8)
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.Password8CharecterRequired];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
            else if (!password.Trim().Any(ch => char.IsUpper(ch)))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.PasswordUpperCharecterRequired];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
            else if (!password.Trim().Any(ch => char.IsNumber(ch)))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.PasswordNumericCharecterRequired];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
            else if (!password.Trim().Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.PasswordSpecialCharecterRequired];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
            else
            {
                PatientDetails _details = new PatientDetails();
                var result = _details.GetPatientDetailByRegistrationNumberOrCRNumber(registrationNumber);
                if (result != null)
                {
                    result.Password = password.Trim();
                    result.ResetCode = "";
                    _details.UpdatePatientDetail(result);
                    return Ok("Password change Succesfuly.");
                }
                else
                {
                    ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.InvalidUser];
                    Response<object> response = new Response<object>(errorDetail, null);
                    return Ok(response);
                }
            }
        }
        private async Task SendMailForgetPassword(string registernumber, PatientInfo patient, string verificationCode)
        {
            await Task.Run(() =>
            {
                Message msg = new Message()
                {
                    MessageTo = patient.Email,
                    MessageNameTo = patient.FirstName + " " + patient.MiddleName + (string.IsNullOrWhiteSpace(patient.MiddleName) ? "" : " ") + patient.LastName,
                    Subject = "Forget Password",
                    Body = EmailHelper.GetForgetPasswordEmail(patient.FirstName, patient.MiddleName, patient.LastName, registernumber, verificationCode)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();

                //Send SMS
                msg.Body = "Hello " + string.Format("{0} {1}", patient.FirstName, patient.LastName) + "\nAs you requested, here is a OTP " + verificationCode + " you can use it to verify your mobile number and reset your password before 15 minutes.\n Regards:\n Patient Portal(RMLHIMS)";
                msg.MessageTo = patient.MobileNumber;
                msg.MessageType = MessageType.OTP;
                sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                sendMessageStrategy.SendMessages();
            });
        }
        private async Task SendMailForgetUserId(PatientInfo patient)
        {
            await Task.Run(() =>
            {
                string regNumber = !string.IsNullOrEmpty(patient.CRNumber) ? patient.CRNumber : patient.RegistrationNumber;
                Message msg = new Message()
                {
                    MessageTo = patient.Email,
                    MessageNameTo = patient.FirstName + " " + patient.MiddleName + (string.IsNullOrWhiteSpace(patient.MiddleName) ? "" : " ") + patient.LastName,
                    Subject = "Forget UserID",
                    Body = EmailHelper.GetForgetUserIdEmail(patient.FirstName, patient.MiddleName, patient.LastName, regNumber)
                };

                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();
            });
        }
    }

}
