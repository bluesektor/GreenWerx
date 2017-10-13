using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This  contains  end-customer  sale 
//information  as  previously  submitted.  It  is 
//license specific and can be queried with all records or only active ones.
    class Sale
    {
        //Array of 1 or more nodes containing 
        //inventory information
        public string Data { get; set; }

        //inventory identifier
        public string BarcodeId { get; set; }

        //integer value, quantity to remove
        public int Quantity { get; set; }


        //the price paid before any applicable taxes.
        public decimal Price { get; set; }

        //Optional, integer, should be provided 
        //if multiple line items of the same barcode were included in one sale. 0 
        //would represent the first item (in the order submitted to the system), 1 the 
        //next, etc.
        //
        public int Item_Number { get; set; }

        //Optional, unix 32-bit integer 
        //timestamp of when the sale occurred. 
        //If not used, will default to current 
        //time. Otherwise, the time must not 
        //be in the future and, also, must not 
        //be in a locked tax period
        //
        public double Sale_Time { get; set; }


        //openthc 
        //public long Id { get; set; }
        //public long OrgiId { get; set; }
        //public decimal Weight { get; set; }
        //public DateTime SessionTime { get; set; }
        //public long InventoryId { get; set; }
        //public long TransactionId { get; set; }
        //public long Location { get; set; }
        //public decimal Price { get; set; }
        //public decimal UsableWeight { get; set; }
        //public long ItemNumber { get; set; }
        //public int Deleted { get; set; }
        //public int Refunded { get; set; }
        //public long TransactionId_Original { get; set; }
        //public string InventoryType { get; set; }
        //public DateTime Created { get; set; }   
    }
}
