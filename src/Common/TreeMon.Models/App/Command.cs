// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
namespace TreeMon.Models.App
{
    public   class Command
    {
        public int RoleWeight { get; set; }

        public string Name { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
