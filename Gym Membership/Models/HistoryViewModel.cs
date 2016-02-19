using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class HistoryViewModel
    {
        public IList<ListItem> Items { get; set; }

        public HistoryViewModel()
        {
            Items = new List<ListItem>();
        }
    }
}