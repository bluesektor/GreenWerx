// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.App
{
    [Table("UserSessions")]
    public  class UserSession
    {
        public UserSession()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "UserSession";
        }
        public int RoleWeight { get; set; }

        // [SQLite.PrimaryKey,SQLite.AutoIncrement]]
        [Key]
        public int Id { get; set; }


        public int ParentId { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }
        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
        [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string UserUUID { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime Issued { get; set; }
        public System.DateTime Expires { get;set; }

        public bool IsPersistent { get; set; }
        public string UserData { get; set; }
        [StringLength(32)]
        public string UserName { get; set; }

        public string UserType { get; set; }
        public string Message { get; set; }

        public string Captcha { get; set; }

        /// <summary>
        /// In minutes
        /// </summary>
        [NotMapped]
        public int SessionLength { get; set; }
    }
}
