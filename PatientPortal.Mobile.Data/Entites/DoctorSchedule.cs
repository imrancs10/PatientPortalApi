using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class DoctorSchedule
    {
        public int DoctorScheduleId { get; set; }
        public int? DoctorId { get; set; }
        public int? DayId { get; set; }
        public int? TimeFrom { get; set; }
        public int? TimeFromMeridiemId { get; set; }
        public int? TimeTo { get; set; }
        public int? TimeToMeridiemId { get; set; }

        public DayMaster Day { get; set; }
        public Doctor Doctor { get; set; }
        public MeridiemMaster TimeFromMeridiem { get; set; }
        public MeridiemMaster TimeToMeridiem { get; set; }
    }
}
