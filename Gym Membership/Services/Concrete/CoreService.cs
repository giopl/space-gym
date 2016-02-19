using Gym_Membership.Models;
using Gym_Membership.Repositories.Abstract;
using Gym_Membership.Repositories.Concrete;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Services.Concrete
{
    public class CoreService
    {

        ILog log = log4net.LogManager.GetLogger(typeof(CoreService));
        public void SaveAccessLog(AccessLog accesslog)
        {

            try
            {

                log.InfoFormat("[SaveAccessLog]");

                IAdminRepository repository = new AdminRepository();
                var success = repository.InsertAccessLog(accesslog);
                ///TODO: implement
                //var success = true;

                if (!success)
                    log.Warn("Error saving access log");


            }
            catch (Exception e)
            {
                log.ErrorFormat("SaveAccessLog error for operation : {0} ", accesslog.ToString());
                throw e;

            }

        }


    }
}