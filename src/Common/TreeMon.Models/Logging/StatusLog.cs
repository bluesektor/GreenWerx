// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Models.Logging
{
    /// <summary>
    /// This keeps status of any long running process.
    /// Delete the entry when the process is complete. 
    /// This should not be a log that grows.
    /// </summary>
    public class StatusLog: Node, INode
    {
        public StatusLog()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "StatusLog";
        }

        /// <summary>
        /// What functionality is being tracked
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// What we  are processing (email list, install etc.)
        /// </summary>
        public string ServiceObject { get; set; }

        /// <summary>
        /// Authorization token from the client.
        /// Authorization : AuthToken
        /// </summary>
        public string AuthToken { get; set; }


        /// <summary>
        /// How many are in the set.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// How many have been processed to completion.
        /// </summary>
        public int TotalProcessed { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}
