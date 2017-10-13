// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Inventory
{
    [Table("Inventory")]
    public class InventoryItem: Item, INode
    {

        public InventoryItem():base()
        {
            this.UUIDType = "InventoryItem";
        }

        public string LocationUUID { get; set; }

        /// <summary>
        /// store,vehicle, room etc..
        /// </summary>
        public string LocationType { get; set; }

        public float Quantity { get; set; }

        public string ReferenceType { get; set; }   //  product, item, user, ballast, plant

        public string ReferenceUUID { get; set; } //id of the item in inventory if we have to break it down to individual items.


        //display in web store.
        public bool Published { get; set; }

        public DateTime ItemDate { get; set; }

        public string DateType { get; set; } //expires, end of cycle ....

        public string VendorUUID { get; set; }

        public string Link { get; set; }

        public string LinkProperties { get; set; }

        //TODO inventory log. show when items added, removed, etc. 
        //public DateTime DateAdded { get; set; }
        //public string AddedByUUID { get; set; }
    }
}
