using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Models
{
    public class ContactDetailModel
    {
        public int RecipientId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public string ENSUsername { get; set; }

        public int? ENSUserId { get; set; }

        public string UniqueId { get; set; }
    }
}
