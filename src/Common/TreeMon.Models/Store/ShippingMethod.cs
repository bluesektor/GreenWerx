// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    [Table("ShippingMethods")]
    public class ShippingMethod : Node, INode
    {
        public ShippingMethod()
        {
            this.UUIDType = "ShippingMethod";
        }
        public decimal Price
        {
            get; set;
        }
    }
}
