using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace PatientPortal.APIController
{
    public class LoginAPIController : ApiController
    {
        /// <summary>
        /// Get Patient List
        /// </summary>
        /// <returns>List of Patientinfo</returns>
        public List<PatientInfo> GetPatientInfo(int id)
        {
            List<PatientInfo> list = new List<PatientInfo>() { new PatientInfo() { PatientId = 1 } };
            return list;
        }
    }
}
