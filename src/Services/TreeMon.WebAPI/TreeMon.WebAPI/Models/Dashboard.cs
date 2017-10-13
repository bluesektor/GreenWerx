// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Web.Models
{
    public class Dashboard
    {
        public Dashboard()
        {
            Content = new List<KeyValuePair<string, string>>();
            SideMenuItems = new List<WebAPI.Models.MenuItem>();
            TopMenuItems = new List<WebAPI.Models.MenuItem>();
        }

        public string View { get; set; }

        public string Domain { get; set; }

        public string Title { get; set; }

        public double SessionLength { get; set; }

        public string Authorization { get; set; }

        public string DefaultLocationUUID { get; set; }

        [StringLength(32)]
        public string UserUUID { get; set; }

        [StringLength(32)]
        public string AccountUUID { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsAdmin { get; set; }


        [NotMapped]
        public List<WebAPI.Models.MenuItem> SideMenuItems { get; set; }

        [NotMapped]
        public List<WebAPI.Models.MenuItem> TopMenuItems { get; set; }


        [NotMapped]
        public List<KeyValuePair<string,string>>  Content { get; set; }
    }
}