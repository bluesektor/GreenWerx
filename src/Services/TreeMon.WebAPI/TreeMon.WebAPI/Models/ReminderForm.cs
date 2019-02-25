// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Events;

namespace TreeMon.Web.Models
{

    public class ReminderForm :Reminder
    {
        [NotMapped]
        public string Captcha { get; set; }
    }
}