using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Receipt
    {

        public Receipt()
        {
            ReceivedBy = new AdminUser();
        }
        public int ReceiptId { get; set; }

        public int TransactionId { get; set; }

        [DisplayName("Received on")]
        public DateTime ReceivedOn { get; set; }

        [DisplayName("Received by")]
        public AdminUser ReceivedBy { get; set; }

        public int MemberId { get; set; }

        [DisplayName("Payment Method")]
        public String PaymentMethod { get; set; }

        public double AmountReceived { get; set; }
        public bool TransactionCancelled { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as Receipt;
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