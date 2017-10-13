using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Membership
{
    [Table("UserPermissions")]
    public class UserPermission:INode
    {
        public UserPermission()
        {
            UUIDType = "UserPermission";
            
        }
        
        [Key]
        public int Id { get; set; }


        public int ParentId { get; set; }

        public string Name { get; set; }

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
        [StringLength(32)]
        public string UserUUID { get; set; }
        [StringLength(32)]
        public string PermissionUUID { get; set; }
        [StringLength(32)]
        public string RoleUUID { get; set; }

        public bool Active { get; set; }

        [StringLength(32)]
        public string AccountUUID { get; set; }


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
