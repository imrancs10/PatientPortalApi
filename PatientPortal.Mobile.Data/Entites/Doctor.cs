using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class Doctor
    {
        public Doctor()
        {
            AppointmentInfo = new HashSet<AppointmentInfo>();
            DoctorLeave = new HashSet<DoctorLeave>();
            DoctorSchedule = new HashSet<DoctorSchedule>();
        }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int DepartmentId { get; set; }
        public string Designation { get; set; }
        public string Degree { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Department Department { get; set; }
        public ICollection<AppointmentInfo> AppointmentInfo { get; set; }
        public ICollection<DoctorLeave> DoctorLeave { get; set; }
        public ICollection<DoctorSchedule> DoctorSchedule { get; set; }
    }
}
