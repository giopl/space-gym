using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Payment
    {
        public Payment()
        {
        }

        private string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                   "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
                                   };


        public int[] DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public int PaymentId { get; set; }
        public int TransactionId { get; set; }


        public int RemainingDays { get; set; }

        public int YearMonth { get; set; }

        public String YearMonthDesc
        {
            get
            {
                var Month = Convert.ToInt32(YearMonth.ToString().Substring(4, 2));
                var Year= Convert.ToInt32(YearMonth.ToString().Substring(0, 4));
                return String.Format("{0} {1}", Months[Month - 1], Year);
                
            }
        }

        public double CalculatedDiscountAndWriteOff {
            get {

                return DiscountedAmount + WriteOffAmount;
             }
        }
        public bool IsProrata { get; set; }

        public double FeeAmount { get; set; }
        public double PaidAmount { get; set; }

        public double DiscountedAmount { get; set; }

        public double WriteOffAmount { get; set; }


        public double CalculatedWrittenOffAmount { 
            get 
            {
                if (IsWrittenOff) {
                    return FeeAmount;
                }
                return 0;
            }
        
        }
        public double RemainingBalanceAmount { get; set; }

        public bool IsWrittenOff { get; set; }
        //public bool IsPostPoned { get; set; }
        public bool IsPostPoned { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as Payment;
            if (t == null)
                return false;
            if (PaymentId == t.PaymentId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + PaymentId.GetHashCode();

            return hash;

        }
    
    }
}