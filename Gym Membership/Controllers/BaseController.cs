using Gym_Membership.Helpers;
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
    public class BaseController : Controller
    {
        // GET: Base

        ILog log = log4net.LogManager.GetLogger(typeof(BaseController));


        protected bool IsPostback
        {
            get { return Request.HttpMethod == "POST"; }
        }



        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            //check if session is valid

            if (ConfigurationHelper.GetClikey() && ConfigurationManager.AppSettings["LicenseKey"] != Helpers.FingerPrint.Value())
            {
                ctx.Result = RedirectToAction("Index", "Home");
            }



            if (!Helpers.UserSession.Current.ValidUser)
            {
                //check if it is an ajax request
                if (!Request.IsAjaxRequest())
                {
                    ctx.Result = RedirectToAction("Login", "Home");
                }
                else
                {
                    ctx.RequestContext.HttpContext.Response.StatusCode = 401;
                    //to test if the ajax call is redirected to this contentresult
                    //if it does not work, redirect to home and check in JS if status code is 401
                    ctx.Result = RedirectToAction("AjaxSessionExpired", "Home");
                }
            }
        }

        /// <summary>
        /// allows to send content of a view to a string
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
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



    }
}