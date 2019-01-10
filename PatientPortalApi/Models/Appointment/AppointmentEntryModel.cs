using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class AppointmentEntryModel
    {
        public int DoctorId { get; set; }
        public string AppointmentDateFrom { get; set; }
        public string AppointmentDateTo { get; set; }
        public string doctorname { get; set; }
        public string deptname { get; set; }
    }
}