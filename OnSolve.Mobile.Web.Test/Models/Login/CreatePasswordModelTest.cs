using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnSolve.Mobile.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Test.Models.Login
{
    [TestClass]
    public class CreatePasswordModelTest
    {

        [TestMethod]
        public void CreatePasswordModel_SpaceTrim()
        {
            PasswordModel password = new PasswordModel { Password = " Onsolve@123 " };

            password.Password.Should().IsSameOrEqualTo("Onsolve@123");
        }

        [TestMethod]
        public void CreatePasswordModel_SpecialCharacterCheck()
        {
            PasswordModel password = new PasswordModel { Password = "Onsolve@$!%*#?&123" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CreatePasswordModel_SpecialCharacterOptionalCheck()
        {
            PasswordModel password = new PasswordModel { Password = "Onsolve123" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void CreatePasswordModel_LessThan8CharactersCheck()
        {
            PasswordModel password = new PasswordModel { Password = "Onsol12" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePasswordModel_CapitalCharactersCheck()
        {
            PasswordModel password = new PasswordModel { Password = "onsolve12" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePasswordModel_SmallCharactersCheck()
        {
            PasswordModel password = new PasswordModel { Password = "ONSOLVE12" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void CreatePasswordModel_NumericCheck()
        {
            PasswordModel password = new PasswordModel { Password = "ONSOLVEEEE" };
            var result = Validator.TryValidateObject(password, new ValidationContext(password, null, null), null, true);

            result.Should().BeFalse();
        }
    }
}
