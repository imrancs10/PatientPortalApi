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
    public class DepartmentDetails
    {
        PatientPortalEntities _db = null;

        public Enums.CrudStatus SaveDept(string deptName)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _deptRow = _db.Departments.Where(x => x.DepartmentName.Equals(deptName)).FirstOrDefault();
            if (_deptRow == null)
            {
                Department _newDept = new Department();
                _newDept.DepartmentName = deptName;
                _db.Entry(_newDept).State = EntityState.Added;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Saved : Enums.CrudStatus.NotSaved;
            }
            else
                return Enums.CrudStatus.DataAlreadyExist;
        }
        public Enums.CrudStatus EditDept(string deptName, int deptId)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _deptRow = _db.Departments.Where(x => x.DepartmentID.Equals(deptId)).FirstOrDefault();
            if (_deptRow != null)
            {
                _deptRow.DepartmentName = deptName;
                _db.Entry(_deptRow).State = EntityState.Modified;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Updated : Enums.CrudStatus.NotUpdated;
            }
            else
                return Enums.CrudStatus.DataNotFound;
        }
        public Enums.CrudStatus DeleteDept(int deptId)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _deptRow = _db.Departments.Where(x => x.DepartmentID.Equals(deptId)).FirstOrDefault();
            if (_deptRow != null)
            {
                _db.Departments.Remove(_deptRow);
                //_db.Entry(_deptRow).State = EntityState.Deleted;
                _effectRow = _db.SaveChanges();
                return _effectRow > 0 ? Enums.CrudStatus.Deleted : Enums.CrudStatus.NotDeleted;
            }
            else
                return Enums.CrudStatus.DataNotFound;
        }

        public List<DepartmentModel> DepartmentList()
        {
            _db = new PatientPortalEntities();
            var _list = (from dept in _db.Departments
                         select new DepartmentModel
                         {
                             DeparmentName = dept.DepartmentName,
                             DepartmentId = dept.DepartmentID
                         }).ToList();
            return _list != null ? _list : new List<DepartmentModel>();
        }

        public Department GetDeparmentById(int deptId)
        {
            _db = new PatientPortalEntities();
            int _effectRow = 0;
            var _deptRow = _db.Departments.Where(x => x.DepartmentID.Equals(deptId)).FirstOrDefault();
            if (_deptRow != null)
            {
                return _deptRow;
            }
            return null;
        }
    }
}