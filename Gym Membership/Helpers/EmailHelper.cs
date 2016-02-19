using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace Gym_Membership.Helpers
{
    public class EmailHelper
    {
        public bool SendEmail(string email, string subject, string body)
        {

            /*
             * http://www.codeproject.com/Tips/520998/Send-Email-from-Yahoo-GMail-Hotmail-Csharp
             * Yahoo! 	smtp.mail.yahoo.com
             * GMail 	smtp.gmail.com
             * Hotmail 	smtp.live.com
             */

            string smtpAddress = ConfigurationHelper.SmtpServer();
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = ConfigurationHelper.WebEmail();
            string password = ConfigurationHelper.WebEmailPwd();
            string emailTo = email;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }

            return true;
        }
    }
}