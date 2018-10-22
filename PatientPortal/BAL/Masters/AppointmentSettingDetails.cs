using DataLayer;
using PatientPortal.Global;
using PatientPortal.Models.Masters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PatientPortal.BAL.Masters
{
    public class AppointmentSettingDetails
    {
        PatientPortalEntities _db;

        public Enums.CrudStatus SaveAppSetting(AppSettingModel _model)
        {
            try
            {
                _db = new PatientPortalEntities();
                var _appSetting = _db.AppointmentSettings.Where(x => x.IsActive).FirstOrDefault();
                if (_appSetting != null)
                {
                    _appSetting.IsActive = false;
                    _db.Entry(_appSetting).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                AppointmentSetting _newSetting = new AppointmentSetting();
                _newSetting.IsActiveAppointmentMessage = _model.IsActiveAppointmentMessage;
                _newSetting.IsActive = true;
                _newSetting.CreatedDate = DateTime.Now;
                _newSetting.CalenderPeriod = _model.CalenderPeriod;
                _newSetting.AutoCancelMessage = _model.AutoCancelMessage;
                _newSetting.AppointmentSlot = _model.AppointmentSlot;
                _newSetting.AppointmentMessage = _model.AppointmentMessage;
                _newSetting.AppointmentLimitPerUser = _model.AppointmentLimitPerUser;
                _newSetting.AppointmentCancelPeriod = _model.AppointmentCancelPeriod;
                _db.Entry(_newSetting).State = EntityState.Added;
                int _result = _db.SaveChanges();
                return _result > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
            }
            catch (Exception ex)
            {
                return Enums.CrudStatus.InternalError;
            }
        }

        public AppointmentSetting GetAppSetting()
        {
            _db = new PatientPortalEntities();
            return _db.AppointmentSettings.Where(x => x.IsActive).FirstOrDefault();
        }
    }
}