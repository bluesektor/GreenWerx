using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class InventoryCombinations
     {
        public long Id  { get; set; }
        public long NewId { get; set; } 
        public long OldId { get; set; } 
        public DateTime SessionTime { get; set; } 
        public long TransactionId { get; set; } 
        public long NewType { get; set; } 
        public long OldType { get; set; }
        public long TransactionId_Original { get; set; }
    }
}
