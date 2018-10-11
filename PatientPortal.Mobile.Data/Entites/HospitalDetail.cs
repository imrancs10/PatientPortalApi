using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class HospitalDetail
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
        public byte[] HospitalLogo { get; set; }
        public bool? IsActive { get; set; }
    }
}
