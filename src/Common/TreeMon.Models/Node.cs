// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models
{
    public class Node :INode
    {
        public Node()
        {
            this.ParentId       = 0;
            if(string.IsNullOrWhiteSpace(this.UUID))
                this.UUID           = Guid.NewGuid().ToString("N");
            this.UUIDType       = string.Empty;
            this.UUParentID     = string.Empty;
            this.UUParentIDType = string.Empty;
            this.Name           = string.Empty;
            this.Status         = string.Empty;
            this.AccountUUID    = string.Empty;
            this.Active         = true;
            this.Deleted        = false;
            this.Private        = true;
            this.SortOrder      = 0;
            this.CreatedBy      = string.Empty;
            this.DateCreated    = DateTime.MinValue;
            this.RoleWeight      = 1;
            this.RoleOperation  = ">=";
            this.Image          = "";
        }

        public string GUUID { get; set; }

        public string GuuidType { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]       // [Column(Order = 1)] //column order for multiple keys
        public int Id { get; set; }        //sqlite only allows integer for auto inc. primary key?! ugh

        // Use ToSafeString(true)?.ToUpper();
        [NotMapped]
        public string SafeName { get; set; }

        public int? ParentId { get; set; }

        [StringLength(64)]
        public string UUID { get; set; }

        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
         [StringLength(32)]
        public string UUIDType { get; set; }

        [StringLength(64)]
        public string UUParentID { get; set; }

        [StringLength(32)]
        public string UUParentIDType { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(32)]
        public string Status { get; set; }

        [StringLength(32)]
        public string AccountUUID { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Active { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Deleted { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Private { get; set; }

        public int SortOrder { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime  DateCreated { get; set; }

        public string Image { get; set; }

        public int RoleWeight { get; set; }

        public string RoleOperation { get; set; }


    }


    public class NodeCompare : IEqualityComparer
    {
         private readonly Func<Node, object> _funcDistinct;
        public NodeCompare( Func<Node, object> funcDistinct)
        {
            this._funcDistinct = funcDistinct;
        }

        public new bool Equals(object x, object y)
        {
            return  _funcDistinct((Node)x).Equals(_funcDistinct((Node)y));
        }

        public int GetHashCode(object obj)
        {
            return this._funcDistinct((Node)obj).GetHashCode();
        }
    }

  
}
