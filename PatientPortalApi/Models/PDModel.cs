using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortalApi.Models
{
    public class PDModel
    {
        public string totOPD { get; set; }
        public string totIPD { get; set; }
        public string CurrBal { get; set; }
        public string patStatus { get; set; }
    }
}