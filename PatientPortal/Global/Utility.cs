using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace PatientPortal.Global
{
    public static class Utility
    {
        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static string GetAppSettingKey(string key)
        {
            try
            {
                string result = ConfigurationManager.AppSettings.Get(key);
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
    }

    public static class WebSession
    {

        public static int PatientId
        {
            get { return HttpContext.Current.Session["PatientId"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["PatientId"]); }
            set { HttpContext.Current.Session["PatientId"] = value; }
        }
        public static int AppointmentSlot
        {
            get { return HttpContext.Current.Session["AppointmentSlot"] == null ?Convert.ToInt32(Utility.GetAppSettingKey("AppointmentPeriodInMinuts")) : Convert.ToInt32(HttpContext.Current.Session["AppointmentSlot"]); }
            set { HttpContext.Current.Session["AppointmentSlot"] = value; }
        }
        public static int CalenderPeriod
        {
            get { return HttpContext.Current.Session["CalenderPeriod"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["CalenderPeriod"]); }
            set { HttpContext.Current.Session["CalenderPeriod"] = value; }
        }
        public static string AppointmentMessage
        {
            get { return HttpContext.Current.Session["AppointmentMessage"] == null ? string.Empty : HttpContext.Current.Session["AppointmentMessage"].ToString(); }
            set { HttpContext.Current.Session["AppointmentMessage"] = value; }
        }
        public static bool IsActiveAppointmentMessage
        {
            get { return HttpContext.Current.Session["IsActiveAppointmentMessage"] == null ? false :Convert.ToBoolean(HttpContext.Current.Session["IsActiveAppointmentMessage"].ToString()); }
            set { HttpContext.Current.Session["IsActiveAppointmentMessage"] = value; }
        }
        public static int AppointmentLimitPerUser
        {
            get { return HttpContext.Current.Session["AppointmentLimitPerUser"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["AppointmentLimitPerUser"]); }
            set { HttpContext.Current.Session["AppointmentLimitPerUser"] = value; }
        }
        public static int AppointmentCancelPeriod
        {
            get { return HttpContext.Current.Session["ApoointmentCancelPeriod"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["ApoointmentCancelPeriod"]); }
            set { HttpContext.Current.Session["ApoointmentCancelPeriod"] = value; }
        }
        public static string AutoCancelMessage
        {
            get { return HttpContext.Current.Session["AutoCancelMessage"] == null ? string.Empty : HttpContext.Current.Session["AutoCancelMessage"].ToString(); }
            set { HttpContext.Current.Session["AutoCancelMessage"] = value; }
        }

        public static string PatientRegNo
        {
            get { return HttpContext.Current.Session["PatientRegNo"] == null ? string.Empty : HttpContext.Current.Session["PatientRegNo"].ToString(); }
            set { HttpContext.Current.Session["PatientRegNo"] = value; }
        }
        public static string PatientCRNo
        {
            get { return HttpContext.Current.Session["PatientCRNo"] == null ? string.Empty : HttpContext.Current.Session["PatientCRNo"].ToString(); }
            set { HttpContext.Current.Session["PatientCRNo"] = value; }
        }
        public static int PatientLedgerPeriodInMonth
        {
            get { return HttpContext.Current.Session["PatientLedgerPeriodInMonth"] == null ? Convert.ToInt32(Utility.GetAppSettingKey("PatientLedgerPeriodInMonth")) : Convert.ToInt32(HttpContext.Current.Session["PatientLedgerPeriodInMonth"].ToString()); }
        }
    }
}