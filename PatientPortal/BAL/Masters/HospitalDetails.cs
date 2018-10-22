using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using System.Data.Entity;
using PatientPortal.Global;
using PatientPortal.Models.Masters;

namespace PatientPortal.BAL.Masters
{
    public class HospitalDetails
    {
        PatientPortalEntities _db = null;

        public Enums.CrudStatus SaveHospital(HospitalDetail hospital)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _hospitalRow = _db.HospitalDetails.Where(x => x.HospitalName.Equals(hospital.HospitalName)).FirstOrDefault();
            if (_hospitalRow == null)
            {
                _db.Entry(hospital).State = EntityState.Added;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
            }
            else
                return Enums.CrudStatus.DataAlreadyExist;
        }

        public List<HospitalDetail> GetAllHospitalDetail()
        {
            _db = new PatientPortalEntities();
            var _hospitalRow = _db.HospitalDetails.ToList();
            if (_hospitalRow.Count > 0)
            {
                return _hospitalRow;
            }
            return null;
        }
        public HospitalDetail GetHospitalDetail()
        {
            _db = new PatientPortalEntities();
            var _hospitalRow = _db.HospitalDetails.FirstOrDefault();
            if (_hospitalRow != null)
            {
                return _hospitalRow;
            }
            return null;
        }
        public bool DeleteHospitalDetail(int Id)
        {
            _db = new PatientPortalEntities();
            var _hospitalRow = _db.HospitalDetails.Where(x => x.Id == Id).FirstOrDefault();
            if (_hospitalRow != null)
            {
                _db.HospitalDetails.Remove(_hospitalRow);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}