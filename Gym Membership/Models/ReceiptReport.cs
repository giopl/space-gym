using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ReceiptReport
    {
        public int ReceiptId { get; set; }
        public int TransactionId { get; set; }

        public String DateRange { get; set; }

        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Membership { get; set; }
        public string PaymentMethod { get; set; }
        public double ReceiptAmount { get; set; }
        public string ReceivedByUser { get; set; }
        public DateTime ReceiptDate { get; set; }
        public bool IsCancelled { get; set; }

        public double PaidAmount { get; set; }
        public double DiscountedAmount { get; set; }
        public double RegistrationAmount { get; set; }
        public double WrittenOffAmount { get; set; }
        public double OustandingAmount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as ReceiptReport;
            if (t == null)
                return false;
            if (ReceiptId == t.ReceiptId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + ReceiptId.GetHashCode();

            return hash;

        }

    }
}