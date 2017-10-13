// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("Roles")]
    public class Role:INode
    {

        public Role()
        {
            this.Id = 0;
            this.ParentId = 0;
            this.UUParentID = string.Empty;
            this.UUParentIDType = string.Empty;
            this.Name = string.Empty;
            this.AccountUUID = string.Empty;
            this.Active = true;
            this.Deleted = false;
            this.Private = true;
            this.SortOrder = 0;
            this.CreatedBy = string.Empty;
            this.DateCreated = DateTime.MinValue;

            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "Role";
            RoleOperation = ">=";
            RoleWeight = 1;

        }

        public string SyncKey { get; set; }

        public string SyncType { get; set; }

        public int RoleWeight { get; set; }

        public string RoleOperation { get; set; }

        public string CreatedBy { get; set; }

        public DateTime DateCreated  { get; set; }

        // [SQLite.PrimaryKey,SQLite.AutoIncrement]]
        [Key]
        public int Id { get; set; }


        public bool Private { get; set; }

        public int SortOrder { get; set; }

        public bool Active { get; set; }
        
        public int? ParentId { get; set; }

        public bool Deleted { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }
        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
        [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string UUParentID { get; set; }
        [StringLength(32)]
        public string UUParentIDType { get; set; }
        public string Name{  get; set;}

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //Set this to ignore EndDate
        public bool Persists { get; set; }

        [StringLength(32)]
        public string AccountUUID { get; set; }

        /// <summary>
        ///  Values: web, forms, or mobile
        /// </summary>
        public string AppType { get; set; }

        /// <summary>
        /// Values: 0-5 where 5 has the highest authority.
        /// </summary>
        public int Weight { get; set; }

    }
}
