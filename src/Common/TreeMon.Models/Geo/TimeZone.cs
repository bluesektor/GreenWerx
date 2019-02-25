// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Models.Geo
{
    public class TimeZone
    {
        //public TimeZone()
        //{
        //    Id = Guid.NewGuid().ToString("N");
        //}
        //public int RoleWeight { get; set; }

        //public string Id { get; set; }
        //public int OffsetHrs
        //{
        //    get; set;
        //}

        //public int OffsetMin
        //{
        //    get; set;
        //}

        public string value { get; set; }

        public string abbr { get; set; }

        public float offset { get; set; }

        public bool isdst { get; set; }

        public string text { get; set; }

        public string[] utc { get; set; }
    }
}
