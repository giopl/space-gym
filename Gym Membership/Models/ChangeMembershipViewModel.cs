using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ChangeMembershipViewModel
    {
        public GymMember Member { get; set; }

        public int MainMemberId { get; set; }

        public Int32[] MemberIds { get; set; }
        public List<Int32> MemberIdsList { get; set; }
        
        public string CurrentMembershipCode { get; set; }
        public string NewMembershipCode { get; set; }

        public bool HasRelations
        {
            get {

            return RelatedMembers.Count > 0;
            }
        }
        public IList<GymMember> RelatedMembers { get; set; }
        public IList<GymMember> SingleMembers { get; set; }

        public IList<Membership> Memberships { get; set; }

        public IList<Membership> NonSystemMemberships {
            get
            {
                return Memberships.Where(x => x.IsSystem == false).ToList();
            }
         }



        public IList<Membership> SingleMemberships {
            get {
            
            return NonSystemMemberships.Where(x => x.IsSingleMembership).ToList();

            }
        }

        public IList<Membership> MembershipsToDisplay { 
            get {

                return HasRelations ? SingleMemberships : NonSystemMemberships;
            }
        
        }
        public List<string> ValidationErrors { get; set; }

        public bool HasValidationErrors
        {
            get
            {
                return ValidationErrors.Count > 0;
            }
        }

        public ChangeMembershipViewModel()
        {
            Member = new GymMember();
            RelatedMembers = new List<GymMember>();
            SingleMembers = new List<GymMember>();
            Memberships = new List<Membership>();
            ValidationErrors = new List<String>();
            MemberIdsList = new List<Int32>();

           // MemberIds = new Int32[5];
            //  MemberIds = new List<Int32>();
        }
    }
}