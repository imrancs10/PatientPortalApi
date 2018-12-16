using PatientPortalApi.BAL.Appointments;
using PatientPortalApi.Global;
using PatientPortalApi.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/appointment")]
    public class AppointmentAPIController : ApiController
    {
        [Route("get/appointments")]
        [Authorize]
        public IHttpActionResult GetAppointmentDetail()
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                AppointDetails _details = new AppointDetails();
                int _patientId = 0;
                string _sessionPatienId = Convert.ToString(userInfo.PatientId);
                Dictionary<int, string> result = new Dictionary<int, string>();
                if (int.TryParse(_sessionPatienId, out _patientId))
                {
                    return Ok(_details.PatientAppointmentList(_patientId, 0, 0));
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
    }

}
