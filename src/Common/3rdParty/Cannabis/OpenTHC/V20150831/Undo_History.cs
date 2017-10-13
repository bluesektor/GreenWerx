using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Undo_History
     {
        public long Id  { get; set; }
        public DateTime SessionTime { get; set; } 
        public string Action { get; set; } 
        public long OrgiId { get; set; } 
        public string  BarCode { get; set; }
        public long TransactionId  { get; set; } 
        public long TransactionId_Original { get; set; }
     
    }
}
