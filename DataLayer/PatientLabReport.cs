//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class PatientLabReport
    {
        public int ReferanceId { get; set; }
        public string RefNo { get; set; }
        public string BillNo { get; set; }
        public string LabName { get; set; }
        public System.DateTime ReportDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime ModificationDate { get; set; }
        public string ReportUrl { get; set; }
        public Nullable<int> PatientId { get; set; }
    
        public virtual PatientInfo PatientInfo { get; set; }
    }
}