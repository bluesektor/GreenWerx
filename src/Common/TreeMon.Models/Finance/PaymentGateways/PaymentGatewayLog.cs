// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Finance.PaymentGateways
{
    [Table("PaymentGatewayLogs")]
    public class PaymentGatewayLog
    {
        public PaymentGatewayLog()
        {
            this.UUID = Guid.NewGuid().ToString("N");
        }

        [Key]
        public int Id { get; set; }

        [StringLength(32)]
        public string UUID { get; set; }

        public string   RequestDirection { get; set; }

        public string Gateway { get; set; }

        public DateTime RequestDate { get; set; }

        //IPN, PDT
        public string RequestType { get; set; }

        public string IpAddress { get; set; }

        public string Payload { get; set; }

        //httprequest, specific gateway data..
        public string PayloadType { get; set; }
    }
}
