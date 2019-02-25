// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Models.Membership;

namespace TreeMon.Models.Events
{
    public class EventsDashboard
    {
        public List<dynamic> Events { get; set; }

        public List<EventGroup> Groups{ get; set; }

        public List<EventLocation> Locations { get; set; }

        public List<EventMember> Members { get; set; }

        public List<EventItem> Inventory { get; set; }

      
    }
}
