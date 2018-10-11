namespace OnSolve.Mobile.Data.Entites
{
    public class ConferenceBridgeDetail
    {
        public int Id { get; set; }
        public string ConferencePhoneNumber { get; set; }
        public string ParticipantCode { get; set; }
        public bool IsSendConferenceIdInMessage { get; set; }
        public bool IsSendConferencePhoneInMessage { get; set; }
    }
}
