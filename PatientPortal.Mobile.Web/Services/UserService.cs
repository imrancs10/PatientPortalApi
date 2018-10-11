using AsyncPoco;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Mobile.Data;
using PatientPortal.Mobile.Data.Entites;
using PatientPortal.Mobile.Web.Infrastructure.Utilities;
using PatientPortal.Mobile.Web.Models;
using PatientPortal.Mobile.Web.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientPortal.Mobile.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IDatabase _swn402Db;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly Func<PatientPortalMobileContext> _dbContextProvider;
        private const int ContactType_ExpressText = 1002;
        private const int ContactType_MobileText = 2002;
        private const int ContactType_MobileVoice = 2001;
        private const string ContactTypeLabel_ExpressText = "Express Messenger";
        private const string ContactTypeLabel_MobileText = "Mobile Text";
        private const string ContactTypeLabel_MobileVoice = "Mobile Voice";

        public UserService(IDatabase swn402Db, IPasswordHasherService passwordHasherService
            , Func<PatientPortalMobileContext> dbContextProvider)
        {
            _swn402Db = swn402Db;
            _passwordHasherService = passwordHasherService;
            _dbContextProvider = dbContextProvider;
        }

        //public async Task<MobileUser> GetMobileUser(string username)
        //{
        //    using (var dbContext = _dbContextProvider())
        //    {
        //        return await dbContext.MobileUser.FirstOrDefaultAsync(x => x.Username == username);
        //    }
        //}
    }
}
