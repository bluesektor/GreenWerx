// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    [Table("Vendors")]
    public class Vendor:Node, INode
    {
        public Vendor()
        {
            this.UUIDType = "Vendor";
        }
        public bool Farmer { get; set; }

        public bool Breeder { get; set; }
       
        public string BreederType { get; set; }

        public bool Dispensary { get; set; }
    }
}
