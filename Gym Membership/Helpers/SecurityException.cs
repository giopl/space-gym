using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    
    public class SecurityException : HttpException
    {
        public SecurityException()
        {
        }

        public SecurityException(string message)
            : base(message)
        {
        }

        public SecurityException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public SecurityException(string message, int code)
           : base(message, code)
        {
        }
    }
}