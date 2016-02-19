using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class MemberProfilesViewModel
    {

        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }


        public IList<Serie> AgeGroup { get; set; }

        public MemberProfilesViewModel()
        {

            AgeGroup = new List<Serie>();
        }

        public string MembershipCategories { get; set; }
        public string CountMembershipFemale { get; set; }
        public string CountMembershipMale { get; set; }

        public string CountAgeGroupFemale { get; set; }
        public string CountAgeGroupMale { get; set; }


    }

  
}