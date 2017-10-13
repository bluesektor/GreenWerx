// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Logging
{
    /// <summary>
    /// for logging items in a ledger, or items being sold.
    /// </summary>
    [Table("LineItems")]
    public class LineItemLog : Item, INode
    {

        public LineItemLog()
        {
            UUIDType = "LineItemLog";
        }
  
        public float Quantity { get; set; }
  
        public decimal Subtotal { get; set; }

        public decimal TotalPrice { get; set; }

        /// <summary>
        /// ISO code for the currency
        /// </summary>
        public string CurrencyCode { get; set; }



        /// <summary>
        /// This is the weight actually sold to the customer
        /// If this differs from the UnitWeight, then a transfer has to occur.
        /// If no transfer occurred then there is a possibility of loss somewhere in this transaction
        /// </summary>
        public float SaleWeight { get; set; }

        public string SaleWeightUOM { get; set; }

        [StringLength(32)]
        public string TerminalUUID { get; set; }


        /// <summary>
        /// from Users
        /// </summary>
        [StringLength(32)]
        public string BuyerUUID { get; set; }

        [StringLength(32)]
        public string BuyerAccountUUID { get; set; }


        /// <summary>
        /// from users
        /// </summary>
        [StringLength(32)]
        public string SellerUUID { get; set; }

        [StringLength(32)]
        public string SellerAccountUUID { get; set; }


      
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        ///  Wholesale, retail,consignment
        /// </summary>
        public string Category { get; set; }


        //sale,donation, transfer(internal only?), return, refund,auction, gift
        //
        public string Type { get; set; }

     


    }
}
