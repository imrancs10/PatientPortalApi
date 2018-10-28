using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace PatientPortal.Infrastructure
{
    public class SMSService : IMessageSystem
    {
        public void Send(Message msg)
        {
            string _smsSenderId = string.Empty;
            string _smsKey = string.Empty;
            if (msg.MessageType == MessageType.Appointment)
            {
                _smsSenderId = Global.Utility.GetAppSettingKey("SmsSenderIdAppointment");
                _smsKey = Global.Utility.GetAppSettingKey("SmsKeyAppointment");
            }
            else if (msg.MessageType == MessageType.OTP)
            {
                _smsSenderId = Global.Utility.GetAppSettingKey("SmsSenderIdOTP");
                _smsKey = Global.Utility.GetAppSettingKey("SmsKeyOTP");
            }

            WebRequest request = WebRequest.Create("http://sms.eteknovation.com/app/smsapi/index.php?key=" + _smsKey + "&routeid=6&type=text&contacts=" + msg.MessageTo + "&senderid=" + _smsSenderId + "&msg=" + msg.Body) as HttpWebRequest;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
        }
    }
}