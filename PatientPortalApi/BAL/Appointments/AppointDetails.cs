﻿using DataLayer;
using PatientPortalApi.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PatientPortalApi.BAL.Appointments
{
    public class AppointDetails
    {
        PatientPortalApiEntities _db = null;
        public IEnumerable<object> DeptWiseDoctorScheduleList(int deptId = 0, int year = 0, int month = 0)
        {
            _db = new PatientPortalApiEntities();
            year = year == 0 ? DateTime.Now.Year : year;
            month = month == 0 ? DateTime.Now.Month : month;
            var _list = (from docSchedule in _db.DoctorSchedules
                         orderby docSchedule.DayID
                         where deptId == 0 || docSchedule.Doctor.DepartmentID.Equals(deptId)
                         select new
                         {
                             DayId = docSchedule.DayID,
                             docSchedule.DayMaster.DayName,
                             docSchedule.DoctorID,
                             docSchedule.Doctor.DoctorName,
                             docSchedule.Doctor.Department.DepartmentName,
                             docSchedule.DoctorScheduleID,
                             TimeFrom = docSchedule.TimeFrom + (docSchedule.TimeFromMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             TimeTo = docSchedule.TimeTo + (docSchedule.TimeToMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             docSchedule.TimeFromMeridiemID,
                             docSchedule.TimeToMeridiemID
                         }).GroupBy(x => x.DayName).ToList();
            return _list;
        }
        public IEnumerable<object> DayWiseDoctorScheduleList(int deptId, string day, DateTime? date)
        {
            _db = new PatientPortalApiEntities();
            var _docList = _db.DoctorLeaves.Where(x => date != null && x.LeaveDate == date).Select(x => x.DoctorId).ToList();
            var _list = (from docSchedule in _db.DoctorSchedules

                         orderby docSchedule.DoctorID
                         where docSchedule.Doctor.DepartmentID.Equals(deptId) && docSchedule.DayMaster.DayName.ToLower().Equals(day.ToLower())
                         && !_docList.Contains(docSchedule.DoctorID.Value)
                         select new
                         {
                             DayId = docSchedule.DayID,
                             docSchedule.DayMaster.DayName,
                             docSchedule.DoctorID,
                             docSchedule.Doctor.DoctorName,
                             docSchedule.Doctor.Department.DepartmentName,
                             docSchedule.DoctorScheduleID,
                             TimeFrom = docSchedule.TimeFrom + (docSchedule.TimeFromMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             TimeTo = docSchedule.TimeTo + (docSchedule.TimeToMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             docSchedule.TimeFromMeridiemID,
                             docSchedule.TimeToMeridiemID
                         }).GroupBy(x => x.DoctorName).ToList();
            return _list;
        }
        public IEnumerable<object> DayAndDoctorWiseDoctorScheduleList(int doctorId, string dayId, DateTime? date)
        {
            _db = new PatientPortalApiEntities();
            var _docList = _db.DoctorLeaves.Where(x => date != null && x.LeaveDate == date).Select(x => x.DoctorId).ToList();
            var _list = (from docSchedule in _db.DoctorSchedules

                         orderby docSchedule.DoctorID
                         where docSchedule.DoctorID.Equals(doctorId) && docSchedule.DayID.Equals(dayId.ToLower())
                         && !_docList.Contains(docSchedule.DoctorID.Value)
                         select new
                         {
                             DayId = docSchedule.DayID,
                             docSchedule.DayMaster.DayName,
                             docSchedule.DoctorID,
                             docSchedule.Doctor.DoctorName,
                             docSchedule.Doctor.Department.DepartmentName,
                             docSchedule.DoctorScheduleID,
                             TimeFrom = docSchedule.TimeFrom + (docSchedule.TimeFromMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             TimeTo = docSchedule.TimeTo + (docSchedule.TimeToMeridiemID == 1 ? ":00 AM" : ":00 PM"),
                             docSchedule.TimeFromMeridiemID,
                             docSchedule.TimeToMeridiemID
                         }).GroupBy(x => x.DoctorName).ToList();
            return _list;
        }
        public IEnumerable<object> DateWiseDoctorAppointmentList(DateTime date, int doctorId)
        {
            _db = new PatientPortalApiEntities();
            var _list = (from docAppointment in _db.AppointmentInfoes

                         orderby docAppointment.DoctorId
                         where DbFunctions.TruncateTime(docAppointment.AppointmentDateFrom) <= date
                               && DbFunctions.TruncateTime(docAppointment.AppointmentDateFrom) >= date.Date
                               && docAppointment.DoctorId == doctorId
                         select new
                         {
                             docAppointment.AppointmentDateFrom,
                             docAppointment.AppointmentDateTo,
                             docAppointment.AppointmentId,
                             docAppointment.DoctorId,
                             docAppointment.Doctor.DoctorName,
                             docAppointment.PatientId
                         }).OrderBy(x => x.AppointmentDateFrom).GroupBy(x => x.DoctorId).ToList();
            return _list;
        }
        public Enums.CrudStatus SaveAppointment(AppointmentInfo model)
        {
            try
            {
                if (model.PatientId < 1)
                    return Enums.CrudStatus.SessionExpired;
                _db = new PatientPortalApiEntities();
                int _effectRow = 0;
                int _deptRow = _db.AppointmentInfoes.Where(x => DbFunctions.TruncateTime(x.AppointmentDateFrom) == DbFunctions.TruncateTime(model.AppointmentDateFrom) && x.IsCancelled == false && x.PatientId.Equals(model.PatientId)).Count();
                var appSetting = _db.AppointmentSettings.Where(x => x.IsActive).FirstOrDefault();
                int AppointmentLimitPerUser = (appSetting != null) ? (appSetting.AppointmentLimitPerUser > 0) ? appSetting.AppointmentLimitPerUser : 1 : 1;
                if (_deptRow < AppointmentLimitPerUser)
                {
                    AppointmentInfo _newAppointment = new AppointmentInfo();
                    _newAppointment.AppointmentDateFrom = model.AppointmentDateFrom;
                    _newAppointment.AppointmentDateTo = model.AppointmentDateTo;
                    _newAppointment.CreatedDate = DateTime.Now;
                    _newAppointment.DoctorId = model.DoctorId;
                    _newAppointment.PatientId = model.PatientId;
                    _newAppointment.IsCancelled = false;
                    _db.Entry(_newAppointment).State = EntityState.Added;
                    _effectRow = _db.SaveChanges();
                    return _effectRow > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
                }
                else
                    return Enums.CrudStatus.DataAlreadyExist;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }
        public IEnumerable<object> PatientAppointmentList(int _patientId, int year = 0, int month = 0)
        {
            _db = new PatientPortalApiEntities();
            var _list = (from docAppointment in _db.AppointmentInfoes

                         orderby docAppointment.DoctorId
                         where docAppointment.PatientInfo.PatientId.Equals(_patientId) &&
                         (year == 0 || docAppointment.AppointmentDateFrom.Year == year) &&
                         (month == 0 || docAppointment.AppointmentDateFrom.Month == month)
                         select new
                         {
                             docAppointment.AppointmentDateFrom,
                             docAppointment.IsCancelled,
                             docAppointment.CancelDate,
                             docAppointment.CancelReason,
                             docAppointment.Doctor.DepartmentID,
                             docAppointment.Doctor.Department.DepartmentName,
                             docAppointment.AppointmentDateTo,
                             docAppointment.AppointmentId,
                             docAppointment.DoctorId,
                             docAppointment.Doctor.DoctorName,
                             docAppointment.PatientId,
                             CanCancel = DbFunctions.TruncateTime(docAppointment.AppointmentDateFrom) >= DbFunctions.TruncateTime(DateTime.Now) ? true : false,
                             PatientName = docAppointment.PatientInfo.FirstName + " " + docAppointment.PatientInfo.MiddleName + " " + docAppointment.PatientInfo.LastName
                         }).OrderBy(x => x.AppointmentDateFrom).ToList();
            return _list;
        }
        public string CancelAppointment(int _patientId, int _appId, string CancelReason = "")
        {
            int _priorCancelTime = 0;
            _db = new PatientPortalApiEntities();
            var appSetting = _db.AppointmentSettings.Where(x => x.IsActive).FirstOrDefault();
            int AppointmentCancelPeriod = appSetting.AppointmentCancelPeriod;
            if (AppointmentCancelPeriod == 0)
            {
                int.TryParse(Utility.GetAppSettingKey("AppointmentCancelInAdvanceMinuts"), out _priorCancelTime);
            }
            else
            {
                _priorCancelTime = AppointmentCancelPeriod;
            }
            string result = string.Empty;
            ;
            var app = _db.AppointmentInfoes.Where(x => x.PatientId.Equals(_patientId) && x.AppointmentId.Equals(_appId)).FirstOrDefault();
            if (app != null)
            {
                if (app.AppointmentDateFrom >= DateTime.Now.AddMinutes(_priorCancelTime))
                {
                    app.CancelDate = DateTime.Now;
                    app.CancelReason = CancelReason;
                    app.ModifiedBy = _patientId;
                    app.ModifiedDate = DateTime.Now;
                    app.IsCancelled = true;
                    _db.Entry(app).State = EntityState.Modified;
                    int _rowCount = _db.SaveChanges();
                    if (_rowCount > 0)
                        result = Enums.JsonResult.Success.ToString();
                    else
                        result = Enums.JsonResult.Unsuccessful.ToString();
                }
                else
                {
                    result = "You can't cancel the appointment " + _priorCancelTime + " minute(s) prior scheduled time";
                }
            }

            return result;
        }

        public int PatientAppointmentCount(int _patientId)
        {
            _db = new PatientPortalApiEntities();
            var _list = (from docAppointment in _db.AppointmentInfoes
                         where docAppointment.PatientInfo.PatientId.Equals(_patientId)
                         && docAppointment.AppointmentDateFrom > DateTime.Now
                         && (docAppointment.IsCancelled == false || docAppointment.IsCancelled == null)
                         select docAppointment
                         ).ToList();
            return _list.Count;
        }
    }
}