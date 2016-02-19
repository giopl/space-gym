using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class ListItem
    {
        public int id { get; set; }

        public string name { get; set; }

        public DateTime date { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as ListItem;
            if (t == null)
                return false;
            if (id == t.id)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + id.GetHashCode();

            return hash;

        }

    }
}