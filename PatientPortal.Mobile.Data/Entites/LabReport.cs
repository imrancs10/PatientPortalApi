using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class LabReport
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public string ReportName { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
