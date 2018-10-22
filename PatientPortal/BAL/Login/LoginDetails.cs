using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer;
using PatientPortal.Global;
using System.Data.Entity;

namespace PatientPortal.BAL.Login
{
    public class LoginDetails
    {
        PatientPortalEntities _db = null;

        /// <summary>
        /// Get Authenticate User credentials
        /// </summary>
        /// <param name="UserName">Username</param>
        /// <param name="Password">Password</param>
        /// <returns>Enums</returns>
        public Enums.LoginMessage GetLogin(string UserName, string Password)
        {
            string _passwordHash = Utility.GetHashString(Password);
            _db = new PatientPortalEntities();

            var _userRow = _db.Gbl_Master_User.Where(x => x.Username.Equals(UserName) && x.PasswordHash.Equals(_passwordHash) && x.IsDeleted == false).FirstOrDefault();

            if (_userRow != null)
            {
                var _userLogin = _userRow.Gbl_Master_Login.FirstOrDefault(x => x.IsDeleted == false);

                if (_userLogin != null)
                {
                    if (_userLogin.IsActive == false)
                        return Enums.LoginMessage.UserBlocked;
                    else if (_userLogin.IsBlocked)
                        return Enums.LoginMessage.UserInactive;
                    else
                    {
                        _userLogin.LastLogin = DateTime.Now;
                        _db.Entry(_userLogin).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                else
                {
                    Gbl_Master_Login _newLogin = new Gbl_Master_Login();
                    _newLogin.CreatedAt = DateTime.Now;
                    _newLogin.IsActive = true;
                    _newLogin.IsBlocked = false;
                    _newLogin.IsDeleted = false;
                    _newLogin.IsSync = false;
                    _newLogin.LastLogin = DateTime.Now;
                    _newLogin.UserId = _userRow.UserId;
                    _db.Entry(_newLogin).State = EntityState.Added;
                    _db.SaveChanges();
                }
                UserData.UserId = _userRow.UserId;
                UserData.Username = _userRow.Username;
                UserData.FirstName = _userRow.FirstName;
                UserData.MiddleName = _userRow.MiddleName;
                UserData.LastName = _userRow.LastName;
                UserData.Email = _userRow.EmailId;
                return Enums.LoginMessage.Authenticated;
            }
            else
                return Enums.LoginMessage.InvalidCreadential;
        }
    }
}