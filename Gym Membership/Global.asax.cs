using Gym_Membership.Controllers;
using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Gym_Membership
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            log4net.Config.XmlConfigurator.Configure();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

      

        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "HttpError404";
                        break;
                    case 500:
                        // server error
                        action = "HttpError500";
                        break;
                    default:
                        action = "General";
                        break;
                }

                // clear error on server
                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));


                // Transfer the user to the appropriate custom error page
                //HttpException lastErrorWrapper = Server.GetLastError() as HttpException;

                //HttpContext ctx = HttpContext.Current;
                //ctx.Response.Clear();


                //RouteData routeData = new RouteData();
                //routeData.Values.Add("controller", "Error");
                //routeData.Values.Add("action", "Error500");
                //routeData.Values.Add("Summary", "Error");
                //routeData.Values.Add("Description", lastErrorWrapper.Message);
                //IController controller = new ErrorController();

                //controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                //ctx.Server.ClearError();
            }
        }

        private void ErrorController()
        {
            throw new NotImplementedException();
        }
    }
}
