using AsyncPoco;
using OnSolve.Mobile.Data.Enum;

namespace OnSolve.Mobile.Web.Swn402Entities
{
    public class ContactDetail
    {
        [Column(Name = "recipient_id")]
        public int RecipientId { get; set; }

        [Column("username")]
        public string ENSUsername { get; set; }

        [Column(Name = "user_id")]
        public int? ENSUserId { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column(Name = "account_name")]
        public string AccountName { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("locked_out")]
        public bool Locked { get; set; }

        [Column("email")]
        public string EmailId { get; set; }

        [Column("phone")]
        public string PhoneNumber { get; set; }

        [Column("member_id")]
        public System.Guid MemberId { get; set; }

        [Column(Name = "ClientsUniqueRecipientID")]
        public string UniqueId { get; set; }

        public UserType UserType => ENSUserId.HasValue ? UserType.ENS : UserType.ContactPoint;

        public string Name => $"{FirstName} {LastName}";

    }
}
