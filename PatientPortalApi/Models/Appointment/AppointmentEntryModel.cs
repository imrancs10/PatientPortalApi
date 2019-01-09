using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class AppointmentEntryModel
    {
        public int DoctorId { get; set; }
        public DateTime AppointmentDateFrom { get; set; }
        public DateTime AppointmentDateTo { get; set; }
        public string doctorname { get; set; }
        public string deptname { get; set; }
    }
}