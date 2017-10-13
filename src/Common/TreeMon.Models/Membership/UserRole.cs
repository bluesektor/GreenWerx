// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("UsersInRoles")]
    public class UserRole : Node, INode
    {

        public UserRole()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "UserRole";

        }
   
        [StringLength(32)]
        public string UserUUID { get; set; }

        [StringLength(32)]
        public string RoleUUID { get; set; }

        /// <summary>
        /// class for the action
        /// </summary>
        public string Type { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// web, forms, mobile
        /// </summary>
        public string AppType { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //Set this to ignore EndDate
        public bool Persists { get; set; }
    }
}
