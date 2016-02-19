using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ChangeMembershipModel
    {

        public string MembershipCode { get; set; }
        public int MainMemberId { get; set; }

        public Int32[] MemberIds {get;set;}

        public List<Int32> MemberIdsList { get; set; }
        public ChangeMembershipModel()
        {
         //   MemberIds =  Int32[];
            MemberIdsList = new List<int>();
        }
    }
}