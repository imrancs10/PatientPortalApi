using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class JwtResponse
    {
        public string Name { get; set; }
        public string JwtToken { get; set; }
    }
}