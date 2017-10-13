using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    //    This  contains  all  inventory  adjustment  information.  It  is  useful  for  retrieving 
    //historical data and should not be necessary in most scenarios. This function will 
    //only return active entries.
    class Inventory_Adjust
    {
        //OpenTHC Imported
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public decimal OldWeight { get; set; }
        public decimal NewWeight { get; set; }
        public decimal Difference { get; set; }
        public DateTime SessionTime { get; set; }
        public long TransactionId { get; set; }
        public long OrgiId { get; set; }
        public string Reason { get; set; }
        public string AType { get; set; }
        public long Location { get; set; }
        public long TransactionId_Original { get; set; }
    }
}
