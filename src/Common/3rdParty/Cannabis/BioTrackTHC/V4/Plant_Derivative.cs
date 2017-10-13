using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This  contains  plant  yield  information  as  previously  submitted.  It  is  license 
//specific and can be queried with all records or only active ones.
    class Plant_Derivative
    {
        //from openthc

        //this is usually barcodeid
        public long Id { get; set; }
        public long PlantId { get; set; }
        public DateTime SessionTime { get; set; }
        public long TransactionId { get; set; }
        public int Deleted { get; set; }
        public DateTime Created { get; set; }
        public string InventoryType { get; set; }
        public decimal Weight { get; set; }
        public bool AccountedFor { get; set; }
        public long InventoryId { get; set; }
        public decimal WholeWeight { get; set; }
        public long OrgiId { get; set; }
        public long Location { get; set; }
        public long Droom { get; set; }
        public bool AddlCollections { get; set; }

        public long TransactionId_Original { get; set; }

    }
}
