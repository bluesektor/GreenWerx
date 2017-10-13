using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
  //  This contains inventory room information as previously submitted. It is license 
//specific and can be queried with all records or only active ones.
    class Inventory_Room
    {
        public long Id { get; set; }
        public long OrgiId { get; set; }
   
        public long Location { get; set; }//	license number of location value
        public int RoomId { get; set; }
        public string Name { get; set; }
        public int Deleted { get; set; }
        public bool Quarantine { get; set; }
        public long TransactionId { get; set; }
        public long TransactionId_Original { get; set; }
    }
}
