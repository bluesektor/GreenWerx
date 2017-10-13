// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using TreeMon.Models.Finance;
using TreeMon.Models.Geo;
using TreeMon.Models.Membership;

namespace TreeMon.Models.Store
{
    public class CartView:ShoppingCart
    {
        public CartView()
        {
          
            this.CartItems = new List<dynamic>();
            this.BillingAddress = new Location();
            this.ShippingAddress = new TreeMon.Models.Geo.Location();
            this.Customer = new TreeMon.Models.Membership.User();
            this.Customer.UUID = "";
            // this.PriceRule = new PriceRule();
            this.PriceRules = new List<PriceRuleLog>();
        }

        public List<PriceRuleLog> PriceRules { get; set; }

        public Location BillingAddress { get; set; }

        public User Customer { get; set; }

        public Location ShippingAddress { get; set; }

        public List<dynamic> CartItems { get; set; }

        public string  PaidTo { get; set; }
        public string PaidType { get; set; }
        public bool IsPaymentSandbox { get; set; }
        public string PaymentGateway { get; set; }


    }
}