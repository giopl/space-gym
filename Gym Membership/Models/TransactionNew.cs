using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class TransactionNew
    {
        
        public TransactionNew(DateTime LastPaymentDate, double membershipfeepermonthperperson)
        {            

            var start = Utils.YearMonthCode(LastPaymentDate);
            var end = Utils.YearMonthCode(DateTime.Now);

            this.CalulatedOpeningAmount = (membershipfeepermonthperperson) * (end-start);
            this.PaymentUntilDate = LastPaymentDate;
            if(BalanceAmount == 0)
            {
                NextPaymentDate = Utils.GetLastDayOfMonth(DateTime.Now);
            } else
            {
                var x = (int)(BalanceAmount / membershipfeepermonthperperson);

                var nextdate = DateTime.Now.AddMonths(x);
                NextPaymentDate = Utils.GetLastDayOfMonth(nextdate); 
            }


            FeePerMonthPerPerson = membershipfeepermonthperperson;
        }
        public double FeePerMonthPerPerson { get; set; }

        public DateTime PaymentUntilDate { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public double CalulatedOpeningAmount { get; set; }
        public double WriteOffAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double PaidAmount { get; set; }
        public double ClosingAmount { get; set; }
        public double BalanceAmount
        {
            get
            {
                return CalulatedOpeningAmount - WriteOffAmount - DiscountAmount - PaidAmount;
            }
                
        }
        public double AdvanceAmount {
            get
            {
                if (BalanceAmount < 0)
                    return BalanceAmount * -1;
                else return 0;

            }
                

                }







    }
}