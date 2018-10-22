using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models
{
    public class HISPatientInfoModel
    {
        public string Mobileno { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string Religion { get; set; }
        public int deptid { get; set; }
        public string Registrationnumber { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public string MaritalStatus { get; set; }
        public string DoR { get; set; }
        public string AadharNo { get; set; }
        public string ValidUpto { get; set; }

    }
}