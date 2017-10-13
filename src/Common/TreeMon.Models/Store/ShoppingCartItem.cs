// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    [Table("ShoppingCartItems")]
    public class ShoppingCartItem: Node, INode
    {
        public ShoppingCartItem()
        {
            this.UUIDType = "ShoppingCartItem";
        }
        public decimal Price { get; set; }
    
        [NotMapped]
        public decimal TotalPrice { get { return Price * (decimal)Quantity; } }

      
        public float Quantity { get; set;}

     
        public string SKU { get; set; }

        public string ItemUUID { get; set; }

        public string ItemType { get; set; }

        public string ShoppingCartUUID { get; set; }

        //User purchasing the item.
        //this is used to see scan the orders table and
        //make sure a payment actually settles
        //if after x time scan the orders table check if user id is
        //logged in. if not cancel the order and return the stock.
        //
        public string UserUUID {get; set; }

        //This is used to group the cart items
        public string SessionKey { get; set; }

        public DateTime DateAdded { get; set; }

        public string ShippingMethodUUID { get; set; }

    }
}
