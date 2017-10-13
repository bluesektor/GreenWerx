using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Labresults_Samples
     {
        public long Id  { get; set; }
        public long InventoryId { get; set; } 
        public decimal Quantity { get; set; } 
        public DateTime SessionTime { get; set; }
        public string InventoryType { get; set; } 
        public string Strain { get; set; }
        public bool Product_Public  { get; set; } 
        string Name { get; set; } 
        public int Deleted { get; set; } 
        public decimal  Result         { get; set; } 
        public string  Lab_License    { get; set; } 
        public bool Sample_Use     { get; set; } 
        public long Location { get; set; } 
        public long OrgiId { get; set; } 
        public long TransactionId { get; set; } 
        public long ParentId { get; set; } 
        public bool  Received { get; set; } 
        public decimal    Received_Quantity { get; set; }  
        public long TransactionId_Original { get; set; } 
        public decimal   Sample_Amount_Used { get; set; } 
        public decimal    Sample_Amount_Destroyed { get; set; }  
        public decimal    Sample_Amount_Other { get; set; }  
        public long  Other_Sample_Id { get; set; } 
        public long InventoryParentId { get; set; }
    }
}
