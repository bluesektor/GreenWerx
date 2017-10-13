// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;

namespace TreeMon.Models.Datasets
{
    public class DataPoint
    {
        public DataPoint() { }

        public DataPoint(string caption, string dataValue, string dataType, DateTime pointDate, int pointIndex, bool isSeries = true)
        {
            Caption = caption;
            Value = dataValue;
            ValueType = dataType;
            Date = pointDate;
            Index = pointIndex;
            IsSeries = isSeries;
        }
        public int RoleWeight { get; set; }

        public bool IsSeries { get; set; }

        public string Caption { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }

        public DateTime? Date { get; set; }
      
        public int Index { get; set; }
    }

    public class Coordinate
    {
        public Coordinate() { }

        public Coordinate(string caption, float lat, float lon , DateTime pointDate, int pointIndex)
        {
            Caption = caption;
            Lat = lat;
            Lon = lon;
            Date = pointDate;
            Index = pointIndex;            
        }
        public int RoleWeight { get; set; }

        public string Caption { get; set; }

        public float Lat { get; set; }

        public float Lon { get; set; }

        public DateTime? Date { get; set; }

        public int Index { get; set; }
    }
}
