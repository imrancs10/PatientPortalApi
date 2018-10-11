using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class AppointmentInfo
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateFrom { get; set; }
        public DateTime AppointmentDateTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public bool? Reminder { get; set; }

        public Doctor Doctor { get; set; }
        public PatientInfo Patient { get; set; }
    }
}
