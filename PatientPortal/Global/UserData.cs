using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientPortal.Global
{
    public static class UserData
    {
        public static int UserId
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["userid"] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session["userid"].ToString());
                }
                else
                    return 0;
            }
            set { HttpContext.Current.Session["userid"] = value; }
        }
        public static string Username
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["username"] != null)
                {
                    return HttpContext.Current.Session["username"].ToString();
                }
                else
                    return string.Empty;
            }
            set { HttpContext.Current.Session["username"] = value; }
        }
        public static string FirstName
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["FirstName"] != null)
                {
                    return HttpContext.Current.Session["FirstName"].ToString();
                }
                else
                    return string.Empty;
            }
            set { HttpContext.Current.Session["FirstName"] = value; }
        }
        public static string MiddleName
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["MiddleName"] != null)
                {
                    return HttpContext.Current.Session["MiddleName"].ToString();
                }
                else
                    return string.Empty;
            }
            set { HttpContext.Current.Session["MiddleName"] = value; }
        }
        public static string LastName
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["LastName"] != null)
                {
                    return HttpContext.Current.Session["LastName"].ToString();
                }
                else
                    return string.Empty;
            }
            set { HttpContext.Current.Session["LastName"] = value; }
        }
        public static string Email
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["Email"] != null)
                {
                    return HttpContext.Current.Session["Email"].ToString();
                }
                else
                    return string.Empty;
            }
            set { HttpContext.Current.Session["Email"] = value; }
        }

    }
}