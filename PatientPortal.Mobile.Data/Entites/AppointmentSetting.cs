using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class AppointmentSetting
    {
        public int Id { get; set; }
        public int AppointmentSlot { get; set; }
        public int CalenderPeriod { get; set; }
        public string AppointmentMessage { get; set; }
        public int AppointmentLimitPerUser { get; set; }
        public int AppointmentCancelPeriod { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string AutoCancelMessage { get; set; }
        public bool IsActiveAppointmentMessage { get; set; }
    }
}
