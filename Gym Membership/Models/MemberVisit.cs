using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class MemberVisit
    {
        public int VisitId { get; set; }

        public int MemberId { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }


        /// <summary>
        /// returns whether member is currently in the club
        /// </summary>
        public bool IsPresent {
            get
            { 
                TimeSpan ts = DateTime.Now - CheckInTime;
                var mins = ts.TotalMinutes;
                return CheckOutTime == null && mins <= 60.0;

            }
        
        }

        public bool IsPass { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as MemberVisit;
            if (t == null)
                return false;
            if (VisitId == t.VisitId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + VisitId.GetHashCode();

            return hash;

        }
    }
}