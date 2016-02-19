using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class AccessLog
    {

        public AccessLog()
        { }

        public AccessLog(string operation, string details, string itemType)
        {
            Operation = operation;
            Details = details;
            Username = UserSession.Current.Username;
            Type = itemType;
        }

        public AccessLog(string operation, string details, string itemType, int itemId)
        {
            Operation = operation;
            Details = details;
            Username = UserSession.Current.Username;
            Type = itemType;
            ItemId = itemId;
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime AccessDateTime { get; set; }
        public string Operation { get; set; }
        public string Details { get; set; }
        public string Type { get; set; }
        public int? ItemId { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as AccessLog;
            if (t == null)
                return false;
            if (Username == t.Username && AccessDateTime == t.AccessDateTime)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 7) + Username.GetHashCode();
            hash += (hash * 7) + AccessDateTime.GetHashCode();

            return hash;

        }  
    
    }
}