using DataLayer;
using PatientPortalApi.BAL.Appointments;
using PatientPortalApi.BAL.Commom;
using PatientPortalApi.BAL.Masters;
using PatientPortalApi.BAL.Patient;
using PatientPortalApi.Global;
using PatientPortalApi.Infrastructure;
using PatientPortalApi.Infrastructure.Adapter.WebService;
using PatientPortalApi.Infrastructure.Utility;
using PatientPortalApi.Models;
using PatientPortalApi.Models.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using static PatientPortalApi.Global.Enums;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/emr")]
    public class EMRAPIController : ApiController
    {
        [Route("get/dischargesummaries")]
        [Authorize]
        public IHttpActionResult GetDischargeSummaries()
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                AppointDetails _details = new AppointDetails();
                string crNumber = !string.IsNullOrEmpty(userInfo.CRNumber) ? userInfo.CRNumber : userInfo.RegistrationNumber;
                if (!string.IsNullOrEmpty(crNumber))
                {
                    var reports = (new WebServiceIntegration()).GetDischargeSummaryDetail(
                                        crNumber,
                                        (Convert.ToInt32(OPDTypeEnum.DischargeSummary)).ToString());
                    if (reports != null)
                    {
                        reports.ForEach(x =>
                        {
                            x.ipnoEncode = CryptoEngine.Encrypt(x.ipno);
                        });
                    }
                    return Ok(reports);
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

        [Route("get/dischargesummary/{ipNoEncode}")]
        [Authorize]
        public IHttpActionResult GetDischargeSummary(string ipNoEncode)
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                AppointDetails _details = new AppointDetails();
                string crNumber = !string.IsNullOrEmpty(userInfo.CRNumber) ? userInfo.CRNumber : userInfo.RegistrationNumber;
                if (!string.IsNullOrEmpty(crNumber))
                {
                    var reports = (new WebServiceIntegration()).GetDischargeSummaryDetail(
                                        crNumber,
                                        (Convert.ToInt32(OPDTypeEnum.DischargeSummary)).ToString());
                    var report = reports.Where(x => x.ipno == CryptoEngine.Decrypt(ipNoEncode)).FirstOrDefault();
                    return Ok(report);
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
