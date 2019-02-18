using DataLayer;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.BAL.Reports;
using PatientPortalApi.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/report")]
    public class ReportAPIController : ApiController
    {
        [Authorize]
        [Route("billingreport")]
        public IHttpActionResult GetBillingReport()
        {
            UserInfo userInfo = new UserInfo(User);
            ReportDetails _details = new ReportDetails();
            List<PateintLeadger> result = _details.GetBillReportData(userInfo.PatientId);
            return Ok(result);
        }

        [Authorize]
        [Route("duplicatereport")]
        public IHttpActionResult GetDuplicateBilling()
        {
            UserInfo userInfo = new UserInfo(User);
            ReportDetails _details = new ReportDetails();
            List<PateintLeadger> result = _details.GetBillReportData(userInfo.PatientId);
            return Ok(result);
        }

        [Authorize]
        [Route("labreport")]
        public IHttpActionResult GetLabReport()
        {
            UserInfo userInfo = new UserInfo(User);
            ReportDetails _details = new ReportDetails();
            var result = _details.GetLabReportData(userInfo.PatientId);
            if (result != null && result.Any())
            {
                result.ForEach(x =>
                {
                    x.Url = ConfigurationManager.AppSettings["HISLabReportUrl"] + x.Url.Replace("~/LabRepPdf", "");
                });
            }
            return Ok(result);
        }

        [Authorize]
        [Route("leadgerreport")]
        public IHttpActionResult GetLeadgerReport()
        {
            UserInfo userInfo = new UserInfo(User);
            ReportDetails _details = new ReportDetails();
            var ledgerData = _details.GetPatientLedger(userInfo.PatientId);
            ledgerData.ForEach(x =>
            {
                if (x.Type == "GP" || x.Type == "PH" || x.Type == "SV" || x.Type == "SP")
                {
                    x.Receipt = "";
                }
                else
                {
                    x.Payment = "";
                }
            });
            return Ok(ledgerData);
        }

        [Authorize]
        [Route("newmessagecount")]
        public IHttpActionResult NewMessageCount()
        {
            UserInfo userInfo = new UserInfo(User);
            PatientDetails _detail = new PatientDetails();
            var result = _detail.GetPatientMessageCount(userInfo.PatientId);
            return Ok(result);
        }

        [Authorize]
        [Route("getmessages")]
        public IHttpActionResult UpdateAndGetPatientMessageList()
        {
            UserInfo userInfo = new UserInfo(User);
            PatientDetails _detail = new PatientDetails();
            var result = _detail.UpdateAndGetPatientMessageList(userInfo.PatientId);
            return Ok(result);
        }
    }

}
