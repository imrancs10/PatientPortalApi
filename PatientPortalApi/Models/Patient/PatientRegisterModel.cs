using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models.Patient
{
    public class PatientRegisterModel
    {
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string mobilenumber { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string religion { get; set; }
        public string department { get; set; }
        public string FatherHusbandName { get; set; }
        public string MaritalStatus { get; set; }
        public string title { get; set; }
        public string aadharNumber { get; set; }

        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string ResponseCode { get; set; }
        public string StatusCode { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionDate { get; set; }
    }
}