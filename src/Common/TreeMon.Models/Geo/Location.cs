// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMon.Models.Helpers;

namespace TreeMon.Models.Geo
{
    [Table("Locations")]
    public class Location:Node, INode
    {
        public Location()
        {
            UUIDType = "Location";

        }

        //This is to link an account to a location.
        //Set this to the account.UUID
        public string AccountReference { get; set; }

        //this replaces the Id field on the insert. the ParentId will reference this.
        public int RootId { get; set; }

        public string Abbr { get; set; }

        public string Code        { get; set; }

        public int CurrencyUUID { get; set; }

        public string LocationType        { get; set; }

        public float Latitude        { get; set; }

        public float Longitude        { get; set; }

        public int TimeZoneUUID        { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Postal { get; set; }

        public int Type { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsBillingAddress { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool Virtual { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool isDefault { get; set; }
    }
}
