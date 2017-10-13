// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Medical
{
    [Table("Symptoms")]
    public class Symptom : Node, INode
    {
        public Symptom()
        {
            UUIDType = "Symptom";
        }

        public string Category { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        //This is used on the client so we know the symptom is generic and
        //that the user needs to input the location of the symptom.
        //
        public bool LocationRequired { get; set; }

       


    }

 
}
