// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Models
{
    public interface INode
    {
        int RoleWeight { get; set; }

        string RoleOperation { get; set; }

        int Id { get; set; }

        int? ParentId { get; set; }
        
        string UUID { get; set; }
        /// <summary>
        /// Defines the type of SettingUUID used (guid, hash.<algo> )..
        /// </summary>
        string UUIDType { get; set; }

        string UUParentID { get; set; }
        string UUParentIDType { get; set; }

        string AccountUUID { get; set; }

        string Status { get; set; }

        int SortOrder { get; set;  }

        string GUUID { get; set; }

        string GuuidType{ get; set; }

        string Name { get; set; }

        bool Deleted { get; set; }

        bool Private { get; set; }

        string CreatedBy { get; set; }

        DateTime DateCreated { get; set; }

        bool Active        { get; set; }

        string Image         { get; set; }

    }
}
