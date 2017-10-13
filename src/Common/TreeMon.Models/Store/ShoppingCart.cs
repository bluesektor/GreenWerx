// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    //This was CheckOutForm, changed it to shopping cart and is passed in
    //from the checkout ui to process the payment and keep track on the client the 
    //items in the cart as a guest.
    [Table("ShoppingCarts")]
    public class ShoppingCart:Node,INode
    {
        public ShoppingCart() {
          
            this.UUIDType = "ShoppingCart";
         
        }

        public int AffiliateUUID { get; set; }

        public string BillingLocationUUID { get; set; }

        public string CurrencyUUID { get; set; }

        public decimal Discount { get; set; }

        public decimal ShippingCost { get; set; }

        public string ShippingLocationUUID { get; set; }

        public string ShippingMethodUUID { get; set; }

        public bool ShippingSameAsBiling { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Taxes { get; set; }

        public decimal Total { get; set; }

        public string UserUUID { get; set; }

        public string FinanceAccountUUID { get; set; }

    }
}