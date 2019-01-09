using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class AppointmentModel
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public bool HasBooked { get; set; } = false;
    }
}