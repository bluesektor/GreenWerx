// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;

namespace TreeMon.WebAPI.Models
{
    public class ApiCommand
    {
        public ApiCommand()
        {
            this.Arguments = new List<KeyValuePair<string, string>>();
        }

        public string Command{ get; set; }

        public List<KeyValuePair<string, string>> Arguments;
    }
}