using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class VisitDataViewModel
    {
        
        public String YearMonthCodeCateg { get; set; }

        public String DateRange { get; set; }


        public String VisitsPerYearMonth { get; set; }
        public String FemaleVisitsPerYearMonth { get; set; }
        public String DayOfWeekCateg { get; set; }
        public String DayOfWeekData { get; set; }


        public String DayOfMonthCateg { get; set; }
    public String DayOfMonthData { get; set; }
    public String HourOfDayData { get; set; }

        public IList<Serie> VisitPerAgeGroup { get; set; }
        public IList<Serie> VisitPerMembership { get; set; }

        public VisitDataViewModel()
        {
            VisitPerAgeGroup = new List<Serie>();
            VisitPerMembership = new List<Serie>();
        }


    }
}