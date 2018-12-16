using PatientPortalApi.BAL.Masters;
using PatientPortalApi.BAL.Patient;
using System.Web.Http;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace PatientPortalApi.APIController
{
    [RoutePrefix("api/master")]
    public class MasterAPIController : ApiController
    {
        [Route("get/departments")]
        public IHttpActionResult GetDepartmentDetail()
        {
            DepartmentDetails detail = new DepartmentDetails();
            var depts = detail.DepartmentList();
            return Ok(depts);
        }
        [Route("get/states")]
        public IHttpActionResult GetStates()
        {
            PatientDetails detail = new PatientDetails();
            var depts = detail.GetStates();
            return Ok(depts);
        }
        [Route("get/cities")]
        public IHttpActionResult GetCities(int stateId)
        {
            PatientDetails detail = new PatientDetails();
            var depts = detail.GetCities(stateId);
            return Ok(depts);
        }
    }

}
