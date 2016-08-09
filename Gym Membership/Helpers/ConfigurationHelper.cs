using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    public class ConfigurationHelper
    {



        public static string GetEnvironment()
        {
            return ConfigurationManager.AppSettings["Environment"];
        }

        


        public static string GetApplicatioName()
        {
            return ConfigurationManager.AppSettings["ApplicationName"];
        }




        #region "connection strings"

        public static string ConnectionStringLocal()
        {
            return ConfigurationManager.AppSettings["ConnectionStringLocal"];
        }
        public static string ConnectionStringDebug()
        {
            return ConfigurationManager.AppSettings["ConnectionStringDebug"];
        }
        public static string ConnectionStringRelease()
        {
            return ConfigurationManager.AppSettings["ConnectionStringRelease"];
        }

        public static string ConnectionStringTest()
        {
            return ConfigurationManager.AppSettings["ConnectionStringTest"];
        }

        #endregion

        public static bool LogQueries()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["LogQueries"]);
        }

        public static bool GetClikey()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["clikey"]);
        }


        public static bool Incognito()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["Incognito"]);
        }

        public static bool InsertMemberId()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["InsertMemberId"]);
        }


        public static string AdminEmail()
        {
            return ConfigurationManager.AppSettings["AdminEmail"];
        }

        public static string WebEmail()
        {
            return ConfigurationManager.AppSettings["WebappMail"];
        }

        public static int MaxMembersAllowed()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["MaxMembersAllowed"]);
        }


        public static int BoundaryLimitForVoucherId()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["BoundaryLimitForVoucherId"]);
        }


        public static string GetReportFolder()
        {
            return ConfigurationManager.AppSettings["ReportFolder"];
        }




        public static int GetLongOverdueDays()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["LongOverdueDays"]);
        }


        public static int GetHistorySize()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["HistorySize"]);
        }



        public static double ProrataFirst()
        {
            return Convert.ToDouble(ConfigurationManager.AppSettings["ProrataFirst"]);
        }

        public static double ProrataSecond()
        {
            return Convert.ToDouble(ConfigurationManager.AppSettings["ProrataSecond"]);
        }


        public static double ProrataThird()
        {
            return Convert.ToDouble(ConfigurationManager.AppSettings["ProrataThird"]);
        }



        public static string WebEmailPwd()
        {
            return ConfigurationManager.AppSettings["WebappPwd"];
        }

        public static string SmtpServer()
        {
            return ConfigurationManager.AppSettings["smtpserver"];
        }

        public static bool ErrorDebug()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["ErrorDebug"]);
        }

        public static bool GetIsPasswordEnabled()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["PasswordEnabled"]);
        }

        public static string GetTestPassword()
        {
            return ConfigurationManager.AppSettings["TestPassword"];
        }

        public static bool UseCache()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["UseCache"]);
        }


        public static string[] GetSupportEmailsDevelopment()
        {

            return Convert.ToString(ConfigurationManager.AppSettings["SupportEmailDevelopment"]).Split(';');
        }


        public static string GetSaltKey()
        {
            return ConfigurationManager.AppSettings["SaltKey"];
        }

        public static string GetIzonePath()
        {
            return ConfigurationManager.AppSettings["IzonePath"];
        }
        public static bool FetchIzoneImage()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["FetchIzoneImage"]);
        }

        public static List<String> AuthorizedImagesExt()
        {
            List<String> result = new List<String>();
            string[] settings = ConfigurationManager.AppSettings["authorizedImagesExt"].ToString().Split(';');
            foreach (var item in settings)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    result.Add(item.ToLower());
                }
            }
            return result;
        }

        #region email


        public static bool SendEmail()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]);
        }

        public static bool SendErrorEmail()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["SendErrorEmail"]);
        }


        public static string GetSmtpHostIp()
        {
            return ConfigurationManager.AppSettings["SmtpHostIp"];
        }


        public static string GetSmtpHostDns()
        {
            return ConfigurationManager.AppSettings["SmtpHostDns"];
        }


        public static bool IsSendErrorEmail()
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings["SendErrorEmail"]);
        }

        public static string[] GetSupportEmails()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["SupportEmail"]).Split(';');
        }

        public static string GetSupportEmailGroup()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["SupportEmailGroup"]);
        }

        #endregion

    }
}