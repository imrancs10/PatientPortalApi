using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;
using PatientPortal.Global;
using System.Data.Entity;
using PatientPortal.Models.Masters;

namespace PatientPortal.BAL.Masters
{
    public class ScheduleDetails
    {
        PatientPortalEntities _db = null;
        public Enums.CrudStatus SaveSchedule(ScheduleModel model)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _deptRow = _db.DoctorSchedules.Where(x =>x.DoctorScheduleID.Equals(model.ScheduleId)).FirstOrDefault();
            if (_deptRow == null)
            {
                DoctorSchedule _newDoc = new DoctorSchedule();
                _newDoc.DayID = model.DayId;
                _newDoc.DoctorID = model.DoctorId;
                _newDoc.TimeFrom = model.TimeFrom;
                _newDoc.TimeFromMeridiemID = model.TimeFromMeridiumId;
                _newDoc.TimeTo = model.TimeTo;
                _newDoc.TimeToMeridiemID = model.TimeToMeridiumId;
                _db.Entry(_newDoc).State = EntityState.Added;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
            }
            else
                return Enums.CrudStatus.DataAlreadyExist;
        }
        public Enums.CrudStatus EditSchedule(ScheduleModel model)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _docRow = _db.DoctorSchedules.Where(x => x.DoctorScheduleID.Equals(model.ScheduleId)).FirstOrDefault();
            if (_docRow != null)
            {
                _docRow.DayID = model.DayId;
                _docRow.DoctorID = model.DoctorId;
                _docRow.TimeFrom = model.TimeFrom;
                _docRow.TimeFromMeridiemID = model.TimeFromMeridiumId;
                _docRow.TimeTo = model.TimeTo;
                _docRow.TimeToMeridiemID = model.TimeToMeridiumId;
                _db.Entry(_docRow).State = EntityState.Modified;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Updated : Enums.CrudStatus.NotUpdated;
            }
            else
                return Enums.CrudStatus.DataNotFound;
        }
        public Enums.CrudStatus DeleteSchedule(int docId)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _docRow = _db.DoctorSchedules.Where(x => x.DoctorScheduleID.Equals(docId)).FirstOrDefault();
            if (_docRow != null)
            {
                _db.DoctorSchedules.Remove(_docRow);
                //_db.Entry(_deptRow).State = EntityState.Deleted;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Deleted : Enums.CrudStatus.NotDeleted;
            }
            else
                return Enums.CrudStatus.DataNotFound;
        }
        public IEnumerable<object> ScheduleList()
        {
            _db = new PatientPortalEntities();
            var _list = (from docSchedule in _db.DoctorSchedules
                         
                         orderby docSchedule.Doctor.DoctorName
                         select new 
                         {
                             DayId=docSchedule.DayID,
                             docSchedule.DayMaster.DayName,
                             docSchedule.DoctorID,
                             docSchedule.Doctor.DoctorName,
                             docSchedule.Doctor.Department.DepartmentName,
                             docSchedule.DoctorScheduleID,
                             TimeFrom=docSchedule.TimeFrom + (docSchedule.TimeFromMeridiemID==1?" AM":" PM"),
                             TimeTo=docSchedule.TimeTo + (docSchedule.TimeToMeridiemID == 1 ? " AM" : " PM"),
                             docSchedule.TimeFromMeridiemID,
                             docSchedule.TimeToMeridiemID 
                         }).ToList();
            return _list;
        }
    }
}