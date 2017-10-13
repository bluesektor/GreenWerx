// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Data.Logging.Models
{
    [Table("SystemLog")]
    public class LogEntry
    {

        public LogEntry()
        {
            this.UUIDType = "LogEntry";
        }

        #region Properties
        [Key]
        public int Id { get; set; }

        public string InnerException
        {
            get; set;
        }

        public string Level
        {
            get; set;
        }

        public DateTime LogDate
        {
            get; set;
        }

        public string Source
        {
            get; set;
        }

        [NotMapped]
        public string UUIDType { get; set; }

        public string StackTrace
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        public string User
        {
            get; set;
        }


        #endregion Properties
    }
}
