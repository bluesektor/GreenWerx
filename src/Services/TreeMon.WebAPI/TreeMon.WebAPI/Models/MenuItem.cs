// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;

namespace TreeMon.WebAPI.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            this.items = new List<MenuItem>();
        }

        public string label { get; set; }

        public string icon { get; set; }

        public string type { get; set; }

        public string href { get; set; }

        public int SortOrder { get; set; }

        public List<MenuItem> items { get; set; }
    }
}