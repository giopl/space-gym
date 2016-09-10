using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Gym_Membership.Helpers
{
    public class Utils
    {


        /*
 * Method checks whether an object is null if so returns a string.empty 
 * else return the objects toString() value
 * Param: Object
 */
        public static string ValidateString(Object myObject)
        {
            if (null == myObject)
                return string.Empty;
            return myObject.ToString();
        }


        /// <summary>
        /// calculates the number of months between two dates
        /// http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int NumberOfMonthsBetweenDates(DateTime start, DateTime end)
        {
            return ((end.Year - start.Year) * 12) + end.Month - start.Month;
        }




        /// <summary>
        /// tries to parse string to int, if fails return 0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ParseStringToInt(string s)
        {

            int i = 0;
            Int32.TryParse(s, out i);
            return i;
        }

        /// <summary>
        /// returns the last day of month, i.e 04-09-2014 --> 30-09-2014
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(DateTime dt)
        {
            int daysInMth = DateTime.DaysInMonth(dt.Year, dt.Month);
            return new DateTime(dt.Year, dt.Month, daysInMth);
        
        }

        public static DateTime GetFirstDayOfMonth(DateTime dt)
        {
            int daysInMth = DateTime.DaysInMonth(dt.Year, dt.Month);
            return new DateTime(dt.Year, dt.Month, 1);

        }


        public static string DaysSinceDescription(int days)
        {
            string r = string.Empty;
            switch (days)
            {
                case 0: r = "Today"; break;
                case 1: r = "Yesterday"; break;
                case 7: r = "One week"; break;
                case 14: r = "Two week"; break;



                default:
                    r = string.Format("{0} days", days);

                    break;
            }
            return r;
            
        }


        /// <summary>
        /// add thousand separator to number, e.g 1000000 --> 1,000,000
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string AddThousandSeparator(double number)
        {
            var intNumber = Convert.ToInt32(number);
            return AddThousandSeparator(intNumber);
        }

        public static string AddThousandSeparator(int number)
        {
            return string.Format("{0:N2}", number);

        }


        /// <summary>
        /// Converts a date to an integer (datekey) so that it can be used for sorting.
        /// ex 05-May-2014 --> 20140505 as an integer
        /// </summary>
        /// <param name="dt">DateTime parameter</param>
        /// <returns></returns>
        public static int ConvertToDateKey(DateTime dt)
        {
            var result = 0;
            if (dt == null || dt == DateTime.MinValue)
                return result;

            result = dt.Year * 10000 + dt.Month * 100 + dt.Day;

            return result;
        }

        public static string HoursSince(DateTime dt)
        {
            StringBuilder result = new StringBuilder();
            if (dt == DateTime.MinValue)
            {
                result.Append("0h");
                return result.ToString();
            }
            var now = DateTime.Now;
            TimeSpan span = now - dt;

            return result.AppendFormat("{0}h", (int)span.TotalHours).ToString();

        }

        public static string ToDurationDescription(DateTime dt)
        {
            StringBuilder result = new StringBuilder();
            if (dt == DateTime.MinValue)
                return result.ToString();

            var now = DateTime.Now;
            TimeSpan span = now - dt;

            if (span.TotalHours <= 24 && now.Hour <= 23)
            {
                result.Append("Today at ");
                //3 days ago at 7:58 pm - 10.06.2014
            }

            if (span.TotalHours <= 24 && dt.Hour > now.Hour)
            {
                result.Append("Yesterday at ");
                //3 days ago at 7:58 pm - 10.06.2014
            }

            if (span.TotalHours >= 24 && span.TotalHours <= 48)
            {
                result.Append("1 day ago at ");
                //3 days ago at 7:58 pm - 10.06.2014
            }


            if (span.TotalHours > 48)
            {
                result.AppendFormat("{0} days ago at ", (int)span.TotalDays);
                //3 days ago at 7:58 pm - 10.06.2014
            }

            result.AppendFormat("{0} - {1}", dt.ToString("HH:mm"), dt.ToString("dd.MM.yyyy"));
            return result.ToString();
        }

        //http://stackoverflow.com/questions/6025560/how-to-ignore-case-in-string-replace
        /// <summary>
        /// Replaces text in string case insensitive
        /// </summary>
        /// <param name="txt">text or line to search</param>
        /// <param name="ToReplace">value to replace</param>
        /// <param name="ReplaceBy">replacing value</param>
        /// <returns></returns>
        public static string SearchAndReplaceCaseInsensitive(string txt, string ToReplace, string ReplaceBy)
        {
            var result = string.Empty;

            List<String> found = new List<String>();

            foreach (Match match in Regex.Matches(txt, @"(?<!\w)@\w+"))
            {
                found.Add(match.Value);

            }

            if (!String.IsNullOrWhiteSpace(txt))
            {
                var regex = new Regex(ToReplace.TrimEnd(), RegexOptions.IgnoreCase);
                result = regex.Replace(txt, ReplaceBy);

                //result = Regex.Replace(txt, ToReplace.Trim(), ReplaceBy.Trim(), RegexOptions.IgnoreCase);

            }
            return result;
        }


     

        /// <summary>
        /// Replaces the underscore in enum names Ex. Work_In_Progress --> Work In Progress
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnumToString(string text, bool toTitleCase = false)
        {


            System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (toTitleCase)
            {
                return myTI.ToTitleCase(text.Trim().ToLower()).Replace('_', ' ');
            }
            else
            {
                return text.Replace('_', ' ');
            }
        }

        /// <summary>
        /// truncates a long string "This is a long sentence becomes" -> "This is a (...)"
        /// </summary>
        /// <param name="val">string to cut</param>
        /// <param name="length">max no. of characters. 4 chars will be added to max length for ellipsis</param>
        /// <returns></returns>
        public static string TruncateString(string val, int length)
        {
            var result = val;
            if (!string.IsNullOrWhiteSpace(val))
            {
                var strlength = val.Length;

                if (strlength > length)
                {
                    result = string.Concat(val.Substring(0, length), " (...)");
                }

            }
            return result;

        }

        /*
         * Helper method that checks whether a string is made up of digits only
         * if so returns true
         * param: String 
         * 
         */
        public static bool IsNumber(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;


            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return (s.Length > 0);
        }

        /*
         * Helper Method to transform text to title case used for names
         * Param: String
         * 
         */
        public static string ToTitleCase(string text)
        {
            System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

            if (String.IsNullOrEmpty(text))
                return string.Empty;


            return myTI.ToTitleCase(text.Trim().ToLower());
        }


        /*
       * Helper Method to transform double type to money format
       * Param: double
       * 
       */
        public static string ToMoney(double amt)
        {

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            cultureInfo.NumberFormat.CurrencySymbol = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            return string.Format("{0:C2}", amt);
        }

        public static string ToMoneyNoDecimal(double amt)
        {

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

            cultureInfo.NumberFormat.CurrencySymbol = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            return string.Format("{0:n0}", amt);
        }


        /*
       * Helper Method to verify if a string is empty if such, return true
       * Param: string
       * 
       */
        public static bool IsEmpty(string value)
        {
            string val = string.Empty;
            if (value != null)
                val = value.Trim();

            return String.IsNullOrEmpty(val);
        }


        public static bool IsDateValid(DateTime dt)
        {
            if (dt == null)
                return false;

            return (dt > new DateTime(2900, 12, 31) || dt < new DateTime(1900, 1, 1));

        }

        public static String CheckDate(String dateobj)
        {
            try
            {
                //DateTime.ParseExact(dateobj, "dd-MM-yyyy");
                //var dt = Convert.ToDateTime(dateobj);

                var dt = DateTime.ParseExact(dateobj, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return dt.ToString("dd/MM/yyyy");
            }
            catch (Exception e)
            {
                e.ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// converts minutes to the format D.HH:mm e.g 2.10:30
        /// </summary>
        /// <param name="minutes">minutes to be converted as int</param>
        /// <param name="DayHours">How many hours to make a day, optional is 7.5 by default as double</param>
        public static string ConvertMinutesToDaysHourMinutes(int minutes, double DayHours = 24)
        {
            int days = 0;
            String answer = "  -";


            if (minutes < 1)
                return answer;

            if (minutes > DayHours * 60)
            {
                int daymins = (int)DayHours * 60;

                days = minutes / daymins;

                minutes = minutes % daymins;
            }



            TimeSpan t = TimeSpan.FromMinutes(minutes * 1.0);

            answer = string.Format("{0:D2}:{1:D2}",
               t.Hours,
               t.Minutes);

            if (days > 0)
            {
                answer = string.Format("{0:D1}.{1:D2}:{2:D2}",
                    days,
                     t.Hours,
                    t.Minutes);

            }

            return answer;
        }


        /// <summary>
        /// special version for bootstrap-timepicker using format HHmm e.g 0445
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="DayHours"></param>
        /// <returns></returns>
        public static string ConvertMinutesToHHmmForEdit(int minutes, double DayHours = 24)
        {
            int days = 0;
            String answer = "0:00";


            if (minutes < 1)
                return answer;

            if (minutes > DayHours * 60)
            {
                int daymins = (int)DayHours * 60;

                days = minutes / daymins;

                minutes = minutes % daymins;
            }



            TimeSpan t = TimeSpan.FromMinutes(minutes * 1.0);

            answer = string.Format("{0:D1}:{1:D2}",
               t.Hours,
               t.Minutes);

            return answer;
        }




        public static string ToFriendlyDate(DateTime dt, bool includeTime = false)
        {
            if (includeTime)
            {
                return String.Format("{0:dd MMM yyyy @ HH:mm}", dt);
            }
            else
            {

                return String.Format("{0:dd MMM yyyy}", dt);
            }
        }

        public static string ToFriendlyDateIfNotDateMinValue(DateTime dt)
        {
            string result = String.Empty;
            if (dt != DateTime.MinValue)
            {
                result = String.Format("{0:dd MMM yyyy}", dt);
            }
            return result;
        }

        public static string ToFriendlyTimestamp(DateTime dt)
        {
            return String.Format("{0:yyyy-MMM-dd hh:mm tt}", dt);
        }


        /// <summary>
        /// verifies if string is empty, replaces with NA
        /// </summary>
        /// <param name="text">text to verify</param>
        /// <param name="ToTitleCase">if text is to be converted to TitleCase</param>
        /// <returns></returns>
        public static string ReplaceNullWithNA(string text, bool ToTitleCase = false)
        {

            if (string.IsNullOrWhiteSpace(text))
                return "N/A";

            if (ToTitleCase)
            {
                System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                return myTI.ToTitleCase(text.Trim().ToLower());
            }

            return text;
        }



        public static string ReplaceNull(Object value)
        {
            return value == null ? String.Empty : value.ToString();
        }

        public static string NotAvail(string value)
        {
            return string.IsNullOrEmpty(value) ? "Not Available" : value;
        }


        public static string EmptyifNull(object value)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }



        public static string AddTrailingSpaces(int numberOfSpaces = 5)
        {

            // string spaces = new String(' ', numberOfSpaces);
            StringBuilder sp = new StringBuilder();
            for (int i = 1; i <= numberOfSpaces; i++)
            {
                sp.Append("&nbsp;");
            }

            return sp.ToString();


        }


        /*
         * Method returns a duration between a timestamp parameter and now
         * duration if facebook style. ex less than a minute ago / 2 hours ago etc
         * 
         */
        public static string ToDurationString(DateTime dt)
        {
            DateTime? nullable_dt = (DateTime?)dt;
            return ToDurationString(nullable_dt);
        }

        public static string ToDurationString(DateTime? dateArg)
        {
            DateTime inputDate = new DateTime();
            if (dateArg == null)
            {
                return String.Empty;
            }
            else
            {
                inputDate = (DateTime)dateArg;
            }

            TimeSpan span = DateTime.Now - inputDate;
            var mins = Convert.ToInt32(span.TotalMinutes);
            var mins1 = mins + 1;

            if (mins <= 1)
            {
                return "less than a minute ago";
            }
            else if (mins > 1 && mins <= 60)
            {
                return "about " + mins + " minutes ago";
            }
            else if (mins > 60 && mins <= 1440)
            {
                var hr = mins / 60.0;
                var t = Math.Round(hr, 0);
                return "about " + t + " hours ago";

            }
            else if (mins > 1440 && mins < 2880)
            {
                return "yesterday";
            }
            else if (mins > 2880 && mins <= 7200)
            {
                var day = mins / 1440.0;
                var d = Math.Ceiling(day);
                return "" + d + " days ago";

            }
            else
            {
                return inputDate.ToString("dd MMM yyyy");
            }


        }

        /// <summary>
        /// returns the number of busienss days between two dates (excludes saturdays and sundays)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static int NumberOfBusinessDaysBetweenDates(DateTime StartDate, DateTime? EndDate = null)
        {
            EndDate = EndDate == null ? DateTime.Now : EndDate;

            int counter = 0;

            if (EndDate < StartDate)
                return counter;

            var sDate = StartDate;
            while (sDate <= EndDate)
            {
                if (sDate.DayOfWeek != DayOfWeek.Sunday || sDate.DayOfWeek != DayOfWeek.Saturday)
                    counter++;

                sDate = sDate.AddDays(1);
            }
            return counter;

        }


        /// <summary>
        /// Returns the date on which the new financial year starts
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartFinancialYear(DateTime date)
        {
            int year, month, day;

            //get financial year date
            year = (date.Month >= 7) ? date.Year : date.Year - 1;
            month = 7;
            day = 1;
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Returns end of Financial Year
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndFinancialYear(DateTime date)
        {
            int year, month, day;
            //if it is after july
            year = (date.Month > 6) ? date.Year + 1 : date.Year;
            month = 6;
            day = 30;
            return new DateTime(year, month, day);

        }

        /// <summary>
        /// Return the previous year month code 
        /// </summary>
        /// <param name="yearmonthcode"></param>
        /// <returns></returns>
        public static int PreviousYearMth(DateTime date)
        {

            var mm = date.Month == 1 ? 12 : date.Month - 1;
            var yy = date.Month == 1 ? date.Year - 1 : date.Year;
            return (yy * 100) + mm;

        }

        /// <summary>
        /// Return the next year month code 
        /// </summary>
        /// <param name="yearmonthcode"></param>
        /// <returns></returns>
        public static int NextYearMth(int yearmonthcode)
        {
            if (yearmonthcode.ToString().Substring(4) == "12")
            {
                int nextyear = Convert.ToInt32(yearmonthcode.ToString().Substring(0, 4)) + 1;
                int prevmth = 1;
                return Convert.ToInt32(nextyear.ToString() + prevmth.ToString().PadLeft(2, '0'));
            }
            else
            {
                return yearmonthcode + 1;
            }
        }

        /// <summary>
        /// Returns a friendly year month representation MMM YYYY
        /// </summary>
        /// <param name="yearmonthcode"></param>
        /// <returns></returns>
        public static String FriendlyYearMonth(int yearmonthcode)
        {
            string yearMonth = yearmonthcode.ToString();
            if (yearMonth.Length > 5)
            {
                int Year = Convert.ToInt32(yearMonth.Substring(0, 4));
                int Month = Convert.ToInt32(yearMonth.Substring(4));
                return new DateTime(Year, Month, 1).ToString("MMM yyyy");
            }
            else
            {
                return String.Empty;
            }
        }


        /// <summary>
        /// Returns a friendly year month representation MMM yy
        /// </summary>
        /// <param name="yearmonthcode"></param>
        /// <returns></returns>
        public static String FriendlyShortYearMonth(int yearmonthcode)
        {
            string yearMonth = yearmonthcode.ToString();
            if (yearMonth.Length > 5)
            {
                int Year = Convert.ToInt32(yearMonth.Substring(0, 4));
                int Month = Convert.ToInt32(yearMonth.Substring(4));
                return new DateTime(Year, Month, 1).ToString("MMM yy");
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the Current period as yearMonth
        /// </summary>
        /// <returns></returns>
        public static string GetYearMonthForCurrentPeriod()
        {
            return String.Concat(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"));
        }

        /// <summary>
        /// Returns the previous month period as yearMonth
        /// </summary>
        /// <returns></returns>
        public static string GetYearMonthForPreviousMonth(int previous = 1)
        {
            DateTime previousMonth = DateTime.Now.AddMonths(-previous);
            return String.Concat(previousMonth.Year.ToString(), previousMonth.Month.ToString("00"));
        }

        /// <summary>
        /// return financial year based on a date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int FinancialYear(DateTime date)
        {
            return (date.Month > 6) ? date.Year + 1 : date.Year;
        }

        /// <summary>
        /// returns Year month code of a date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int YearMonthCode(DateTime date)
        {
            return Convert.ToInt32(date.Year.ToString() + date.Month.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// nullable parameter
        /// if null return current week else return param date's week number
        /// </summary>
        /// <returns></returns>

        public static int GetWeekNumber(DateTime? dtPassed)
        {
            //http://www.codeproject.com/Tips/315958/ISO-Week-Number-by-Date-Date-by-ISO-Week-Number
            DateTime actualDate = (dtPassed.HasValue && !(dtPassed.Value == DateTime.MinValue || dtPassed.Value == DateTime.MaxValue)) ? dtPassed.Value : DateTime.Now;

            int year = actualDate.Year;
            // Get jan 1st of the year
            DateTime startOfYear = new DateTime(year, 1, 1);
            // Get dec 31st of the year
            DateTime endOfYear = new DateTime(year, 12, 31);
            // ISO 8601 weeks start with Monday 
            // The first week of a year includes the first Thursday 
            // DayOfWeek returns 0 for sunday up to 6 for saterday
            int[] iso8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
            int nds = actualDate.Subtract(startOfYear).Days + iso8601Correction[(int)startOfYear.DayOfWeek];
            int wk = nds / 7;
            switch (wk)
            {
                case 0:
                    // Return weeknumber of dec 31st of the previous year
                    wk = GetWeekNumber(startOfYear.AddDays(-1));
                    break;
                case 53:
                    // If dec 31st falls before thursday it is week 01 of next year
                    if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                    {
                        wk = 1;
                    }
                    break;
                default:
                    break;
            }

            //Calendar calendar = new GregorianCalendar();
            //CultureInfo ciCurr = CultureInfo.CurrentCulture;
            //int weekNum = ciCurr.Calendar.GetWeekOfYear(actualDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return wk;
        }




        /// <summary>
        /// returns the monday date for a given date, 
        /// i.e if tues is 09-may-2010 then return 01-may-2010 (which is mon)
        /// </summary>
        /// <param name="dtPassed"></param>
        /// <returns></returns>
        public static DateTime GetMondayDate(DateTime dtPassed)
        {
            //http://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
            DayOfWeek startOfWeek = DayOfWeek.Monday;
            int diff = dtPassed.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dtPassed.AddDays(-1 * diff).Date;

            /*OLD VERSION CODING:*/
            //monday = 1
            //int dayOfWk = (Int32)dtPassed.DayOfWeek;
            //double diff = (dayOfWk - 1) * -1;
            //return dtPassed.AddDays(diff);

        }



        /// <summary>
        /// return the number of days between two dates
        /// if start date is greater than end date then return value is -ve
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int NumOfDaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            if (startDate == null || endDate == null)
                return 0;

            var numdays = endDate.Date.Subtract(startDate.Date).Duration().Days + 1;
            return numdays;
        }



        /// <summary>
        /// add MCB domain to a login name, e.g giolet becomes MCB\GIOLET
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddMCBDomain(string username)
        {
            var result = string.Empty;

            if (!(string.IsNullOrEmpty(username)))
            {
                result = string.Concat(@"MCB\", username.Replace(@"MCB\", "")).ToUpper().Trim();
            }
            return result;
        }


        /// <summary>
        /// remove MCB domain from login name, e.g MCB\GIOLET becomes GIOLET
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveMCBDomain(string username)
        {
            var result = string.Empty;

            if (!(string.IsNullOrEmpty(username)))
            {
                result = username.Replace(@"MCB\", "").ToUpper().Trim();
            }
            return result;
        }





        public static int ClockToMins(string Clock)
        {
            return (Convert.ToInt32(Clock.Substring(0, 2)) * 60) + Convert.ToInt32(Clock.Substring(3, 2));

        }


        /// <summary>
        /// 
        /// http://stackoverflow.com/questions/15788055/how-to-create-persistent-unique-md5-hash
        /// </summary>
        /// <param name="strword"></param>
        /// <returns></returns>
        private const string _myGUID = "{C05ACA39-C810-4DD1-B138-41603713DD8A}";
        public static string ConvertStringtoMD5(string strword)
        {

            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(_myGUID + strword);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }



        public static string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }

        public static string base64Encode(string data)
        {
            try
            {
                
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }


        public static string GetShortString(string text)
        {

            return GetShortString(text, 40);
        }

        public static string GetShortString(string text, int maxLength)
        {
            string result = text;
            if (!String.IsNullOrWhiteSpace(text) && text.Length > maxLength)
            {
                result = String.Concat(text.Substring(0, maxLength), "...");
            }

            return result;
        }

        /// <summary>
        /// extracts string between parentheses 
        /// ex. Financial BU(01FM002) -> 01FM002
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ExtractFromParenthesis(string text)
        {
            string result = string.Empty;
            if (!String.IsNullOrWhiteSpace(text))
            {
                var openParenthesis = text.IndexOf("(");
                var closeParenthesis = text.IndexOf(")");

                if (openParenthesis == -1 || closeParenthesis == -1)
                {
                    result = text;
                    return result;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    result = text.Substring(openParenthesis + 1, (closeParenthesis - 1) - openParenthesis);
                }
            }
            return result;

        }

        public static string RemoveSpacesAndSpecial(string entry)
        {
            return entry.Replace(" ", "").Replace("/", "").Replace(":", "");
        }
        /// <summary>
        /// method replaces angle brackets < > in text to avoid browser crashing to to dangerous character
        /// </summary>
        /// <param name="text">text to check and replace</param>
        /// <param name="replaceWith">empty, [ or (</param>
        /// <returns></returns>
        public static string ReplaceAngleBrackets(string text, char replaceWith = ' ')
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                if (replaceWith == '[' || replaceWith == ']')
                {
                    result = text.Replace('<', '[').Replace('>', ']');
                }
                else if (replaceWith == '(' || replaceWith == ')')
                {
                    result = text.Replace('<', '(').Replace('>', ')');
                }
                else
                {
                    result = text.Replace('<', ' ').Replace('>', ' ');
                }

            }
            return result;

        }



        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripHtmlTags(string source, char replaceWith = ' ')
        {
            if (source == null)
            {
                Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

                return _htmlRegex.Replace(source, replaceWith.ToString());
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// removes the last character from a String ex. insert yxz into, ==> insert yxz into
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string RemoveLastCharacterFromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return string.Empty;
            }

            var trimmed_param = param.Trim();
            var len = trimmed_param.Length;
            var result = trimmed_param.Remove(len - 1);
            return result;
        }


        /// <summary>
        /// Concatenates an array of string to a single string separated with char
        /// ex. 'THIS','THAT','THESE' --> 'THIS,THAT,THESE'
        /// </summary>
        /// <param name="arrayToConcat"></param>
        /// <param name="concatWith"></param>
        /// <returns></returns>
        public static string ConcatenateStringArray(string concatWith, String[] arrayToConcat)
        {

            var result = String.Empty;
            if (!(arrayToConcat == null || arrayToConcat.Length == 0))
            {
                result = String.Join(concatWith, arrayToConcat);

            }
            return result;
        }

        public static String ObjectToXMLGeneric<T>(T filter)
        {

            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, filter);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    //log.ErrorFormat("ListToXMLGeneric: " + e.ToString());
                    throw e;

                }
            }
            return xml;
        }

        public static Object XMLToObjectGeneric(String Filter)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Object));
                var reader = new StringReader(Filter);
                object obj = deserializer.Deserialize(reader);
                Object XmlData = (Object)obj;
                reader.Close();

                return XmlData;

            }
            catch (Exception )
            {
                throw;
            }
        }

    }
}