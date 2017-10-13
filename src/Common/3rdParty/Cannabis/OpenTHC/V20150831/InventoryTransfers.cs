using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class InventoryTransfers
     {
        public long Id  { get; set; }
        public long InventoryId { get; set; }
        public string Strain { get; set; } 
        public decimal Weight { get; set; } 
        public long TransactionId { get; set; }
        public long Location { get; set; }
        public int Direction { get; set; } 
        public int RequiresWeighing { get; set; }
        public int TransferType { get; set; } 
        public long OrgiId { get; set; }
        public long ParentId { get; set; }
     
        public string InventoryType { get; set; }	
        public decimal UsableWeight { get; set; }	
        public string    Outbound_License	             { get; set; }
        public string     Inbound_License	             { get; set; }
        public string    Description	                 { get; set; }
        public decimal      SalePrice	             { get; set; }
        public long     ManifestId	             { get; set; }
        public bool       Manifest_Stop        { get; set; }
        public bool       Received	         { get; set; }
        public decimal           ReceiveWeight	 { get; set; }
        public int Deleted { get; set; }	
        public decimal   UnitPrice	                  { get; set; }
        public bool     Is_Refund	              { get; set; }
        public decimal     Refund_Amount	          { get; set; }
        public long       Inbound_Location	  { get; set; }
        public long TransactionId_Original { get; set; }
        public long   Inbound_OrgId	 { get; set; }
        public long    Transactionid_Original_Inbound { get; set; }

    }
}
