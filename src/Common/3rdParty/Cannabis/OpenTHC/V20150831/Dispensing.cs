using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    //Table should be called Orders?  
    public class Dispensing
     {
        public long Id  { get; set; }
        public long  OrgiId { get; set; }
        public decimal Weight { get; set; } 
        public  DateTime SessionTime { get; set; }
        public long InventoryId  { get; set; }
        public long TransactionId  { get; set; }
        public long Location  { get; set; }
        public decimal Price  { get; set; }
        public decimal UsableWeight { get; set; }
        public long ItemNumber  { get; set; }
        public int   Deleted { get; set; }
        public int Refunded  { get; set; }
        public long   TransactionId_Original { get; set; }
        public string   InventoryType { get; set; }
        public DateTime Created  { get; set; }   
    }
}
