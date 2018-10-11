using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientLoginHistory
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public DateTime? LoginDate { get; set; }
        public string Ipaddress { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
