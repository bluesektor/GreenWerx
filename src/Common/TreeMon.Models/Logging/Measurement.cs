// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace TreeMon.Models.Logging
{
    [Table("Measurements")]
    public class MeasurementLog :Node, INode
    {
        public MeasurementLog()
        {
            UUIDType = "MeasurementLog";
        }

        [StringLength(32)]
        public string ItemUUID{ get; set; }
        
        public string ItemType { get; set; }

        public float Measure { get; set; }
       
        public string MeasureType { get; set; }

       
        public string UnitOfMeasure { get; set; }

        
        public string Instrument { get; set; }

        [StringLength(32)]
        public string InstrumentUUID { get; set; }

        [StringLength(32)]
        public string ByUUID { get; set; }

        public DateTime DateTaken { get; set; }

        [StringLength(32)]
        public string LocationUUID { get; set; }


        public string Reason { get; set; }

        public string Comments { get; set; }

        //Was it measured by a lab
        public bool LabMeasurement { get; set; }


        public string Group { get; set; }
        

    }
}
