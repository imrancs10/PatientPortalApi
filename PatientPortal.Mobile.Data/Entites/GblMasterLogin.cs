using System;
using System.Collections.Generic;

namespace PatientPortal.Mobile.Data.Entites
{
    public partial class GblMasterLogin
    {
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsActive { get; set; }
        public bool IsSync { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public GblMasterUser User { get; set; }
    }
}
