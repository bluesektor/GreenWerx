// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Event
{
    [Table("Reminders")]
    public class Reminder : Node, INode
    {
        public Reminder()
        {
            UUIDType = "Reminder";
            ReminderRules = new List<ReminderRule>();
        }

        public string Body { get; set; }

   
        public DateTime? EventDateTime { get; set; }

        //how many times to remind
        public int RepeatCount { get; set; }

        //if this is set ignore RepeatCount
        public bool RepeatForever { get; set; }

        //daily, bi-weekly, monthly..
        public string Frequency { get; set; }

        //forgot what this was for. I think it was how many times the reminder has notified..
        public int ReminderCount { get; set; }

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
