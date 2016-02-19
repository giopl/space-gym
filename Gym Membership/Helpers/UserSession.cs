using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    public class UserSession
    {

        private UserSession()
        {
            //generics
            ValidUser = false;
            Username = string.Empty;
            Fullname = string.Empty;
            CommonName = string.Empty;
            ReadOnlyMode = false;
            EmployeeNumber = string.Empty;
            ExternalEmail = string.Empty;

            UserId = 0;
        }



        public static UserSession Current
        {
            get
            {
                try
                {
                    UserSession session = (UserSession)HttpContext.Current.Session["__MySession__"];
                    if (session == null)
                    {
                        session = new UserSession();
                        HttpContext.Current.Session["__MySession__"] = session;
                    }
                    return session;
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public string EmployeeNumber { get; set; }
        public string EncryptedPassword { get; set; }

        //public bool IsTeamLeader { get; set; }
        public bool ValidUser { get; set; }
        public bool ReadOnlyMode { get; set; }
        public string Fullname { get; set; }
        public string CommonName { get; set; }
        public string Username { get; set; }

        public int UserId { get; set; }
        public int UserAccessLevel { get; set; }

        public DateTime? DateViewed { get; set; }



        /// <summary>
        /// if there is a filter, verifies if the logged in user is the selected user by filter
        /// else if 
        /// </summary>

        public string WindowsUser
        {
            get
            {
                return Environment.UserName;
            }
        }

        public bool IsUser
        {
            get
            {
                return UserAccessLevel == 0;
            }

        }



        public bool IsAdmin
        {
            get
            {
                return UserAccessLevel == 2;
            }

        }


        public string ExternalEmail { get; set; }

        public string LocalEmail
        {
            get
            {
                return string.IsNullOrEmpty(Username) ? string.Empty : String.Concat(Username, "@mcb.local");

            }

        }





        /// <summary>
        /// Clears the current session.
        /// </summary>
        public void ClearSession()
        {
            HttpContext.Current.Session["__MySession__"] = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }


    }
}