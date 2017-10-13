using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class InventoryConversions
     {
        public long Id  { get; set; }
        public long InventoryId { get; set; } 
        public decimal OldWeight { get; set; }
        public decimal NewWeight { get; set; }
        public decimal Difference { get; set; } 
        public DateTime SessionTime { get; set; } 
        public long TransactionId { get; set; } 
        public long ChildId { get; set; } 
        public decimal ChildWeight { get; set; }
        public int RequiresWeighing { get; set; }
        public long OrgiId { get; set; }
        public string InventoryArray { get; set; } 
        public string InventoryType { get; set; }
        public long Location { get; set; } 
        public long TransactionId_Original { get; set; }
        public string  InventoryType_Parent  { get; set; }
        public decimal ParentWeight_Start { get; set; } 
        public decimal ParentWeight_End { get; set; } 
        public decimal Parent_Difference { get; set; }
        public decimal Child_UsableWeight  { get; set; }
    }
}
