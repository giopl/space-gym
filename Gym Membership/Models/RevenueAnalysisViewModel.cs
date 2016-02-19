using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class RevenueAnalysisViewModel
    {

        public RevenueAnalysisViewModel()
        {
            PaymentMethod = new List<Serie>();
            RevenueTable = new List<Stat>();
            ReceptionTable = new List<Stat>();
        }

        public String DateRange { get; set; }

        public string RevenuePerMonthCategory { get; set; }
        public string RevenuePerMonthActual { get; set; }
        public string RevenuePerMonthDiscounted { get; set; }

        public string RevenueMembershipCategory { get; set; }
        public string RevenueMembershipActual { get; set; }
        public string RevenueMembershipBudgeted { get; set; }

        public IList<Stat> RevenueTable { get; set; }
        public IList<Stat> ReceptionTable { get; set; }

        public IList<Serie> PaymentMethod { get; set; }
        public IList<Serie> RevenueAgeGroup { get; set; }
        public IList<Serie> RevenueMembership { get; set; }
    }
}