using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Infrastructure
{
    public class SendMessageStrategyForSMS : ISendMessageStrategy
    {
        private readonly Message _msg;
        public SendMessageStrategyForSMS(Message msg)
        {
            _msg = msg;
        }
        public void SendMessages()
        {
            SMSService objSms = new SMSService();
            objSms.Send(_msg);
        }
    }
}