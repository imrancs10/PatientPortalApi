using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientInfo
    {
        public PatientInfo()
        {
            AppointmentInfo = new HashSet<AppointmentInfo>();
            LabReport = new HashSet<LabReport>();
            PatientBillReport = new HashSet<PatientBillReport>();
            PatientLabReport = new HashSet<PatientLabReport>();
            PatientLoginEntry = new HashSet<PatientLoginEntry>();
            PatientLoginHistory = new HashSet<PatientLoginHistory>();
            PatientTransaction = new HashSet<PatientTransaction>();
        }

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
        public DateTime? Dob { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public int? City { get; set; }
        public string Country { get; set; }
        public int? PinCode { get; set; }
        public string Religion { get; set; }
        public int? DepartmentId { get; set; }
        public string Otp { get; set; }
        public int? State { get; set; }
        public byte[] Photo { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string ResetCode { get; set; }
        public string Crnumber { get; set; }
        public DateTime? ValidUpto { get; set; }

        public City CityNavigation { get; set; }
        public Department Department { get; set; }
        public PatientInfo Patient { get; set; }
        public State StateNavigation { get; set; }
        public PatientInfo InversePatient { get; set; }
        public ICollection<AppointmentInfo> AppointmentInfo { get; set; }
        public ICollection<LabReport> LabReport { get; set; }
        public ICollection<PatientBillReport> PatientBillReport { get; set; }
        public ICollection<PatientLabReport> PatientLabReport { get; set; }
        public ICollection<PatientLoginEntry> PatientLoginEntry { get; set; }
        public ICollection<PatientLoginHistory> PatientLoginHistory { get; set; }
        public ICollection<PatientTransaction> PatientTransaction { get; set; }
    }
}
