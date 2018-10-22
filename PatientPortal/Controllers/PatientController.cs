using PatientPortal.BAL.Patient;
using PatientPortal.Global;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace PatientPortal.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult GetLabReport()
        {
            PatientDetails _details = new PatientDetails();
            return View(_details.GetReportList(Convert.ToInt32(Session["PatientId"])));
        }

        public ActionResult GetPDF(string fileName)
        {
            string dirUrl = "~/LabReports/" + WebSession.PatientRegNo;
            string dirPath = Server.MapPath(dirUrl);
            if (Directory.Exists(dirPath))
            {
                string[] files = Directory.GetFiles(dirPath);
                if (files.Length > 0)
                {
                    var file = files.Where(x => x.Substring(x.LastIndexOf("\\") + 1) == fileName).FirstOrDefault();
                    if (file != null)
                    {
                        byte[] FileBytes = System.IO.File.ReadAllBytes(file);
                        return File(FileBytes, "application/pdf", fileName.Substring(0, fileName.LastIndexOf('.')) + "-" + DateTime.Now.ToShortDateString() + ".pdf");
                    }
                }
            }

            return RedirectToAction("GetLabReport");
        }
    }
}