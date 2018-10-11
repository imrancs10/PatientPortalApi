using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Models
{
    public class ENSLoginResponse
    {
        public int ENSUserId { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int RecipientId { get; set; }
    }
}
