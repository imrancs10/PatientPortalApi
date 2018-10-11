namespace PatientPortal.Mobile.Web.Models
{
    public class UserClaims
    {
        public int MobileUserId { get; set; }
        public int AccountId { get; set; }
        public long RecipientId { get; set; }
        public string MemberId { get; set; } = string.Empty;

    }
}
