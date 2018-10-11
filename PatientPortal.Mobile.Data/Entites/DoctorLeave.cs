using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class DoctorLeave
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime LeaveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Doctor Doctor { get; set; }
    }
}
