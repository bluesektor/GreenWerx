// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeMon.Models.Geo
{
    public class GeoCoordinate
    {
        public GeoCoordinate()
        {
            MaxDistance = 100.0;
            Distances = new List<GeoCoordinate>();
        }
        public double MaxDistance { get;   }

        public string LocationType { get; set; }

        public double SearchDistance { get; set; }

        public string UUID { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Distance { get; set; }

        public List<string> Tags { get; set; }

        public List<GeoCoordinate> Distances { get; set; }
    }

    
}
