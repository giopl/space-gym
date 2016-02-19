using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class RelatedMembers
    {
        public int RelationshipId { get; set; }

        public IList<GymMember> Members { get; set; }

        public RelatedMembers()
        {
            Members = new List<GymMember>();
        }

    }
}