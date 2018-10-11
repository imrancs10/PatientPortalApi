using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class PatientTransaction
    {
        public int PatientTransactionId { get; set; }
        public string OrderId { get; set; }
        public int? Amount { get; set; }
        public int? PatientId { get; set; }
        public string TransactionNumber { get; set; }
        public string ResponseCode { get; set; }
        public string StatusCode { get; set; }
        public DateTime? TransactionDate { get; set; }

        public PatientInfo Patient { get; set; }
    }
}
