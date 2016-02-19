using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Membership
    {
        public int MembershipId { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        [DisplayName("Code")]
        public string MembershipCode { get; set; }


        public bool IsPass { get; set; }
        public bool IsSystem { get; set; }

        [DisplayName("Reporting Categ")]
        public String Category { get; set; }


        //public bool IsPass
        //{
        //    get
        //    {
        //        return MembershipCode == "ONED" || MembershipCode == "TEND";

        //    }
        //}

        public bool IsCustom
        {
            get
            {
                return MembershipCode == "CUST" || MembershipCode == "TEMP";
            }
        }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsYearly
        {
            get
            {

                return MonthTerms == 12;
            }
        }

        [DisplayName("Reg. Fee")]
        public double RegistrationFee { get; set; }

        public double RegistrationFeePerPerson
        {

            get
            {
                return RegistrationFee / NumberMembers;
            }
        }
        public double Fee { get; set; }

        [DisplayName("Months")]
        public int MonthTerms { get; set; }


        public string PaymentTerms
        {
            get
            {

                if (MonthTerms == 1)
                {
                    return "Monthly";
                }
                else if (MonthTerms == 12)
                {
                    return "Yearly";
                }
                else
                {
                    return "N/A";
                }
            }
        }

        [DisplayName("Members allowed")]
        public int NumberMembers { get; set; }

        public bool IsSingleMembership
        {
            get
            {
                return NumberMembers == 1;
            }
        }

        [DisplayName("Rules")]
        public string MembershipRules { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string MonthlyDue
        {
            get
            {
                return string.Format("{0:n2}", Fee / (MonthTerms * 1.0));
            }
        }


        public double MonthlyDueAmount
        {
            get
            {
                return Fee / (MonthTerms * 1.0);
            }
        }

        public double MonthlyDueAmountPerPerson
        {
            get
            {
                return Fee / (MonthTerms * 1.0) / NumberMembers;
            }
        }

        public IList<String> ValidationErrors { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as Membership;
            if (t == null)
                return false;
            if (MembershipId == t.MembershipId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + MembershipId.GetHashCode();

            return hash;

        }

        public bool HasValidationErrors
        {
            get
            {
                return ValidationErrors.Count > 0;
            }
        }

        public Membership()
        {
            ValidationErrors = new List<String>();
        }
    }
}