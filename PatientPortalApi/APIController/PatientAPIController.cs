using DataLayer;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
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
        public IHttpActionResult ChangePassword(string password)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                PatientDetails detail = new PatientDetails();
                PatientInfo info = new PatientInfo()
                {
                    PatientId = userInfo.PatientId,
                    Password = password
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
        [Route("forgetuserid")]
        public IHttpActionResult ForgetUserId([FromBody]string emailormobile)
        {
            PatientDetails _detail = new PatientDetails();
            var patient = _detail.GetPatientDetailByMobileNumberOrEmail(emailormobile);
            if (patient != null)
            {
                SendMailForgetUserId(patient);
                return Ok();
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.MobileEmailInCorrect];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
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
