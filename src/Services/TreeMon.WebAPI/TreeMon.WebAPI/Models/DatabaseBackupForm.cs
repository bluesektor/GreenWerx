// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;

namespace TreeMon.Web.Models
{
    public class ToolsDashboard
    {
        public List<string> Backups { get; set; }

        public string DefaultDatabase { get; set; }

        public List<string > ImportFiles { get; set; }
    }
}