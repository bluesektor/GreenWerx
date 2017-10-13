// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models.Finance
{
    [Table("Currency")]
    public class Currency :Node, INode
    {

        public Currency()
        {
            this.UUIDType = "Currency";
        }


        public string AssetClass        { get; set; }

        public string Country       { get; set; }

        public string Code { get; set; }

        public string Symbol         { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool   Test             { get; set; }

    }
}
