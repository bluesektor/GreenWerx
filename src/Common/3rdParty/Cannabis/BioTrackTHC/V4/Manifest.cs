using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This contains manifest information as previously submitted. It is license specific 
//and can be queried with all records or only active ones.
    class Manifest
    {
        //Unix timestamp
        //
        public double SessionTime { get; set; }

        //Array   of   1   or   more   nodes   containing 
        //transportation information
        //
        public string Data { get; set; }
        //Integer, number of separate items
        public int Item_Count{ get; set; }

            // license  number  of shipping entity
        public string License_Number{ get; set; }

            //unique  manifest identifier
            public string Manifest_Id{ get; set; }

                // name   of   the shipping entity
        public string Trade_Name{ get; set; }

            //Date of actual shipment
        public DateTime Transfer_Date { get; set; }

    }
}
