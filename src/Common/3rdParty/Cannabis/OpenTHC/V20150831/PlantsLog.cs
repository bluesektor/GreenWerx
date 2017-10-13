using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public  class PlantsLog
     {
        public long Id  { get; set; }
        public long PlantId { get; set; }
        public string Strain { get; set; }
        public DateTime SessionTime { get; set; }
        public long TransactionId { get; set; }
        public long Seed { get; set; } 
        public long InventoryId { get; set; }
        public long ParentId { get; set; } 
        public int Deleted { get; set; }
        public DateTime DeleteTime { get; set; } 
        public long Location { get; set; } 
        public string State { get; set; }
        public bool CloneConverted  { get; set; } 
        public int Seized { get; set; } 
        public long OrgiId { get; set; } 
        public bool  HarvestScheduled { get; set; } 
        public DateTime  HarvestScheduleTime { get; set; }
        public DateTime    Created { get; set; }
        public bool  HarvestCollect { get; set; }
        public bool  CureCollect { get; set; }
        public long  Room { get; set; }
        public long TransactionId_Original { get; set; }
    }
}
