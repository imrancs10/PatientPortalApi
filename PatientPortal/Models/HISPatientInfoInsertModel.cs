using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models
{
    public class HISPatientInfoInsertModel
    {
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string Religion { get; set; }
        public string DepartmentId { get; set; }
        public string State { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string CRNumber { get; set; }
        public string ValidUpto { get; set; }
        public string MaritalStatus { get; set; }
        public string Title { get; set; }
        public string Amount { get; set; }
        public string PatientTransactionId { get; set; }
        public string CreateDate { get; set; }

    }
}