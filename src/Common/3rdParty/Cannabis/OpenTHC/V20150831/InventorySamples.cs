using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class InventorySamples
     {
        public long Id  { get; set; }
        public long OrgiId { get; set; }
        public long Sample_Type { get; set; } 
        public long Vendor_Id { get; set; } 
        public long InventoryId { get; set; } 
        public decimal Quantity { get; set; } 
        public DateTime SessionTime { get; set; } 
        public long TransactionId { get; set; } 
        public long Location { get; set; } 
        public long TransactionId_Original { get; set; }
        public int Deleted { get; set; }
    }
}
