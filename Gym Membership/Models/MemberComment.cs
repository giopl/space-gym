using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class MemberComment
    {
        public int CommentId { get; set; }
        public int MemberId { get; set; }

        public string CommentDescription { get; set; }
        public DateTime CommentDate { get; set; }
        public DateTime FollowupDate { get; set; }

        //public CommentTypeEnum CommentType { get; set; }
        public String CommentType { get; set; }

        public AdminUser Inputter { get; set; }

        public String CommentStatus { get; set; }
        //public CommentStatusEnum CommentStatus { get; set; }

        public MemberComment()
        {
            Inputter = new AdminUser();
        }


        public enum CommentStatusEnum
        {
            FOR_ACTION = 0,
            CLOSED,


        }

        public enum CommentTypeEnum { 
            GENERAL = 0,
            SUGGESTION,
            TRAINING,
            COMPLAINT

        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as MemberComment;
            if (t == null)
                return false;
            if (CommentId == t.CommentId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + CommentId.GetHashCode();

            return hash;

        }
    }
}