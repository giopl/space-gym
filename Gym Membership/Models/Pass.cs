using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Pass
    {
        public int PassId { get; set; }
        public string Title { get; set; }
        public string Fullname { get; set; }

        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public DateTime LastVisit { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        public int Age { get; set; }
        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [DisplayName("Contact No.")]
        public string Phone { get; set; }

        public string Club { get; set; }

        public int VisitsAllowed { get; set; }
        public int VisitsLeft { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } 

        public string PaymentMethod { get; set; }

        public string Comments { get; set; }

        public bool IsOneDayPass { get
            {
                return VisitsAllowed == 1;

            } }


        public IList<MemberVisit> Visits { get; set; }

        public IList<Receipt> Receipts { get; set; }


        public Pass() {
            
            Visits = new List<MemberVisit>();
            Receipts = new List<Receipt>();

            }

    }
}