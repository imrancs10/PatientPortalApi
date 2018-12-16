using PatientPortalApi.BAL.Masters;
using PatientPortalApi.Infrastructure.Adapter.WebService;
using PatientPortalApi.Models;
using System.Web.Http;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/hospital")]
    public class HospitalAPIController : ApiController
    {
        [Route("get/hospital")]
        public IHttpActionResult GetHospitalDetail()
        {
            HospitalDetails _details = new HospitalDetails();
            return Ok(_details.GetHospitalDetail());
        }
        [Route("get/opddetail")]
        [System.Web.Http.Authorize]
        public IHttpActionResult GetPatientOPDDetail()
        {
            UserInfo userInfo = new UserInfo(User);
            if (userInfo != null)
            {
                string crNumber = string.IsNullOrEmpty(userInfo.CRNumber) ? userInfo.RegistrationNumber : userInfo.CRNumber;
                var opdDetail = (new WebServiceIntegration()).GetPatientOPDDetail(crNumber);
                return Ok(opdDetail);
            }
            return BadRequest();
        }
    }

}
