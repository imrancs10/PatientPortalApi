using DataLayer;
using PatientPortal.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PatientPortal.Infrastructure.Utility
{
    public class EmailHelper
    {
        public static string GetDeviceVerificationEmail(string firstname, string middlename, string lastname, string verificationCode)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, here is a OTP is : <b>" + verificationCode + "</b> you can use to verify your mobile number.<br/><br/>";
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }

        public static string GetRegistrationSuccessEmail(string firstname, string middlename, string lastname, string registrationnumber, string link)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, here registration is created, your registration number is : <b>" + registrationnumber + "</b> you can use to create your Password by clicking on below URL.<br/>";
            body += "<br/><b></b>" + link + "<br/><br/>";
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }

        public static string GetRegistrationSuccessEmailRenew(string firstname, string middlename, string lastname, PatientTransaction transaction)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, your registration is Renew.<br/><br/>";
            body += "<br/><b>Transaction Referance No : </b>" + transaction.TransactionNumber + "";
            body += "<br/><b>Transaction Date : </b>" + transaction.TransactionDate + "";
            body += "<br/><b>Transaction Amount : </b>" + transaction.Amount + "";
            body += "<br/><br/>Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }

        public static string GetAppointmentSuccessEmail(string firstname, string middlename, string lastname,string doctorname,DateTime apptime,string deptname)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, here Appointment is booked, Please find the below Appointment details<br/>";
            body += "<br/>" + string.Format("Department Name : {0} <br/>", deptname);
            body += "<br/>" + string.Format("Doctor Name : {0} <br/>", doctorname);
            body += "<br/>" + string.Format("Appointment Time : {0} <br/>", apptime.ToString());
            if(WebSession.IsActiveAppointmentMessage && !string.IsNullOrEmpty(WebSession.AppointmentMessage))
            {
                body += "<br/>" + string.Format("Note : {0} <br/>", WebSession.AppointmentMessage) + "<br/><br/>";
            }
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }
        public static string GetDoctorAbsentEmail(string firstname, string middlename, string lastname, string doctorname, DateTime leaveDate, string deptname)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += string.Format("This just to inform you. Doctor {0} is not available on {1}. so your below appointment is cancelled.\n We request you to please book another appointment as per doctor availability<br/>",doctorname,leaveDate);
            body += "<br/>" + string.Format("Department Name : {0} <br/>", deptname);
            body += "<br/>" + string.Format("Doctor Name : {0} <br/>", doctorname);
            body += "<br/>" + string.Format("Appointment Time : {0} <br/>", leaveDate.ToString()) + "<br/><br/>";
            body += "<br/>" + string.Format("Appointment Status : <span style='color:red'>Cancelled</span><br/>") + "<br/><br/>";
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }

        public static string GetForgetPasswordEmail(string firstname, string middlename, string lastname, string registrationnumber, string link)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, you can use to create your Password by clicking on below URL.<br/>";
            body += "<br/><b></b>" +  link + "<br/><br/>";
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }

        public static string GetForgetUserIdEmail(string firstname, string middlename, string lastname, string registrationnumber)
        {
            string body = string.Format("Hi {0} {1} {2}<br/><br/>", firstname, middlename, lastname);
            body += "As you requested, your registration number is : <b>" + registrationnumber + "</b>.<br/><br/>";
            body += "Thank You,<br/>";
            body += "Patient Portal Information System Customer Support";
            return body;
        }
    }
}