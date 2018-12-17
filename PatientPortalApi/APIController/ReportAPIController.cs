using DataLayer;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.BAL.Reports;
using PatientPortalApi.Models;
using System.Collections.Generic;
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
            return Ok(_details.GetLabReportData(userInfo.PatientId));
        }

        [Authorize]
        [Route("leadgerreport")]
        public IHttpActionResult GetLeadgerReport()
        {
            UserInfo userInfo = new UserInfo(User);
            ReportDetails _details = new ReportDetails();
            return Ok(_details.GetPatientLedger(userInfo.PatientId));
        }
    }

}
