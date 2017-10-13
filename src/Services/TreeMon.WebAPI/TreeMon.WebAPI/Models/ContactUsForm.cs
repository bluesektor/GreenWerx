// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Web.Models
{
    public class Message
    {
        public string SendTo { get; set; }

        public string SentFrom { get; set; }

        public string Subject { get; set; }

        public string Comment { get; set; }

        public DateTime DateSent { get; set; }

        public string Type { get; set; }

    }

}