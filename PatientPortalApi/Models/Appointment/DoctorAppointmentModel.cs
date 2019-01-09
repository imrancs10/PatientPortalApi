using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class DoctorAppointmentModel
    {
        public int? DayId { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        
    }
}