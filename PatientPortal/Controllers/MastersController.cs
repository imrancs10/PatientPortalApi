using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using PatientPortal.BAL.Masters;
using PatientPortal.BAL.Patient;
using PatientPortal.Models.Masters;

namespace PatientPortal.Controllers
{
    public class MastersController : CommonController
    {
        DepartmentDetails _details = null;
        private readonly object FileUpload1;

        public ActionResult AddSchedule()
        {
            return View();
        }

        public ActionResult AddDepartments()
        {
            return View();
        }

        public ActionResult AddDoctors()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveDepartment(string deptName)
        {
            _details = new DepartmentDetails();

            return Json(CrudResponse(_details.SaveDept(deptName)), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditDepartment(string deptName, int deptId)
        {
            _details = new DepartmentDetails();

            return Json(CrudResponse(_details.EditDept(deptName, deptId)), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteDepartment(int deptId)
        {
            _details = new DepartmentDetails();

            return Json(CrudResponse(_details.DeleteDept(deptId)), JsonRequestBehavior.AllowGet);
        }

        public override JsonResult GetDepartments()
        {
            _details = new DepartmentDetails();
            return Json(_details.DepartmentList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDoctor(string doctorName, int deptId, string designation, string degree)
        {
            DoctorDetails _details = new DoctorDetails();

            return Json(CrudResponse(_details.SaveDoctor(doctorName, deptId, designation, degree)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditDoctor(string doctorName, int deptId, int docId, string designation, string degree)
        {
            DoctorDetails _details = new DoctorDetails();
            return Json(CrudResponse(_details.EditDoctor(doctorName, deptId, docId, designation, degree)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDoctor(int docId)
        {
            DoctorDetails _details = new DoctorDetails();
            return Json(CrudResponse(_details.DeleteDoctor(docId)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDoctors()
        {
            DoctorDetails _details = new DoctorDetails();
            return Json(_details.DoctorList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSchedule(ScheduleModel model)
        {
            ScheduleDetails _details = new ScheduleDetails();
            return Json(CrudResponse(_details.SaveSchedule(model)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditSchedule(ScheduleModel model)
        {
            ScheduleDetails _details = new ScheduleDetails();
            return Json(CrudResponse(_details.EditSchedule(model)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSchedule(ScheduleModel model)
        {
            ScheduleDetails _details = new ScheduleDetails();

            return Json(CrudResponse(_details.DeleteSchedule(model.ScheduleId)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSchedule()
        {
            ScheduleDetails _details = new ScheduleDetails();
            return Json(_details.ScheduleList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult HospitalDetail()
        {
            HospitalDetails details = new HospitalDetails();
            ViewData["hospitals"] = details.GetAllHospitalDetail();
            return View();
        }

        [HttpPost]
        public ActionResult HospitalDetail(string name, HttpPostedFileBase File1)
        {
            HospitalDetail hospital = new HospitalDetail();
            if (File1 != null && File1.ContentLength > 0)
            {
                hospital.HospitalName = name;
                hospital.HospitalLogo = new byte[File1.ContentLength];
                File1.InputStream.Read(hospital.HospitalLogo, 0, File1.ContentLength);
                HospitalDetails details = new HospitalDetails();
                details.SaveHospital(hospital);
                SetAlertMessage("Hospital detail saved", "Hospital Entry");
                return RedirectToAction("HospitalDetail");
            }
            else
            {
                SetAlertMessage("Hospital detail not saved", "Hospital Entry");
                return RedirectToAction("HospitalDetail");
            }
        }
        public ActionResult DeleteHospital(string Id)
        {
            int result = 0;
            int.TryParse(Id, out result);
            HospitalDetails details = new HospitalDetails();
            details.DeleteHospitalDetail(result);
            return RedirectToAction("HospitalDetail");
        }

        [HttpGet]
        public ActionResult AddDoctorLeave()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDoctorLeaveList(int doctorId)
        {
            DoctorDetails _details = new DoctorDetails();
            return Json(_details.GetDoctorLeaveList(doctorId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveDoctorLeave(int doctorId, DateTime leaveDate)
        {
            DoctorDetails _details = new DoctorDetails();
            return Json(CrudResponse(_details.SaveDoctorLeave(doctorId, leaveDate)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult AppointmentSetting()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveAppSetting(AppSettingModel model)
        {
            AppointmentSettingDetails _details = new AppointmentSettingDetails();
            return Json(CrudResponse(_details.SaveAppSetting(model)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAppSetting()
        {
            AppointmentSettingDetails _details = new AppointmentSettingDetails();
            return Json(_details.GetAppSetting(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LabReport(string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                PatientDetails _detail = new PatientDetails();
                var result = _detail.GetPatientDetailByRegistrationNumberSearch(search);
                ViewData["search"] = search;
                ViewData["PatientInfo"] = result;
            }
            return View();
        }

        [HttpPost]
        public ActionResult LabReport(string report, HttpPostedFileBase file, string patientId, string registrationNumber, string searchText)
        {
            PatientDetails patientdetail = new PatientDetails();
            if (file != null && file.ContentLength > 0)
            {
                LabReport labReport = new LabReport
                {
                    CreatedDate = DateTime.Now,
                    FileName = file.FileName,
                    PatientId = int.Parse(patientId),
                    ReportName = report
                };
                bool result = patientdetail.SavePatientLabReport(labReport);
                if (result)
                {
                    string dirUrl = "~/LabReports/" + registrationNumber;
                    string dirPath = Server.MapPath(dirUrl);
                    // Check for Directory, If not exist, then create it  
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    // save the file to the Specifyed folder  
                    string fileUrl = dirUrl + "/" + Path.GetFileName(file.FileName);
                    file.SaveAs(Server.MapPath(fileUrl));
                }
                SetAlertMessage("Lab Report saved", "Hospital Entry");
                return RedirectToAction("LabReport", new { search = searchText });
            }
            else
            {
                SetAlertMessage("Lab Report not saved", "Lab Report");
                return RedirectToAction("LabReport", new { search = searchText });
            }
        }
    }
}