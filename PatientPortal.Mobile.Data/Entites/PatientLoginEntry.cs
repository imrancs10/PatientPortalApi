using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientLoginEntry
    {
        public int Id { get; set; }
        public int? LoginAttempt { get; set; }
        public bool? Locked { get; set; }
        public DateTime? LoginAttemptDate { get; set; }
        public int PatientId { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
