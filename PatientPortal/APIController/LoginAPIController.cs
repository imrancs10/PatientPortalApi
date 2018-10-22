using DataLayer;
using PatientPortal.BAL.Patient;
using PatientPortal.Infrastructure.Utility;
using PatientPortal.Models;
using PatientPortal.Models.User;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace PatientPortal.APIController
{
    public class LoginAPIController : ApiController
    {
        /// <summary>
        /// Get Patient List
        /// </summary>
        /// <returns>List of Patientinfo</returns>
        [Authorize]
        public List<PatientInfo> GetPatientInfo()
        {

            UserInfo userInfo = new UserInfo(User);

            List<PatientInfo> list = new List<PatientInfo>() { new PatientInfo() { PatientId = 1 } };
            return list;
        }

        [HttpPost]
        public IHttpActionResult Authenticate([FromUri]string registrationNo, [FromUri]string password)
        {
            IHttpActionResult response;
            HttpResponseMessage responseMsg = new HttpResponseMessage();
            bool isUsernamePasswordValid = false;

            if (!string.IsNullOrEmpty(registrationNo) && !string.IsNullOrEmpty(password))
            {
                PatientDetails detail = new PatientDetails();
                var result = detail.GetPatientDetail(registrationNo, password);
                var patientInfo = ((PatientInfo)result["data"]);
                if (result != null && patientInfo != null)
                {
                    UserClaims claims = new UserClaims()
                    {
                        CRNumber = patientInfo.CRNumber,
                        PatientId = patientInfo.PatientId,
                        RegistrationNumber = patientInfo.RegistrationNumber,
                        ValidUpTo = patientInfo.ValidUpto.Value
                    };
                    string token = (new JWTTokenService()).CreateToken(claims);
                    //return the token
                    JwtResponse jwtResponse = new JwtResponse()
                    {
                        Name = patientInfo.FirstName,
                        JwtToken = token
                    };
                    return Ok<JwtResponse>(jwtResponse);
                }
                else
                {
                    // if credentials are not valid send unauthorized status code in response
                    //loginResponse.responseMsg.StatusCode = HttpStatusCode.Unauthorized;
                    responseMsg.StatusCode = HttpStatusCode.NotFound;
                    response = ResponseMessage(responseMsg);
                    return response;
                }
            }
            return BadRequest();
        }
    }
}
