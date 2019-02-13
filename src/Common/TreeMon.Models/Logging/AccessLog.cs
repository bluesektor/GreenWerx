// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Logging
{
    [Table("AccessLog")]
    public class AccessLog : Node, INode
    {
        public AccessLog()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "AccessLog";
        }

        public DateTime? AuthenticationDate { get; set; }

        public string IPAddress { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Passed or failed the authentication scheme
        /// </summary>
        public bool Authenticated { get; set; }

        public string FailType { get; set; }

        /// <summary>
        /// Where is the attempt coming from..
        /// </summary>
        public string Vector { get; set; }


    }
}
