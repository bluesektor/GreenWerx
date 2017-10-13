using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class InventoryLog
     {
        public long logid  { get; set; }
        public long Id  { get; set; }
        public DateTime SessionTime { get; set; } 
        public string Strain { get; set; } 
        public decimal Weight { get; set; } 
        public long TransactionId { get; set; } 
        public long PlantId { get; set; } 
        public long Location { get; set; } 
        public decimal RemainingWeight { get; set; } 
        public int RequiresWeighing { get; set; }
        public long ParentId  { get; set; }
    }
}
