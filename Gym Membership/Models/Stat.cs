using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Stat
    {

        public int Count { get; set; }
        public string Gender { get; set; }
        public string Membership { get; set; }
        public string MembershipName { get; set; }
        public string AgeGroup { get; set; }

        public DateTime VisitDate { get; set; }
        
        public int VisitHour { get; set; }

        /* section for revenue analysis */
        public int YearMonthPayment { get; set; }
        public int ReceiptYearMonth { get; set; }

        public double Bill { get; set; }
        public double Paid { get; set; }
        public double WrittenOff { get; set; }
        public double Discounted { get; set; }
        public double InstallmentDue { get; set; }
        public double RegistrationAmount { get; set; }

        public double DuePerPerson { get; set; }
        public double PotentialRevenue { get; set; }
        public int TotalMembers { get; set; }
        public int MembersPaid { get; set; }
        public string PaymentMethod { get; set; }


        public double NetReturn
        {

            get
            {
                if (PotentialRevenue == 0)
                {
                    return PotentialRevenue;
                }
                return (Paid / PotentialRevenue) * 100.0;
            }
        }
        
        public string NetReturnFlag
        {
            get
            {
               if(NetReturn < 30.0)
                {
                    return "danger";
                } else if (NetReturn >=30.0 & NetReturn <70.0 )
                {
                    return "warning";
                } else
                {
                    return "success";
                }
            }
        }

        public String MembershipCategory {get; set;}

        public double WrittenOffAndDiscounted
        {
            get
            {
                return WrittenOff + Discounted;
            }
        }


        public double FeeAndRegistration
        {
            get
            {
                return Paid + RegistrationAmount;
            }
        }

        public string YearMonthPaymentDesc
        {
            get
            {
                if (YearMonthPayment > 200000)
                {
                    var mth = Int32.Parse(YearMonthPayment.ToString().Substring(4, 2)) - 1;

                    var year = YearMonthPayment.ToString().Substring(0, 4);
                    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                    return string.Format("{0} {1}", months[mth], year);
                }
                else
                {
                    return string.Empty;

                }
            }
        }

/* read only section  */
        public Int32 YearMonthInt
        {
            get
            {
                Int32 num = 0;
                 Int32.TryParse(VisitDate.ToString("yyyyMM"), out num);
                return num;
            }
        }

        public string DayOfWeek
        {
            get
            {
                return VisitDate.DayOfWeek.ToString() ;
               // return VisitDate.DayOfWeek;
            }
        }

        public int DayDate
        {
            get
            {
                Int32 num = 0;
                var dd =VisitDate.ToString("dd");
                Int32.TryParse(dd, out num);

                return num; 


            }

        }

        public int DayOfWeekInt
        {
            get
            {
                return (int)VisitDate.DayOfWeek;
            }
        }

        public string YearMonth
        {
            get
            {
                return VisitDate.ToString("MMM yyyy");
            }
        }


        public String AgeGroupDesc
        {
            get
            {
                var m = string.Empty;
                switch (AgeGroup)
                {

                    case "A": m = "below 12"; break;
                    case "B": m = "12-17"; break;
                    case "C": m = "18-25"; break;
                    case "D": m = "26-35"; break;
                    case "E": m = "36-45"; break;
                    case "F": m = "46-55"; break;
                    case "G": m = "56-65"; break;
                    case "H": m = "above 65"; break;

                }
                return m;
            }
        }



        
        public String MembershipDesc {
            get
            {
                var m = string.Empty;
                switch (Membership)
                {

                    case "10VS": m = "Ten Visit Session"; break;
                    case "1DAY": m = "One Day Pass"; break;
                    case "CORP": m = "Corporate - Monthly"; break;
                    case "COUP": m = "Couple - Special Monthly Package"; break;
                    case "CRMB": m = "Members of Cercle de Rose Hill"; break;
                    case "CUST": m = "Custom - monthly"; break;
                    case "FAM3": m = "Family 3 - Special Monthly Package"; break;
                    case "FAM4": m = "Family 4 - Special Monthly Package"; break;
                    case "FAM5": m = "Family 5 - Special Monthly Package"; break;
                    case "FREE": m = "Free Access"; break;
                    case "SGLS": m = "Single - Special Monthly for Single Member "; break;
                    case "STDM": m = "Standard - Monthly"; break;
                    case "STDY": m = "Standard - Yearly"; break;
                    case "STUM": m = "Student - Monthly"; break;
                    case "STUY": m = "Student - Yearly"; break;
                    case "TEMP": m = "Temporary"; break;

                    default:
                        break;
                }
                        return m;


            }
        }
    }
}