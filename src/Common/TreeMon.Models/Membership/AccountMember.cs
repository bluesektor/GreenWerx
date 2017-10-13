// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("UsersInAccount")]
    public class AccountMember
    {
        public AccountMember()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "AccountMember";
        }
        public int RoleWeight { get; set; }

        // [SQLite.PrimaryKey,SQLite.AutoIncrement]]
        [Key]
        public int Id { get; set; }

        public int ParentId { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }

        /// <summary>
        /// Defines the type of UUID used (guid, hash.<algo> )..
        /// </summary>
        [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(32)]
        public string UUParentID { get; set; }
        [StringLength(32)]
        public string UUParentIDType { get; set; }

        [StringLength(32)]
        public string AccountUUID { get; set; }

        [StringLength(32)]
        public string MemberUUID { get; set; }

        [StringLength(32)]
        public string MemberType { get; set; }

        public string Status { get; set; }

        public int SortOrder { get; set; }
    }
}
