// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMon.Models.Equipment
{
    [Table("Bulbs")]
    public class Bulb:Item, INode
    {
        public Bulb()
        {
            UUIDType = "Bulb";
            CategoryUUID = UUIDType;
        }
        //flourescent
        //     Mercury-vapor lamps
        // Metal-halide(MH) lamps
        //Ceramic MH lamps
        // Sodium-vapor lamps
        //Low-pressure sodium vapor lamps are extremely efficie
        // Xenon short-arc lamps

        public int Watts { get; set; }

        public int Lumens { get; set; }

        //red blue green?
        public int Spectrum { get; set; }

        public float HoursUsed { get; set; }
    }
}
