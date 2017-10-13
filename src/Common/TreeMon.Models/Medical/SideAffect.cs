// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Medical
{
    [Table("SideAffects")]
    public class SideAffect:Node, INode
    {

         public SideAffect()
        {
            UUIDType = "SideAffect";
        }
        //This is to log when the symptom was observed
        //use status for start, stop, peak etc..
        public DateTime? SymptomDate { get; set; }

        [StringLength(32)]
        public string DoseUUID { get; set; }

        [StringLength(32)]
        public string SymptomUUID { get; set; }

        public float Severity { get; set; }

        public string Category { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        //This is used on the client so we know the symptom is generic and
        //that the user needs to input the location of the symptom.
        //
        public bool LocationRequired { get; set; }

        public float Efficacy { get; set; }

        public float Duration { get; set; }

        public string DurationMeasure { get; set; }
    }
}
