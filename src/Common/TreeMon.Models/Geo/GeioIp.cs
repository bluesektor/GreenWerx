// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeMon.Models.Geo
{
   public class GeoIp
    {
        public string LocationUUID { get; set; }

        #region blocks file

        public int LocationId { get; set; }

        public float StartIpNum { get; set; }

        public float EndIpNum { get; set; }

        #endregion

        #region Location File

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string MetroCode { get; set; }

        public string AreaCode { get; set; }

        #endregion
    }
}
