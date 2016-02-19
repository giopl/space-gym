using Gym_Membership.Services.Abstract;
using Gym_Membership.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym_Membership.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult ForgetPassword(string UserId, string EmailAddress)
        {
            try
            {
                IAdminService userService = new AdminService();
                bool result = false;
                if (!string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(EmailAddress))
                {
                    result = userService.ResetPassword(UserId, EmailAddress);
                }

                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception )
            {

                throw;

            }
        }

        public ActionResult ChangePassword(string UserId, string OldPassword, string NewPassword, string ConfirmNewPassword)
        {
            try
            {
                IAdminService userService = new AdminService();
                bool result = false;
                if (!string.IsNullOrWhiteSpace(UserId) && 
                    !string.IsNullOrWhiteSpace(OldPassword) && 
                    !string.IsNullOrWhiteSpace(NewPassword) &&
                    !string.IsNullOrWhiteSpace(ConfirmNewPassword) &&
                    (ConfirmNewPassword == NewPassword))
                {
                   // result = userService.ChangePassword(UserId, OldPassword, NewPassword);
                }

                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {

                throw;

            }
        }


    }
}