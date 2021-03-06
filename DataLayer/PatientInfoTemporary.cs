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
    
    public partial class PatientInfoTemporary
    {
        public int PatientId { get; set; }
        public string RegistrationNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MaritalStatus { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public Nullable<int> CityId { get; set; }
        public string Country { get; set; }
        public Nullable<int> PinCode { get; set; }
        public string Religion { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string OTP { get; set; }
        public Nullable<int> StateId { get; set; }
        public byte[] Photo { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string ResetCode { get; set; }
        public string CRNumber { get; set; }
        public Nullable<System.DateTime> ValidUpto { get; set; }
        public string AadharNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string RenewalStatusHIS { get; set; }
        public string RegistrationStatusHIS { get; set; }
        public Nullable<decimal> pid { get; set; }
        public string Location { get; set; }
        public string LoginPin { get; set; }
        public string DeviceIdentityfier { get; set; }
    
        public virtual City City { get; set; }
        public virtual Department Department { get; set; }
        public virtual State State { get; set; }
    }
}
