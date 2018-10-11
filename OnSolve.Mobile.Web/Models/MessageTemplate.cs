namespace OnSolve.Mobile.Web.Models
{
    public class MessageTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
        public string CultureNamesList { get; set; }
        public int StatusId { get; set; }
        public bool IsActive
        {
            get { return StatusId == 1; }
            set
            {
                StatusId = value ? 1 : 3;
            }
        }
        public string ResourceId { get { return Id.ToString(); } }
    }
}
