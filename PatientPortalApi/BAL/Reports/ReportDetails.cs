using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DataLayer;
using PatientPortalApi.Global;
using PatientPortalApi.Models.Patient;

namespace PatientPortalApi.BAL.Reports
{
    public class ReportDetails
    {
        PatientPortalApiEntities _db = null;

        public List<PateintLeadger> GetBillReportData(int userId)
        {
            _db = new PatientPortalEntities();
            var patientInfo = _db.PatientInfoes.Where(x => x.PatientId == userId).FirstOrDefault();
            return _db.PateintLeadgers.Where(x => x.PId == patientInfo.pid).ToList();
        }

        public List<LabreportPdf> GetLabReportData(int userId)
        {
            _db = new PatientPortalEntities();
            var patientInfo = _db.PatientInfoes.Where(x => x.PatientId == userId).FirstOrDefault();
            return _db.LabreportPdfs.Where(x => x.pid == patientInfo.pid).ToList();
        }

        public List<PatientLedgerModel> GetPatientLedger(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            _db = new PatientPortalEntities();
            DateTime _period = DateTime.Now.AddMonths(-Convert.ToInt32(ConfigurationManager.AppSettings["RegistrationValidityInMonth"]));
            var patientInfo = _db.PatientInfoes.Where(x => x.PatientId == userId).FirstOrDefault();
            List<PateintLeadger> data = new List<PateintLeadger>();
            if (fromDate == null && toDate == null)
                data = _db.PateintLeadgers.Where(x => x.PId == patientInfo.pid && DbFunctions.TruncateTime(x.billdate) >= DbFunctions.TruncateTime(_period)).ToList();
            else
            {
                data = _db.PateintLeadgers.Where(x => x.PId == patientInfo.pid && DbFunctions.TruncateTime(x.billdate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(x.billdate) <= DbFunctions.TruncateTime(toDate)).ToList();
            }
            List<PatientLedgerModel> ledgerList = new List<PatientLedgerModel>();
            if (data != null)
            {
                foreach (var currentLedger in data)
                {
                    PatientLedgerModel newLedger = new PatientLedgerModel();
                    newLedger.Balance = currentLedger.subtotal.ToString();
                    newLedger.Date = currentLedger.billdate == null ? DateTime.Now.ToString("dd/MM/yyyy") : Convert.ToDateTime(currentLedger.billdate).ToString("dd/MM/yyyy");
                    newLedger.Description = getBillType(currentLedger.vtype);
                    newLedger.IPNo = currentLedger.ipno;
                    newLedger.Payment = Math.Round(currentLedger.netamt.Value, 2).ToString();
                    newLedger.Receipt = Math.Round(currentLedger.netamt.Value, 2).ToString();
                    newLedger.Type = currentLedger.vtype;
                    newLedger.VNo = currentLedger.vno;
                    newLedger.schemeid = Convert.ToString(currentLedger.schemeid);
                    newLedger.SaleType = !string.IsNullOrEmpty(currentLedger.saletype) ? currentLedger.saletype.ToUpper() : string.Empty;
                    ledgerList.Add(newLedger);
                }
            }
            return ledgerList;
        }

        private string getBillType(string billtype)
        {
            string desc = string.Empty;
            switch (billtype)
            {
                case "SV":
                    desc = "Procedure/Diagnostic Billing";
                    break;
                case "PH":
                    desc = "Pharmacy Billing-Refund";
                    break;
                case "GP":
                    desc = "Patient Payment";
                    break;
                case "GR":
                    desc = "Receipt from Patient";
                    break;
                case "PHR":
                    desc = "Pharmacy Return";
                    break;
                case "SR":
                    desc = "Sales Return";
                    break;
            }
            return desc;
        }
        public List<PatientTransaction> GetPaymentReceipt(int patientId)
        {
            _db = new PatientPortalEntities();
            _db.Configuration.LazyLoadingEnabled = false;
            var result = _db.PatientTransactions.Where(x => x.PatientId == patientId).OrderBy(x => x.TransactionDate).ToList();
            result.ForEach(x =>
            {
                x.StatusCode = x.StatusCode == "S" ? "Success" : "Fail";
            });
            return result;
        }
    }
}