using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatientPortal.BAL;
using PatientPortal.BAL.Reports;
using System.IO;
using PatientPortal.BAL.Commom;

namespace PatientPortal.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult GetBillingReport()
        {
            ReportDetails _details = new ReportDetails();
            return View(_details.GetBillReportData());
        }

        public ActionResult ReportViewing()
        {
            ReportDetails _details = new ReportDetails();
            return View(_details.GetLabReportData());
        }

        public ActionResult PatientLedger()
        {
            ReportDetails _details = new ReportDetails();
            return View(_details.GetPatientLedger());
        }

        public ActionResult DownloadReportFile(string fileUrl)
        {
            string _fileDirectory = fileUrl.Substring(0, fileUrl.LastIndexOf("\\") + 1);
            string _fileName = fileUrl.Substring(fileUrl.LastIndexOf("\\") + 1);
            if (Directory.Exists(_fileDirectory))
            {
                string[] files = Directory.GetFiles(_fileDirectory);
                if (files.Length > 0)
                {
                    var file = files.Where(x => x.Substring(x.LastIndexOf("\\") + 1) == _fileName).FirstOrDefault();
                    if (file != null)
                    {
                        byte[] FileBytes = System.IO.File.ReadAllBytes(file);
                        return File(FileBytes, "application/pdf", _fileName.Substring(0, _fileName.LastIndexOf('.')) + "-" + DateTime.Now.ToShortDateString() + ".pdf");
                    }
                }
            }

            return RedirectToAction("GetBillingReport");
        }

        public ActionResult ViewReportFile(string fileUrl, string _view)
        {
            string _fileDirectory = fileUrl.Substring(0, fileUrl.LastIndexOf("\\") + 1);
            string _fileName = fileUrl.Substring(fileUrl.LastIndexOf("\\") + 1);
            if (Directory.Exists(_fileDirectory))
            {
                string[] _files = Directory.GetFiles(_fileDirectory);
                if (_files.Where(x => x.Substring(fileUrl.LastIndexOf("\\") + 1) == _fileName).Count() > 0)
                {
                    byte[] _fileContent = System.IO.File.ReadAllBytes(fileUrl);
                    return File(_fileContent, "application/pdf");
                }
                return View(_view);
            }
            return View(_view);
        }
    }
}