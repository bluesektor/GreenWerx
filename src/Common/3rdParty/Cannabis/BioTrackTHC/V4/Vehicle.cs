using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    
    //This contains vehicle information as previously submitted. It is UBI specific (as 
    //opposed to license specific) and can be queried with all records or only active 
    //ones.

    class Vehicle
    {
         public int  Vehicle_Id   { get; set; } //	unique integer
         public string   Color { get; set; } //	variable length text field
         public string   Make	{ get; set; }// variable length text field
         public string   Model	{ get; set; }// variable length text field
         public string   Plate	{ get; set; }// variable length text field
         public string   Vin	{ get; set; }// variable length text field
    }
}
