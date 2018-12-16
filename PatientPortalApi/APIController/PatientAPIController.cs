using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Models;
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
            return Ok(result);
        }

    }

}
