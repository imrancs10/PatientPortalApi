using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models
{
    public class PatientInfoModel
    {
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string CityId { get; set; }
        public string Country { get; set; }
        public Nullable<int> PinCode { get; set; }
        public string Religion { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string OTP { get; set; }
        public string StateId { get; set; }
        public byte[] Photo { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string CRNumber { get; set; }
        public DateTime? ValidUpto { get; set; }
        public string MaritalStatus { get; set; }
        public string MaritalStatusLabel
        {
            get
            {
                return MaritalStatus == "S" ? "Single" : "Married";
            }
        }
        public string AadharNumber { get; set; }
        public string Title { get; set; }
        public string DoR { get; set; }
    }
}