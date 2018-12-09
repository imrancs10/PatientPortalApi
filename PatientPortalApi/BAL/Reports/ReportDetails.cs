using System;
using System.Collections.Generic;
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

        public List<PateintLeadger> GetBillReportData()
        {
            _db = new PatientPortalApiEntities();
            return _db.PateintLeadgers.ToList();
        }

        public Enums.CrudStatus SetBillReportData(int PatientId, string BillNo, string BillType, DateTime BillDate, string ReportUrl, decimal BillAmount)
        {
            _db = new PatientPortalApiEntities();
            PateintLeadger _report = new PateintLeadger();
            //_report.BillAmount = BillAmount;
            //_report.BillDate = BillDate;
            //_report.BillNo = BillNo;
            //_report.BillType = BillType;
            //_report.ReportUrl = ReportUrl;
            //_report.CreatedDate = DateTime.Now;
            //_report.ModificationDate = DateTime.Now;
            //_report.PatientId = PatientId;
            //_db.PatientBillReports.Add(_report);
            int _result = _db.SaveChanges();
            return _result > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
        }

        public Enums.CrudStatus SetLabReportData(int PatientId, string BillNo, string RefNo, string ReportUrl, string LabName, DateTime ReportDate)
        {
            _db = new PatientPortalApiEntities();
            PateintLeadger _report = new PateintLeadger();
            //_report.ReportDate = ReportDate;
            //_report.RefNo = RefNo;
            //_report.BillNo = BillNo;
            //_report.LabName = LabName;
            //_report.CreatedDate = DateTime.Now;
            //_report.ReportUrl = ReportUrl;
            //_report.ModificationDate = DateTime.Now;
            //_report.PatientId = PatientId;
            _db.PateintLeadgers.Add(_report);
            int _result = _db.SaveChanges();
            return _result > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
        }

        public List<PateintLeadger> GetLabReportData()
        {
            _db = new PatientPortalApiEntities();
            return _db.PateintLeadgers.ToList();
        }

        public List<PatientLedgerModel> GetPatientLedger()
        {
            _db = new PatientPortalApiEntities();
            DateTime _period = DateTime.Now.AddMonths(-WebSession.PatientLedgerPeriodInMonth);
            var data = _db.PateintLeadgers.Where(x => x.PId == WebSession.PatientId && DbFunctions.TruncateTime(x.billdate)>= DbFunctions.TruncateTime(_period)).ToList();
            List<PatientLedgerModel> ledgerList = new List<PatientLedgerModel>();

            if (data != null)
            {
                foreach (var currentLedger in data)
                {
                    PatientLedgerModel newLedger = new PatientLedgerModel();
                    newLedger.Balance = currentLedger.subtotal.ToString();
                    newLedger.Date = currentLedger.billdate == null ? DateTime.Now : Convert.ToDateTime(currentLedger.billdate);
                    newLedger.Description = currentLedger.remarks;
                    newLedger.IPNo = currentLedger.ipno;
                    newLedger.Payment = currentLedger.gramount.ToString();
                    newLedger.Receipt = currentLedger.netamt.ToString();
                    newLedger.Type = currentLedger.vtype;
                    newLedger.VNo = currentLedger.vno;
                    ledgerList.Add(newLedger);
                }
            }
            return ledgerList;
        }
    }
}