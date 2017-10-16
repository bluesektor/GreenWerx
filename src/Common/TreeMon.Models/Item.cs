// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models
{

    public  class Item:Node, INode
    {
        public Item() : base() { }


        public bool Custom { get; set; }

        /// <summary>
        /// This the price paid by the store/product owner.
        /// If the TotalPrice is less than this, or a defined percentage above
        /// then we need an override to sell at a loss or reduced profit.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// New, used
        /// </summary>
        public string Condition { get; set; }
        public string Quality { get; set; }

        public float Rating { get; set; }

        [StringLength(32)]
        public string CategoryUUID { get; set; }

        [StringLength(32)]
        public string GroupUUID { get; set; }

        /// <summary>
        /// Set to true if the product is virtual
        /// </summary
        [JsonConverter(typeof(BoolConverter))]
        public bool Virtual { get; set; }

        /// <summary>
        /// This is weight diplayed for the customer.
        /// Does NOT include packaging weight
        /// if UnitWeight and sale weight differ then you need to do a transfer from to
        /// </summary>    
        [DefaultValue(0)]
        public float Weight { get; set; }

        public string UOMUUID { get; set; }

        [NotMapped]
        public string WeightUOM { get; set; }


        [StringLength(32)]
        public string DepartmentUUID { get; set; }

        public string Description { get; set; }


        /// <summary>
        ///        Discount for the product as a percentage.
        /// When updating these records:
        ///   If you specify Discount without specifying price, the price is adjusted to accommodate the new Discount value, 
        ///   and the UnitPrice is held constant.
        ///   If you specify both Discount and Quantity, you must also specify either TotalPrice or UnitPrice so the system knows which one to automatically adjust.
        /// </summary>
        public float Discount { get; set; }

        /// <summary>
        /// Number to apply the type 
        /// </summary>
        public float MarkUp { get; set; }

        /// <summary>
        /// percent, numeric, multiplier, function/formula (would have to figure this out first).
        /// </summary>
        public string MarkUpType { get; set; }

        /// <summary>
        /// This is the price on display for the customer
        /// </summary>
        /// 
        public decimal Price { get; set; }

        //StringLength(50)]
        public string SKU { get; set; }

        //Manufacturer
        [StringLength(32)]
        public string ManufacturerUUID { get; set; }

        //Account
        public string ManufacturerUUIDType { get; set; }

        public DateTime? Expires { get; set; }

        public string SerialNumber { get; set; }

        //reserved for custom tracking.
        public string SystemUUID { get; set; }

        //how many units were in the product 
        public float UnitsInProduct { get; set; }

        //Download,access,trade, jars etc..
        public string UnitType { get; set; }


    }
}
