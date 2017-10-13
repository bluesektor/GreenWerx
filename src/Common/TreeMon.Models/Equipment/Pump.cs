// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Equipment
{
    [Table("Pumps")]
    public class Pump:Item, INode
    {
        public Pump()
        {
            UUIDType = "Pump";
            CategoryUUID = UUIDType;
        }
    }
}
