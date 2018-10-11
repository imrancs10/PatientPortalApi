using AsyncPoco;

namespace OnSolve.Mobile.Web.Swn402Entities
{
    public class AccountDetails
    {
        [Column("account_name")]
        public string AccountName { get; set; }

        [Column("company")]
        public string CompanyName { get; set; }

    }
}
