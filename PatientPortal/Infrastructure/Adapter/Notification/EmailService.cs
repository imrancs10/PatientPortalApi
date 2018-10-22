using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using PatientPortal.Infrastructure.Utility;

namespace PatientPortal.Infrastructure
{
    public class EmailService : IMessageSystem
    {
        public void Send(Message msg)
        {
            //notify user via email
            string hostEmail = Convert.ToString(ConfigurationManager.AppSettings["HostEmail"]);
            string HostEmailName = Convert.ToString(ConfigurationManager.AppSettings["HostEmailName"]);
            string HostEmailPassword = Convert.ToString(ConfigurationManager.AppSettings["HostEmailPassword"]);
            string HostAddress = Convert.ToString(ConfigurationManager.AppSettings["HostAddress"]);
            int HostPort = Convert.ToInt32(ConfigurationManager.AppSettings["HostPort"]);
            var fromAddress = new MailAddress(hostEmail, HostEmailName);
            var toAddress = new MailAddress(msg.MessageTo, string.IsNullOrEmpty(msg.MessageNameTo) ? "User" : msg.MessageNameTo);
            string subject = msg.Subject;
            string body = msg.Body;

            var smtp = new SmtpClient
            {
                Host = HostAddress,
                Port = HostPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, HostEmailPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.SendCompleted += (s, e) =>
                {
                    smtp.Dispose();
                    message.Dispose();
                };
                smtp.Send(message);
            }
        }
    }
}