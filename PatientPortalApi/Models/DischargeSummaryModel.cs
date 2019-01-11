using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PatientPortalApi.Models
{
    [XmlObjectWrapper(ElementName = "NewDataSet")]
    public class DischargeSummaryModel
    {
        public string ipno { get; set; }
        public string ipnoEncode { get; set; }
        public string iage { get; set; }
        public string idmy { get; set; }
        [XmlElement(ElementName = "AdmitDate")]
        public string AdmitDate { get; set; }
        [XmlElement(ElementName = "DischargeDate")]
        public string DischargeDate { get; set; }
        public string status { get; set; }
        public string admittime { get; set; }
        public string dischargedate1 { get; set; }
        [XmlElement(ElementName = "DoctorName")]
        public string DoctorName { get; set; }
        [XmlElement(ElementName = "BedNo")]
        public string BedNo { get; set; }
        [XmlElement(ElementName = "Matter")]
        public string Matter { get; set; }
        public string CRNumber { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string MobileNumber { get; set; }
    }

    [XmlRoot(ElementName = "NewDataSet")]
    public class DischargeSummaryModelList : List<DischargeSummaryModel>
    {

    }
    public class XmlObjectWrapperAttribute : Attribute
    {
        public string ElementName { get; set; }
    }
}