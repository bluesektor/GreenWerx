// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models
{
    [Table("UnitsOfMeasure")]
    public class UnitOfMeasure : Node, INode
    {
        public UnitOfMeasure()
        {
            UUIDType = "UnitOfMeasure";
        }


        public string ShortName { get; set; }

        public string Category { get; set; }

    }
 
 }
