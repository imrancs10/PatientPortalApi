using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models.Masters
{
    public class ScheduleModel
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public int DayId { get; set; }
        public int TimeFrom { get; set; }
        public int TimeFromMeridiumId { get; set; }
        public int TimeTo { get; set; }
        public int TimeToMeridiumId { get; set; }

    }
}