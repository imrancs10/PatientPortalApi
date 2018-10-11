using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientLabReport
    {
        public int ReferanceId { get; set; }
        public string RefNo { get; set; }
        public string BillNo { get; set; }
        public string LabName { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ReportUrl { get; set; }
        public int? PatientId { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
