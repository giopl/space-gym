using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class AddMemberViewModel
    {
       // public GymMember[] GymMembers { get; set; }
        public IList<GymMember> GymMembers { get; set; }
        public String MembershipCode { get; set; }

        public bool IsCustom
        {
            get
            {
                return MembershipCode == "CUST" || MembershipCode == "TEMP";
            }
        }

        public String Club { get; set; }


        public int NumMembers { get; set; }

        public string HowYouHeardForm { get; set; }

        public bool test { get; set; }

        public List<string> ValidationErrors { get; set; }

        public AddMemberViewModel() {

            ValidationErrors = new List<string>();
            GymMembers = new List<GymMember>();
            for (var i = 0; i < 5; i++)
            {
                GymMembers.Add(new GymMember());

            }
         //   GymMember[] GymMembers = InitializeArray<GymMember>(4);
        }

        //http://stackoverflow.com/questions/3301678/how-to-declare-an-array-of-objects-in-c-sharp

        T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }
    }
}