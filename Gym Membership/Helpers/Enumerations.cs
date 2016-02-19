using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Helpers
{
    public static class Enumerations
    {


        public enum ExcelExport {
            ALL,
          OVERDUE,
          LONG_OVERDUE,
          INACTIVE,
          BIRTHDAYS,
          ONE_DAY,
          TEN_VISIT

        };
    }
}