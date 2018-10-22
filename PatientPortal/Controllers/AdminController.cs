using PatientPortal.BAL.Commom;
using PatientPortal.BAL.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PatientPortal.Controllers
{
    public class AdminController : CommonController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PatientBillReport()
        {
            return View();
        }

        public ActionResult PatientLabReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SetBillingReport(HttpPostedFileBase reportfile, int PatientId, string BillNo, string BillType, DateTime BillDate, string ReportUrl, decimal BillAmount)
        {
            string ReportPath = string.Empty;
            if (reportfile != null)
            {
                CommonDetails fileupload = new CommonDetails();
                ReportPath = fileupload.ReportFileUpload(reportfile, Global.Enums.ReportType.Bill, BillNo);
            }
            else
            {
                ReportPath = ReportUrl;
            }
            ReportDetails _details = new ReportDetails();
            _details.SetBillReportData(PatientId, BillNo, BillType, BillDate, ReportPath, BillAmount);
            return View("PatientBillReport");
        }

        [HttpPost]
        public ActionResult SetLabReport(HttpPostedFileBase reportfile,DateTime ReportDate, int PatientId, string BillNo, string RefNo, string LabName, string ReportUrl)
        {
            string ReportPath = string.Empty;
            if (reportfile != null)
            {
                CommonDetails fileupload = new CommonDetails();
                ReportPath = fileupload.ReportFileUpload(reportfile, Global.Enums.ReportType.Lab, RefNo);
            }
            else
            {
                ReportPath = ReportUrl;
            }
            ReportDetails _details = new ReportDetails();
            _details.SetLabReportData(PatientId, BillNo, RefNo, ReportPath, LabName, ReportDate);
            return View("PatientLabReport");
        }

    }
}