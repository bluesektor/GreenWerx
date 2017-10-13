// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Logging
{
    [Table("EmailLog")]
    public class EmailLog :Node, INode
    {
        public EmailLog()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "EmailLog";
        }

        [StringLength(32)]
        public string UserUUID { get; set; }

        public string EmailTo { get; set; }

        public string EmailFrom { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        //This is the date when the email successfully sent to recipient.
        //Not the date it was submitted.
        //
        public DateTime?  DateSent{ get; set; }

        public string IpAddress { get; set; }
    }
}
