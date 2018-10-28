using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatientPortal.BAL.Appointments;
using DataLayer;
using PatientPortal.Infrastructure;
using PatientPortal.Infrastructure.Utility;
using PatientPortal.Global;
using PatientPortal.Models;
using PatientPortal.BAL.Patient;

namespace PatientPortal.Controllers
{
    public class AppointmentController : CommonController
    {
        // GET: Appointment
        public ActionResult GetAppointments()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DeptWiseDoctorScheduleList(int deptId = 0, int year = 0, int month = 0)
        {
            AppointDetails _details = new AppointDetails();
            return Json(_details.DeptWiseDoctorScheduleList(deptId, year, month), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DayWiseDoctorScheduleList(int deptId, string day, DateTime? date)
        {
            AppointDetails _details = new AppointDetails();
            return Json(_details.DayWiseDoctorScheduleList(deptId, day, date), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DateWiseDoctorAppointmentList(DateTime date)
        {
            AppointDetails _details = new AppointDetails();
            return Json(_details.DateWiseDoctorAppointmentList(date), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAppointment(AppointmentInfo model, string doctorname, string deptname)
        {
            AppointDetails _details = new AppointDetails();
            string PatientId = Session["PatientId"] == null ? "0" : Session["PatientId"].ToString();
            int pId = 0;
            if (int.TryParse(PatientId, out pId))
            {
                model.PatientId = pId;
                //PatientInfo data = (PatientInfo)Session["PatientData"];
                var user = User;
                Enums.CrudStatus result = _details.SaveAppointment(model);
                if (result == Enums.CrudStatus.Saved)
                {
                    //Send Email
                    Message msg = new Message()
                    {
                        MessageTo = user.Email,
                        MessageNameTo = user.FirstName + " " + user.MiddleName + (string.IsNullOrWhiteSpace(user.MiddleName) ? string.Empty : " ") + user.LastName,
                        Subject = "Appointment Booking Confirmation",
                        Body = EmailHelper.GetAppointmentSuccessEmail(user.FirstName, user.MiddleName, user.LastName, doctorname, model.AppointmentDateFrom, deptname)
                    };
                    ISendMessageStrategy sendMessageStrategy = new SendMessageStrategyForEmail(msg);
                    sendMessageStrategy.SendMessages();
                    
                    //Send SMS
                    msg.Body = "Hello " + string.Format("{0} {1}", user.FirstName, user.LastName) + "\nAs you requested an appointment with " + doctorname + "is  booked on schedule time " + model.AppointmentDateFrom.ToString("dd-MMM-yyyy hh:mm") + " at " + deptname + " Department\n Regards:\n Patient Portal(RMLHIMS)";
                    msg.MessageTo = user.Mobile;
                    msg.MessageType = MessageType.Appointment;
                    sendMessageStrategy = new SendMessageStrategyForSMS(msg);
                    sendMessageStrategy.SendMessages();
                    //Message smsConfig = new Message()
                    //{
                    //    Body = "Hello " + string.Format("{0} {1}", user.FirstName, user.LastName) + "\nAs you requested an appointment with " + doctorname + "is  booked on schedule time " + model.AppointmentDateFrom.ToString("dd-MMM-yyyy hh:mm") + " at " + deptname + " Department\n Regards:\n Patient Portal(RMLHIMS)",
                    //    MessageTo = user.Mobile
                    //};
                    //Infrastructure.SMSService sMSService = new SMSService();
                    //sMSService.Send(smsConfig);
                }
                return Json(CrudResponse(result), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(CrudResponse(Enums.CrudStatus.SessionExpired), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(new string[] { "Get", "Post" })]
        public JsonResult GetPatientAppointmentList(int year = 0, int month = 0)
        {
            AppointDetails _details = new AppointDetails();
            int _patientId = 0;
            string _sessionPatienId = Session["PatientId"] == null ? "0" : Session["PatientId"].ToString();
            Dictionary<int, string> result = new Dictionary<int, string>();
            if (int.TryParse(_sessionPatienId, out _patientId))
            {
                return Json(_details.PatientAppointmentList(_patientId, year, month), JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.Add((int)Enums.JsonResult.Invalid_DataId, "Patient Id is invalid");
                return Json(result.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CancelAppointment(int appointmentId, string CancelReason)
        {
            AppointDetails _details = new AppointDetails();
            int PatientId = 0;
            Dictionary<int, string> result = new Dictionary<int, string>();
            if (int.TryParse(Session["PatientId"].ToString(), out PatientId))
            {
                return Json(_details.CancelAppointment(PatientId, appointmentId, CancelReason).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.Add((int)Enums.JsonResult.Invalid_DataId, "Patient Id is invalid");
                return Json(result.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PatientProfile()
        {
            if (User == null)
            {
                SetAlertMessage("User has been logged out", "Update Profile");
                return RedirectToAction("Index");
            }
            var patient = GetPatientInfo();
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
        private PatientInfoModel GetPatientInfo()
        {
            PatientDetails _details = new PatientDetails();
            var result = _details.GetPatientDetailById(User.Id);
            PatientInfoModel model = new PatientInfoModel
            {
                RegistrationNumber = result.RegistrationNumber,
                Address = result.Address,
                CityId = Convert.ToString(result.City),
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
                StateId = Convert.ToString(result.State),
                Photo = result.Photo
            };
            return model;
        }
    }
}