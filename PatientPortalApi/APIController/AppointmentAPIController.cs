using PatientPortalApi.BAL.Appointments;
using PatientPortalApi.BAL.Masters;
using PatientPortalApi.Global;
using PatientPortalApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Route("get/doctors/{deaprtmentId}")]
        [Authorize]
        public IHttpActionResult GetDoctorsByDepartment(int deaprtmentId)
        {
            DoctorDetails details = new DoctorDetails();
            var list = details.DoctorList(deaprtmentId);
            if (list.Any())
            {
                return Ok(list);
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.NoDoctorFound];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
        [Route("get/doctorshedules/{doctorId}")]
        [Authorize]
        public IHttpActionResult GetDoctorShedulesByDoctor(int doctorId)
        {
            DoctorDetails details = new DoctorDetails();
            var list = details.GetDoctorShedulesByDoctor(doctorId);
            if (list.Any())
            {
                return Ok(list);
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.NoDoctorScheduleFound];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
        [Route("get/doctors/{doctorId}/timeslot/{dayId}")]
        [Authorize]
        public IHttpActionResult GetDoctorTimeslots(int doctorId, int dayId)
        {
            DoctorDetails details = new DoctorDetails();
            var list = details.GetDoctorTimeslots(doctorId, dayId);
            if (list.Any())
            {
                return Ok(list);
            }
            else
            {
                ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.NoDoctorScheduleFound];
                Response<object> response = new Response<object>(errorDetail, null);
                return Ok(response);
            }
        }
    }

}
