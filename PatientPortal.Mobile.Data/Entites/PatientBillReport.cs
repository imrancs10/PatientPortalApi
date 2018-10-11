using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientBillReport
    {
        public int BillId { get; set; }
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string BillType { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ReportUrl { get; set; }
        public int? PatientId { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
