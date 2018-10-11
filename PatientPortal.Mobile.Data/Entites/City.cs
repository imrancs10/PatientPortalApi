using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class City
    {
        public City()
        {
            PatientInfo = new HashSet<PatientInfo>();
        }

        public int CityId { get; set; }
        public string CityName { get; set; }
        public int? StateId { get; set; }

        public State State { get; set; }
        public ICollection<PatientInfo> PatientInfo { get; set; }
    }
}
