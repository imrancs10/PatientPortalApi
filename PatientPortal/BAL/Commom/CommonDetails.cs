using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using PatientPortal.Models.Common;
using PatientPortal.Global;
using System.IO;

namespace PatientPortal.BAL.Commom
{
    public class CommonDetails
    {
        PatientPortalEntities _db = null;
        private string[] Months = new string[] { "January", "Febuary", "March", "April", "May", "June", "July", "August", "Sepetember", "October", "November", "December" };

        public List<DayModel> DaysList()
        {
            _db = new PatientPortalEntities();
            var _list = (from day in _db.DayMasters
                         select new DayModel
                         {
                             DayId = day.DayId,
                             DayName = day.DayName
                         }).ToList();
            return _list != null ? _list : new List<DayModel>();
        }

        public List<PatientModel> PatientSearch(string _searchValue)
        {
            _db = new PatientPortalEntities();
            return _db.PatientInfoes.Where(x => x.CRNumber.Contains(_searchValue) || x.RegistrationNumber.Contains(_searchValue) || x.MobileNumber.Contains(_searchValue) || x.LastName.Contains(_searchValue) || x.FirstName.Contains(_searchValue) || x.Email.Contains(_searchValue))
                                    .Select(x => new PatientModel { PatientId = x.PatientId, PatientName = x.FirstName + " " + x.LastName }).ToList();
        }

        public string ReportFileUpload(HttpPostedFileBase file, Enums.ReportType _type, string RefNo)
        {
            try
            {
                string baseUrl = AppDomain.CurrentDomain.BaseDirectory.ToString();
                string filepath = baseUrl;
                if (file.ContentLength > 0)
                {
                    filepath += "\\Reports";
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                        Directory.CreateDirectory(filepath + "\\Bill");
                        Directory.CreateDirectory(filepath + "\\Lab");
                    }
                    else
                    {
                        if (_type == Enums.ReportType.Bill)
                        {
                            filepath += "\\Bill";
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            filepath += "\\" + DateTime.Now.Year.ToString();
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            filepath += "\\" + Months[DateTime.Now.Month];
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                        }
                        else if (_type == Enums.ReportType.Lab)
                        {
                            filepath += "\\Lab";
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            filepath += "\\" + DateTime.Now.Year.ToString();
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            filepath += "\\" + Months[DateTime.Now.Month];
                            if (!Directory.Exists(filepath))
                            {
                                Directory.CreateDirectory(filepath);
                            }
                        }
                        filepath += "\\" + RefNo;
                        if (file.ContentLength > 0)
                        {
                            filepath += Path.GetExtension(file.FileName);
                            file.SaveAs(filepath);
                            return filepath;
                        }
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}