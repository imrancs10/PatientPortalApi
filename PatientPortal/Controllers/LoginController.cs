using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using PatientPortal.BAL.Login;
using PatientPortal.Global;
using PatientPortal.Infrastructure.Authentication;

namespace PatientPortal.Controllers
{
    public class LoginController : CommonController
    {
        // GET: Login
        public ActionResult Index()
        {
            ViewData["LoginPage"] = true;
            return View();
        }

        public ActionResult GetLogin(string username, string password)
        {
            LoginDetails _details = new LoginDetails();
            string _response = string.Empty;
            Enums.LoginMessage message = _details.GetLogin(username, password);
            _response = LoginResponse(message);
            if (message == Enums.LoginMessage.Authenticated)
            {
                //setUserClaim();
                return RedirectToAction("AddDepartments", "Masters");
            }
            else
            {
                SetAlertMessage(_response, "Login Response");
                return View("index");
            }
        }

        private void setUserClaim()
        {
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
            serializeModel.Id = UserData.UserId;
            serializeModel.FirstName = string.IsNullOrEmpty(UserData.FirstName) ? string.Empty : UserData.FirstName;
            serializeModel.MiddleName = string.IsNullOrEmpty(UserData.MiddleName) ? string.Empty : UserData.MiddleName;
            serializeModel.LastName = string.IsNullOrEmpty(UserData.LastName) ? string.Empty : UserData.LastName;
            serializeModel.LastName = string.IsNullOrEmpty(UserData.Username) ? string.Empty : UserData.Username;
            serializeModel.Email = string.IsNullOrEmpty(UserData.Email) ? string.Empty : UserData.Email;

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(serializeModel);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     UserData.Email,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(15),
                     false,
                     userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }
    }
}