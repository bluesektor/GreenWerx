using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    class Inventory_Statistics
     {
        public long Id  { get; set; }
        public long OrgiId { get; set; }
        public long Location { get; set; }
        public string InventoryType { get; set; }
        public decimal Quantity { get; set; } 
        public DateTime Created { get; set; }
        public long Day  { get; set; } 
    }
}
