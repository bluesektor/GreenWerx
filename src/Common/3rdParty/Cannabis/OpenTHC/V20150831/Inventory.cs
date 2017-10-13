using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Inventory
     {
        public long Id  { get; set; }
        public long ParentId { get; set; }
        public DateTime SessionTime { get; set; } 
        public string Strain { get; set; } 
        public decimal Weight { get; set; }
        public long TransactionId { get; set; } 
        public long PlantId  { get; set; } 
        public long Location { get; set; } 
        public decimal RemainingWeight { get; set; }
        public int RequiresWeighing { get; set; }
        public string InventoryType { get; set; }	
        public int Wet { get; set; }	
        public int Seized { get; set; }	
        public long OrgiId { get; set; }	
        public int Deleted { get; set; }	
        public string PlantArray { get; set; }	
        public decimal UsableWeight { get; set; }	
        public int RemoveScheduled { get; set; }	
        public DateTime RemoveTime { get; set; }	
        public long InventoryParentId { get; set; }	
        public string ProductName { get; set; }	
        public int CurrentRoom { get; set; }	
        public long IdSerial { get; set; }	
        public string RemoveReason { get; set; }	
        public int InventoryStatus { get; set; }	
        public DateTime InventoryStatusTime { get; set; }	
        public decimal InventoryParentIdPct { get; set; }	
        public long Sample_Id { get; set; }	
        public long Source_Id { get; set; }	
        public long TransactionId_Original { get; set; }	
        public int Recalled { get; set; }
    }
}
