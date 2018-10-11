using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class State
    {
        public State()
        {
            City = new HashSet<City>();
            PatientInfo = new HashSet<PatientInfo>();
        }

        public int StateId { get; set; }
        public string StateName { get; set; }

        public ICollection<City> City { get; set; }
        public ICollection<PatientInfo> PatientInfo { get; set; }
    }
}
