using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    public class LicenseKeyException:HttpException
    {

        public LicenseKeyException()
        {
        }

        public LicenseKeyException(string message)
        : base(message)
    {
        }

        public LicenseKeyException(int httpCode,string message)
        : base(httpCode,message)
        {
        }

        public LicenseKeyException(string message, Exception inner)
        : base(message, inner)
    {
        }

    }
}