using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ErrorLog
    {

        bool errorDebug = Helpers.ConfigurationHelper.ErrorDebug();



        public string SupportEmail
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Helpers.ConfigurationHelper.GetSupportEmailGroup()))
                    return string.Empty;

                return Helpers.ConfigurationHelper.GetSupportEmailGroup();

            }
        }

        public Exception AppException { get; set; }
        public string ExceptionDesc { get; set; }

        public ErrorLog(Exception e)
        {

            this.AppException = e;
            DisplayError = errorDebug;
        }

        public enum ErrorCodeNum
        {
            ApplicationError = 0,
            DatabaseError
        }


        public string FullDescription
        {
            get
            {
                return Utils.ValidateString(AppException);
            }
        }


        public string InnerException
        {
            get
            {
                return Utils.ValidateString(AppException.InnerException);
            }
        }


        public string Source
        {
            get
            {
                return Utils.ValidateString(AppException.Source);
            }
        }

        public string Message
        {
            get { return Utils.ValidateString(AppException.Message); }
        }


        public bool DisplayError { get; set; }

        public ErrorCodeNum ErrorCode { get; set; }


        public string GetErrorDescription()
        {

            var en = this.ErrorCode;

            Type type = en.GetType();

            System.Reflection.MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)

                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;

            }

            return en.ToString();

        }


    }
}