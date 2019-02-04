using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using System.Data.Entity;
using PatientPortalApi.Global;
using PatientPortalApi.Models.Masters;

namespace PatientPortalApi.BAL.Masters
{
    public class DepartmentDetails
    {
        PatientPortalApiEntities _db = null;

        public Enums.CrudStatus SaveDept(string deptName)
        {
            _db = new PatientPortalApiEntities();
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
            _db = new PatientPortalApiEntities();
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
            _db = new PatientPortalApiEntities();
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
            _db = new PatientPortalApiEntities();
            var _list = (from dept in _db.Departments
                         select new DepartmentModel
                         {
                             DeparmentName = dept.DepartmentName,
                             DepartmentId = dept.DepartmentID,
                             DepartmentUrl = dept.DepartmentUrl,
                             Description = dept.Description,
                             Image = dept.Image 
                         }).ToList();
            _list.ForEach(x =>
            {
                if (x.Image != null)
                {
                    x.ImageUrl = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(x.Image));
                    x.Image = null;
                }
            });
            return _list != null ? _list : new List<DepartmentModel>();
        }

        public Department GetDeparmentById(int deptId)
        {
            _db = new PatientPortalApiEntities();
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