using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PatientPortalApi.Models
{
    [XmlObjectWrapper(ElementName = "NewDataSet")]
    public class AppointmentsModel
    {
        [XmlElement(ElementName = "DoctorName")]
        public string DoctorName { get; set; }
        [XmlElement(ElementName = "DepartName")]
        public string DepartName { get; set; }
        [XmlElement(ElementName = "datescheduled")]
        public string datescheduled { get; set; }
        [XmlElement(ElementName = "fromtime")]
        public string fromtime { get; set; }
        [XmlElement(ElementName = "totime")]
        public string totime { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string AppointmentDate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string Slot { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public bool? Reminder { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PatientName { get; set; }
        public string Status { get; set; }
        public string srNumber { get; set; }
        public bool CanCancel { get; set; }
    }

    [XmlRoot(ElementName = "NewDataSet")]
    public class MyVisitModelList : List<AppointmentsModel>
    {

    }
}