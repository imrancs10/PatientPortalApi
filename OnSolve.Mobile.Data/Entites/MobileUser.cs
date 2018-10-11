using System;
using System.Collections.Generic;

namespace OnSolve.Mobile.Data.Entites
{
    public class MobileUser
    {
        public int Id { get; set; }
        public long RecipientId { get; set; }
        public int AccountId { get; set; }
        public long? ENSUserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<MessageDetail> Messages { get; set; }
        public IEnumerable<FCMTokenInfo> FCMTokenInfo { get; set; }
    }
}
