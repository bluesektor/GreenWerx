// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Store
{
    [Table("Orders")]
    public class Order : Node, INode
    {
        public Order()
        {
            this.UUIDType = "Order";
        }

        public string AddedBy { get; set; }

        public int AffiliateUUID { get; set; }

        public string BillingLocationUUID { get; set; }

        public string CurrencyUUID { get; set; }

        public string CustomerEmail { get; set; }

        public decimal Discount { get; set; }

        public string FinancAccountUUID { get; set; }

        //lets us know the AffiliateManager came through and copied the data to the affiliate table.
        public bool ReconciledToAffiliate { get; set; }

        public decimal ShippingCost { get; set; }

        public DateTime? ShippingDate { get; set; }

        public string ShippingLocationUUID { get; set; }

        public string ShippingMethodUUID { get; set; }

        public bool ShippingSameAsBiling { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Taxes { get; set; }

        public decimal Total { get; set; }

        public string TrackingUUID { get; set; }

        public string TransactionID { get; set; }

        public string UserUUID { get; set; }

        public string CartUUID { get; set; }

        public string PayStatus { get; set; }

    }
}
