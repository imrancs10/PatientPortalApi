using AsyncPoco;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnSolve.Mobile.Data;
using OnSolve.Mobile.Data.Entites;
using OnSolve.Mobile.Web.Infrastructure.Utilities;
using OnSolve.Mobile.Web.Models;
using OnSolve.Mobile.Web.Services;
using OnSolve.Mobile.Web.Services.Interface;
using System;
using System.Threading.Tasks;

namespace OnSolve.Mobile.Web.Test.Services.Login
{
    [TestClass]
    public class UserServiceTest
    {
        Mock<IDatabase> _swn402db;
        Mock<IPasswordHasherService> _passwordHasherService;
        Func<OnSolveMobileContext> _dbContextProvider;
        Mock<IContactsService> _contactsService;
        IUserService _userService;


        [TestInitialize]
        public void TestInitialize()
        {
            _swn402db = new Mock<IDatabase>();
            _passwordHasherService = new Mock<IPasswordHasherService>();
            _contactsService = new Mock<IContactsService>();
        }

        [TestMethod]
        public async Task CreateUser_CreatesAUser()
        {
            _dbContextProvider = GetContext;
            _userService = new UserService(_swn402db.Object, _passwordHasherService.Object, _dbContextProvider, _contactsService.Object);
            var userDetails = new UserRequest()
            {
                AccountId = 1,
                RecipientId = 1,
                EmailAddress = "test@gmail.com",
                Password = "Passw0rd"
            };
            _contactsService.Setup(x => x.VerifyRecipientAccount(1, 1))
                .ReturnsAsync(true);

            var actualResult = await _userService.CreateUser(userDetails);

            actualResult.CreatedAtId.Should().NotBe(0);
        }

        [TestMethod]
        public async Task ValidateRegistrationDetails_WhenInvalidContactPointOrAccount()
        {
            _dbContextProvider = GetContext;
            _userService = new UserService(_swn402db.Object, _passwordHasherService.Object, _dbContextProvider, _contactsService.Object);
            var userDetails = GetDummyUser();
            _contactsService.Setup(x => x.VerifyRecipientAccount(1, 1))
                .ReturnsAsync(false);

            var actualResult = await _userService.AreRegistrationDetailsValid(userDetails);

            actualResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateRegistrationDetails_WhenRecipientIsExisting()
        {
            _dbContextProvider = GetContextWithExistingRecipient;
            _userService = new UserService(_swn402db.Object, _passwordHasherService.Object, _dbContextProvider, _contactsService.Object);
            var userDetails = GetDummyUser();

            var actualResult = await _userService.AreRegistrationDetailsValid(userDetails);

            actualResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateRegistrationDetails_WhenEmailIsExisting()
        {
            _dbContextProvider = GetContextWithExistingEmail;
            _userService = new UserService(_swn402db.Object, _passwordHasherService.Object, _dbContextProvider, _contactsService.Object);
            var userDetails = GetDummyUser();

            var actualResult = await _userService.AreRegistrationDetailsValid(userDetails);

            actualResult.Should().BeFalse();
        }

        private UserRequest GetDummyUser()
        {
            return new UserRequest()
            {
                AccountId = 1,
                RecipientId = 1,
                EmailAddress = "test@gmail.com",
                Password = "Passw0rd"
            };
        }

        private static DbContextOptions<OnSolveMobileContext> GetDBOptions()
        {
            return new DbContextOptionsBuilder<OnSolveMobileContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private OnSolveMobileContext GetContext()
        {
            return new OnSolveMobileContext(GetDBOptions());
        }

        private OnSolveMobileContext GetContextWithExistingRecipient()
        {
            var dbContext = new OnSolveMobileContext(GetDBOptions());
            dbContext.MobileUser.Add(new MobileUser()
            {
                RecipientId = 1,
                Username = "test@gmail.com"
            });
            dbContext.SaveChanges();
            return dbContext;
        }

        private OnSolveMobileContext GetContextWithExistingEmail()
        {
            var dbContext = new OnSolveMobileContext(GetDBOptions());
            dbContext.MobileUser.Add(new MobileUser()
            {
                RecipientId = 2,
                Username = "test@gmail.com",
                ENSUserId = 1,
                CreatedOn = DateTime.Now
            });
            dbContext.SaveChanges();
            return dbContext;
        }
    }
}
