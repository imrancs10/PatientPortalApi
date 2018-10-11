using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class DayMaster
    {
        public DayMaster()
        {
            DoctorSchedule = new HashSet<DoctorSchedule>();
        }

        public int DayId { get; set; }
        public string DayName { get; set; }

        public ICollection<DoctorSchedule> DoctorSchedule { get; set; }
    }
}
