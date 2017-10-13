// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace TreeMon.Models.Membership
{
    [Table("Permissions")]
    public class Permission : Node, INode
    {
        public Permission()
        {
            UUID = Guid.NewGuid().ToString("N");
            UUIDType = "Permission";
        }

        public string Type { get; set; }

        public string Action { get; set; }

        public string Request { get; set; }
        /// <summary>
        /// Type + Action .ToLower()
        ///SQL UPDATE Permissions SET Key = LOWER( CONCAT(Type, Action));
        ///SQLITE UPDATE Permissions SET Key = LOWER( Type || Action );
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// web, forms, mobile
        /// </summary>
        public string AppType { get; set; }

        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }

        //Set this to ignore EndDate
        public bool Persists { get; set; }

        /// <summary>
        /// Scale of how powerful the permission is.
        /// Default scale is 1-10, 10 being most powerful.
        /// </summary>
        public int Weight { get; set; }

    }
}
