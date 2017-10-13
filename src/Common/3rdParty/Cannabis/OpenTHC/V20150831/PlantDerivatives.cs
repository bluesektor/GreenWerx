using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class PlantDerivatives
     {
            public long Id  { get; set; }
            public long PlantId { get; set; } 
            public DateTime SessionTime { get; set; }
            public long TransactionId { get; set; }
            public int Deleted { get; set; } 
               public DateTime Created { get; set; }
            public string InventoryType { get; set; } 
           public decimal  dWeight { get; set; } 
           public bool dAccountedFor { get; set; } 
            public long InventoryId { get; set; } 
           public decimal  dWholeWeight { get; set; } 
            public long OrgiId { get; set; } 
            public long Location { get; set; } 
          public long   Droom { get; set; }
          public bool AddlCollections  { get; set; } 

        public long TransactionId_Original { get; set; }

    }
}
