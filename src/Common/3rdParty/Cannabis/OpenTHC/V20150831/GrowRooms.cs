using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class GrowRooms
     {
        public long Id  { get; set; }
        public long OrgiId { get; set; }
        public long Location  { get; set; }
        public long RoomId  { get; set; }
        public string Name  { get; set; }
        public int   Deleted { get; set; }
        public long TransactionId  { get; set; }
    }
}
