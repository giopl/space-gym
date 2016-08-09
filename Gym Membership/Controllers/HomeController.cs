using Gym_Membership.Helpers;
using Gym_Membership.Models;
using Gym_Membership.Services.Abstract;
using Gym_Membership.Services.Concrete;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym_Membership.Controllers
{
    public class HomeController : Controller
    {
        ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            //  log.Debug("Hello World from TS2!");
            //this is a change
            //return View();
            if (ConfigurationHelper.GetClikey() && ConfigurationManager.AppSettings["LicenseKey"] != Helpers.FingerPrint.Value())
            {
                return RedirectToAction("InvalidLicense");
            }


            return RedirectToAction("Login");

        }



        public ActionResult Login(LoginState.LoginStateEnum loginState = LoginState.LoginStateEnum.Not_Set)
        {
            try
            {
                
                LoginState state = new LoginState { LoginStateValue = loginState };
                return View(state);
            }
            catch (Exception e)
            {
                log.Error("[Login] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("ShowError", "Home");
            }

        }

        public ActionResult Error()
        {

            //var errLog = ViewBag.ErrorLog;
            var errLog = TempData["errorLog"] as ErrorLog;

            log.Debug("[ShowError]");

            //#if HOME
            //            //do not send email
            //#else


            //            if (ConfigurationHelper.IsSendErrorEmail() && ConfigurationHelper.GetEnvironment() != "HOME")
            //            {
            //                EmailHelper mm = new EmailHelper();

            //                //Generates the email body
            //                var mbb = RenderRazorViewToString(this.ControllerContext, "_emailError", errLog);


            //                var sendemailname = string.Concat("<", ConfigurationHelper.GetApplicatioName().Replace(" ", ""), "@mcb.local>");
            //                mm.SenderMail = new MailAddress(string.Format("{0} {1}", ConfigurationHelper.GetApplicatioName(), sendemailname));

            //                var env = ConfigurationHelper.GetEnvironment();

            //                string[] recipients = null;

            //                if (env == "PRODUCTION")
            //                {
            //                     recipients = ConfigurationHelper.GetSupportEmails();
            //                }
            //                else 
            //                {
            //                    recipients = ConfigurationHelper.GetSupportEmailsDevelopment();
            //                }


            //                foreach (var recipient in recipients)
            //                {
            //                    //GL: DP (defensive prg) in case there's the list ends in ; meaning the next recipient is empty
            //                    if (!string.IsNullOrWhiteSpace(recipient))
            //                    { 
            //                        mm.AddRecipient(recipient);

            //                    }
            //                }

            //                mm.SendMail(string.Format("{0} Error", ConfigurationHelper.GetApplicatioName()), mbb, true);
            //            }
            //        #endif
            return View(errLog);
        }

        protected static String RenderRazorViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }




        [HttpPost]
        public ActionResult Login(LoginState loginState)
        {
            log.Info("Login[Post]");
            try
            {

                loginState.Username = (!string.IsNullOrWhiteSpace(loginState.Username)) ? loginState.Username.ToLower() : string.Empty;
                loginState.Password = (!string.IsNullOrWhiteSpace(loginState.Password)) ? Utils.base64Encode(loginState.Password) : string.Empty;
                IAdminService loginservice = new AdminService();
                var result = loginservice.VerifyUser(loginState);


                //redirect correctly based on validation results
                if (result.LoginStateValue == LoginState.LoginStateEnum.Login_Successful)
                {
                    return RedirectToAction("Home", "User", new { isLogin = true });
                }
                else
                {
                    if (result.LoginStateValue == LoginState.LoginStateEnum.Password_Reset)
                    {
                        //Redirect to change password screen
                        UserSession.Current.ValidUser = true;
                        return RedirectToAction("ChangePassword", "User");
                    }
                    else
                    {
                        //return RedirectToAction("Login", "Home", new { err = "Username or password invalid" });
                        return View(result);
                    }
                }


            }
            catch (Exception e)
            {
                log.Error("[login-html] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult AjaxSessionExpired()
        {

            log.Info("AjaxSessionExpired");
            try
            {

                var cookiename = "LeadsUserSettings";
                HttpCookie aCookie = Request.Cookies[cookiename];
                int minslogged = 0;

                string userId = string.Empty;
                if (aCookie != null)
                {
                    //object userSettings = aCookie.Value;
                    var dt = (string)aCookie["lastlogin"];
                    userId = (string)aCookie["userId"];


                    DateTime intime = DateTime.Now;
                    if (DateTime.TryParse(dt, out intime))
                    {
                        DateTime endTime = DateTime.Now;
                        TimeSpan span = endTime.Subtract(intime);
                        minslogged = span.Minutes;
                    }
                }


                IAdminService adminService = new AdminService();
                adminService.SaveAccessLog(new AccessLog { Username = userId, Operation = "SESSION EXPIRED", Details = string.Format("User Session Expired - Session Time Mins: {0}", minslogged) });

                //UserSession.Current.ValidUser = false;
                UserSession.Current.ClearSession();
                return JavaScript("goHome()");

            }
            catch (Exception e)
            {
                log.ErrorFormat("Logout failed with error: {0}", e.ToString());
                throw;
            }

        }


        public ActionResult About()
        {
            return View();
        }


        /// <summary>
        /// Method to destrou UserSession
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            log.Info("Logout[Post]");
            try
            {



                //clears user session
                UserSession.Current.ClearSession();

                return RedirectToAction("Login", "Home", null);
            }
            catch (Exception e)
            {
                log.ErrorFormat("Logout failed with error: {0}", e.ToString());
                throw;
            }
        }


        /// <summary>
        /// Method to destrou UserSession
        /// </summary>
        /// <returns></returns>
        public ActionResult InvalidLicense()
        {
            log.Info("InvalidLicense");
            try
            {
                //clears user session
                UserSession.Current.ClearSession();

                return View();
            }
            catch (Exception e)
            {
                log.ErrorFormat("Logout failed with error: {0}", e.ToString());
                throw;
            }
        }
    }
}