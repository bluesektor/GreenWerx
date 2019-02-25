// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Geo;
using TreeMon.Models.Helpers;
using TreeMon.Models.Membership;

namespace TreeMon.Models.Events
{

    [Table("Events")]
    public class Event : Node, INode
    {
        public Event()
        {
            UUIDType = "Event";
        }

        public string Body { get; set; }

        public string Category { get; set; }

        public DateTime? EventDateTime { get; set; }

        //how many times to remind
        public int RepeatCount { get; set; }

        //if this is set ignore RepeatCount
        public bool RepeatForever { get; set; }

        //daily, bi-weekly, monthly..
        public string Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// uuid of the account that is hosting the event.
        /// </summary>
        public string HostAccountUUID { get; set; }
        /// <summary>
        /// For reference when editing the event.
        /// Technically an event can have more than one location,
        /// for simplicity the ui is restricted to one.
        /// </summary>
        [NotMapped]
        public string EventLocationUUID { get; set; }

    }

    /// <summary>
    /// time slots
    /// </summary>
    [Table("EventGroups")]
    public class EventGroup : Node, INode
    {
        public EventGroup()
        {
            UUIDType = "EventGroup";
        }

        public string Body { get; set; }

        public string Category { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string EventUUID { get; set; }

        public string SessionUUID { get; set; }
    }

    [Table("EventMembers")]
    public class EventMember : Node, INode
    {
        public EventMember()
        {
            UUIDType = "EventMember";
        }

        public string EventUUID { get; set; }

        public string UserUUID { get; set; }

        //List<Role> roles
    }

    [Table("EventInventory")]
    public class EventItem : Node, INode
    {
        public EventItem()
        {
            UUIDType = "EventItem";
        }

        public string EventUUID { get; set; }

        public float Count { get; set; }


        // need, want, have  Count = how many are 'there'for the status. i.e. status = 'need' count = 10. 
        //clone item, set status to need and count to how many are needed. set the clones parent id to the clone from uuid
    }

    [Table("EventLocations")]
    public class EventLocation : Node, INode
    {
        public EventLocation()
        {
            UUIDType = "EventLocation";
        }

        public string EventUUID { get; set; }

        public string Email { get; set; }

        //this replaces the Id field on the insert. the ParentId will reference this.
        public int RootId { get; set; }

        public string Abbr { get; set; }

        public string Code { get; set; }

        public int CurrencyUUID { get; set; }

        public string LocationType { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int TimeZoneUUID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Postal { get; set; }

        public string Category { get; set; }

        public int Type { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsBillingAddress { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Virtual { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool isDefault { get; set; }

        public bool OnlineStore { get; set; }

        public float IpNumStart { get; set; }

        public float IpNumEnd { get; set; }


        public float IpVersion { get; set; }
    }



    [Table("Reminders")]
    public class Reminder : Node, INode //Event
    {
        public Reminder()
        {
            UUIDType = "Reminder";
            ReminderRules = new List<ReminderRule>();
            Event = new Event();
            Account = new Account();
        }

        //forgot what this was for. I think it was how many times the reminder has notified..
        public int ReminderCount { get; set; }

        public string EventUUID { get; set; }

        public bool Favorite { get; set; }

        public string Body { get; set; }

      //  public string Category { get; set; }

        public DateTime? EventDateTime { get; set; }

        //how many times to remind
        public int RepeatCount { get; set; }

        //if this is set ignore RepeatCount
        public bool RepeatForever { get; set; }

        //daily, bi-weekly, monthly..
        public string Frequency { get; set; }

     //   public DateTime StartDate { get; set; }

     //   public DateTime EndDate { get; set; }

            [NotMapped]
        public Event Event { get; set; }

        [NotMapped]
        public Account Account { get; set; }

        [NotMapped]
        public List<ReminderRule> ReminderRules { get; set; }
    }

    [Table("ReminderRules")]
    public class ReminderRule
    {
        public ReminderRule()
        {
            this.UUIDType = "ReminderRule";
        }

        [Key]
        [StringLength(32)]
        public string UUID { get; set; }

        [NotMapped]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string ReminderUUID { get; set; }
      

        /// SkipRange
        public string RuleType {get; set;}

        /// Date, time
        public string RangeType { get; set; }

        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }

        public DateTime? DateCreated { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }
    }

}
