using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Models.Common
{
    public class CommonModel
    {
    }
    public class DayModel
    {
        public int DayId { get; set; }
        public string DayName { get; set; }
    }

    public class  PatientModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }

    }
}