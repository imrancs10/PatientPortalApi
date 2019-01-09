using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class BookedAppointmentModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public System.DateTime AppointmentDateFrom { get; set; }
        public System.DateTime AppointmentDateTo { get; set; }

    }
}