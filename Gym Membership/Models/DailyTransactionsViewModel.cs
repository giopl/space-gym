using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class DailyTransactionsViewModel
    {
        public DailyTransactionsViewModel()
        {
            Transactions = new List<ReceiptReport>();
        }
        public IList<ReceiptReport> Transactions { get; set; }
        public string DateRange { get; set; }
    }
}