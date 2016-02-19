using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class LoginState
    {
        public bool IsValid { get; set; }

        public LoginStateEnum LoginStateValue { get; set; }

        public string Browser { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }


        public string GetErrorDescription()
        {
            return LoginStateValue.ToString().Replace('_', ' ');
        }

        public enum LoginStateEnum
        {

            Not_Set = 0,

            [Description("Login Successful")]
            Login_Successful,

            [Description("Either Username or Password or both are missing!")]
            Username_Or_Password_Missing,

            [Description("You are not Authorized to access this site!")]
            Unauthorized_Access,

            [Description("Your account is disabled. Please contact your admin!")]
            User_Access_Disabled,

            [Description("Your account is locked out. Please contact your admin!")]
            User_Account_Locked_Out,

            [Description("Authentication Failed, please re-try.")]
            Authentication_Failed,

            [Description("System Error, Please contact your admin.")]
            Application_Error,

            [Description("Your Password has expired, Please reset!")]
            Password_Expired,

            [Description("Your Password was reset!")]
            Password_Reset,

            [Description("Your session expired after a period of inactivity, please re-login.")]
            Session_Expired

        }

    }
}