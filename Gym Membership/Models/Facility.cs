using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Facility
    {
        public int FacilityId { get; set; }
        public int TransactionId { get; set; }

        public DateTime DueDate { get; set; }

        public double DueAmount { get; set; }

        public bool IsPaid { get; set; }

        public DateTime PaymentUntilDate { get; set; }

        public int InstallmentNum { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as Facility;
            if (t == null)
                return false;
            if (FacilityId == t.FacilityId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + FacilityId.GetHashCode();

            return hash;

        }

    }
}