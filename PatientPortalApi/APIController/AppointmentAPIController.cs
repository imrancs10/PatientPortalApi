﻿using DataLayer;
using PatientPortalApi.BAL.Appointments;
using PatientPortalApi.BAL.Commom;
using PatientPortalApi.BAL.Masters;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
using PatientPortalApi.Models.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("get/doctors/{doctorId}/timeslot/{dayId}/{date}")]
        [Authorize]
        public IHttpActionResult GetDoctorTimeslots(int doctorId, int dayId, DateTime date)
        {
            DoctorDetails details = new DoctorDetails();
            var list = details.GetDoctorTimeslots(doctorId, dayId, date);
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

        [HttpPost]
        [Route("saveappointment")]
        [Authorize]
        public IHttpActionResult SaveAppointment([FromBody]AppointmentEntryModel model)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                AppointDetails _details = new AppointDetails();
                int _patientId = 0;
                string _sessionPatienId = Convert.ToString(userInfo.PatientId);
                if (int.TryParse(_sessionPatienId, out _patientId))
                {
                    AppointmentInfo info = new AppointmentInfo
                    {
                        PatientId = _patientId,
                        AppointmentDateFrom = model.AppointmentDateFrom,
                        AppointmentDateTo = model.AppointmentDateTo,
                        DoctorId = model.DoctorId
                    };
                    Enums.CrudStatus result = _details.SaveAppointment(info);
                    if (result == Enums.CrudStatus.Saved)
                    {
                        SendMailForAppointment(info, model.doctorname, model.deptname, _patientId);
                        return Ok();
                    }
                    else
                    {
                        ErrorCodeDetail errorDetail = ResponseCodeCollection.ResponseCodeDetails[ErrorCode.OnlyLimitedAppointmentCanBooked];
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
            return BadRequest();
        }

        private async Task SendMailForAppointment(AppointmentInfo model, string doctorname, string deptname, int _patientId)
        {
            await Task.Run(() =>
            {
                var user = (new PatientDetails()).GetPatientDetailById(_patientId);
                var appSetting = (new CommonDetails()).GetAppointmentSetting();
                //Send Email
                Message msg = new Message()
                {
                    MessageTo = user.Email,
                    MessageNameTo = user.FirstName + " " + user.MiddleName + (string.IsNullOrWhiteSpace(user.MiddleName) ? string.Empty : " ") + user.LastName,
                    Subject = "Appointment Booking Confirmation",
                    Body = EmailHelper.GetAppointmentSuccessEmail(user.FirstName, user.MiddleName, user.LastName, doctorname, 
                                                            model.AppointmentDateFrom, deptname, appSetting.IsActiveAppointmentMessage, 
                                                            appSetting.AppointmentMessage)
                };
                ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                sendMessageStrategy.SendMessages();

                //Send SMS
                msg.Body = "Hello " + string.Format("{0} {1}", user.FirstName, user.LastName) + "\nAs you requested an appointment with " + doctorname + " is  booked on schedule time " + model.AppointmentDateFrom.ToString("dd-MMM-yyyy hh:mm tt") + " at " + deptname + " Department\n Regards:\n Patient Portal(RMLHIMS)";
                msg.MessageTo = user.MobileNumber;
                msg.MessageType = MessageType.Appointment;
                sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                sendMessageStrategy.SendMessages();
            });
        }
    }

}
