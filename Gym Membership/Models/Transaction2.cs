using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Transaction2
    {

        public Transaction2()
        {
            Member = new GymMember();
        }
        public int TransactionId { get; set; }
        public GymMember Member { get; set; }
        public string MembershipCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public double AmountPaid { get; set; }
        public double AmountDiscounted { get; set; }
        public double AmountRegistration { get; set; }
        public double AmountUnpaid { get; set; }
        public int LastTransactionid { get; set; }
        public int NumFacilitiesOrig { get; set; }
        public int NumFacilitiesLeft { get; set; }
        public string Comment { get; set; }
        public double AmountWrittenoff { get; set; } 


        /* member derived details */

        public DateTime PaymentUntilDate
        {
            get
            {
                return Member.PaymentUntilDate;
            }
        }

        public DateTime PaymentNextStartingDate
        {
            get
            {
                return Member.PaymentUntilDate.AddDays(1);
            }
        }

        public double OriginalRegistrationDue
        {
            get
            {
                return Member.Membership.RegistrationFee;
            }
        }

        public double OriginalFeeDue
        {
            get
            {
                return Member.Membership.Fee;
            }
        }

        public bool IsYearly
        {
            get
            {
                return Member.Membership.MonthTerms == 12;
            }
        }



        public double OriginalFeeDuePerMonthPerPerson
        {
            get
            {
                return Member.Membership.Fee / (Member.Membership.MonthTerms * Member.Membership.NumberMembers);
            }
        }



        /* calculated fields */
        public double FeesDue
        {
            get
            {

              
                //
                //end of month of start period - start period
                TimeSpan ts = Utils.GetLastDayOfMonth(PaymentNextStartingDate) -PaymentNextStartingDate;
                        var days = ts.TotalDays;

                        var day = PaymentNextStartingDate.Day;
                        var rate = 1.0;
                        if (day <= 10)
                        {

                            rate = ConfigurationHelper.ProrataFirst();
                        }
                        else if (day > 10 && day <= 20)
                        {
                            rate = ConfigurationHelper.ProrataSecond();

                        }
                        else
                        {
                            rate = ConfigurationHelper.ProrataThird();

                        }

                        // var prorata = days * (MonthlyFee / DateTime.DaysInMonth(lastPaymentDate.Year, lastPaymentDate.Month));
                        var prorata = (OriginalFeeDuePerMonthPerPerson * rate);



                        TimeSpan tsm = Utils.GetLastDayOfMonth(DateTime.Now) - Utils.GetLastDayOfMonth(PaymentNextStartingDate);
                        int NumMonths = (((DateTime.Now.Year - PaymentNextStartingDate.Year) * 12) + DateTime.Now.Month - PaymentNextStartingDate.Month) + 1;


                        var fees = (NumMonths - 1) * OriginalFeeDuePerMonthPerPerson;

                        return prorata + fees;


            }
        }

    }
}