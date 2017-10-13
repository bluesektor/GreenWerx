// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.App
{
    [Table("AppInfo")]
    public class AppInfo :Node, INode
    {
        public AppInfo()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "AppInfo";
            Settings = new List<Setting>();
        }

        public string AppKey { get; set; }

        public string ActiveDbProvider { get; set; }

        public string ActiveDbConnectionKey { get; set; }

        public string DatabaseServer { get; set; }
        public string ActiveDatabase { get; set; }
        public string ActiveDbUser { get; set; }
        public string ActiveDbPassword { get; set; }

        //web,forms, mobile
        public string   AppType { get; set; }

        public string   AccountName            { get; set; }
        public string   AccountEmail           { get; set; }
        public bool     AccountIsPrivate         { get; set; }
        public string   UserName               { get; set; }
        public string   UserEmail              { get; set; }
        public bool     UserIsPrivate            { get; set; }
        public string   UserPassword           { get; set; }
        public string   ConfirmPassword    { get; set; }
        public string   SecurityQuestion   { get; set; }
        public string   UserSecurityAnswer { get; set; }

        public string   PasswordSalt { get; set; }
        public bool     RunInstaller { get; set; }

        public string   SiteDomain { get; set; }

        [NotMapped]
        public string Command { get; set; }

        [NotMapped]
        public List<Setting> Settings { get; set; }
    }
}
