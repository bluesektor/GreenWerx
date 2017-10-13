using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//AUCTION types silent,english, dutch, Converging bid auctions (commodities market).
// drop dead, extend if bid within last x seconds
//reserve price
//private auction
//farmers to dispensaries. dispensaries to customers

//Market your auction. Do like a send grid 

//Sale by auction.
//(1) In a sale by auction if goods are put up in lots each lot is the subject of a separate sale.
//(2) A sale by auction is complete when the auctioneer so announces by the fall of the hammer or in other customary manner. Where a bid is made while the hammer is falling in acceptance of a prior bid the auctioneer may in his or her discretion reopen the bidding or declare the goods sold under the bid on which the hammer was falling.
//(3) Such a sale is with reserve unless the goods are in explicit terms put up without reserve. In an auction with reserve, the auctioneer may withdraw the goods at any time until he or she announces completion of the sale. In an auction without reserve, after the auctioneer calls for bids on an article or lot, that article or lot cannot be withdrawn unless no bid is made within a reasonable time. In either case a bidder may retract his or her bid until the auctioneer's announcement of completion of the sale, but a bidder's retraction does not revive any previous bid.
//(4) If the auctioneer knowingly receives a bid on the seller's behalf or the seller makes or procures such a bid, and notice has not been given that liberty for such bidding is reserved, the buyer may at his or her option avoid the sale or take the goods at the price of the last good faith bid prior to the completion of the sale. This subsection shall not apply to any bid at a forced sale.

namespace BioTrackTHC.V4
{
    //    This contains inventory information as previously submitted. It is license specific 
    //and  can  be  queried  with  all  records  or  only  active  ones.  Active  records  are 
    //considered to be inventory that has not been moved into cultivation, zeroed or
    //destroyed.
    class Inventory
    {
        public string BarcodeId { get; set; } //inventory identifier
        public string  Data{ get; set; }  //Array of 1 or more nodes containing inventory information

        //Decimal value, optional if 
        //remove_quantity //is provided, new quantity to adjust to.
        public decimal Quantity{ get; set; }

        //variable length text field.
        //Valid values  are: g, mg, kg, oz, lb, each. These 
        //represent: grams, milligrams, 
        //kilograms, ounces, pounds, each.
        public string Quantity_UOM { get; set; }

        //Decimal value, optional if quantity is 
        //provided, quantity to remove. Does 
        //not need to be remaining quantity 
        //(can be a partial removal).
        public decimal remove_quantity{ get; set; }

        //variable length text field. Valid values 
        //are: g, mg, kg, oz, lb, each. These 
        //represent: grams, milligrams, 
        //kilograms, ounces,
        //pounds, each.
        public string Remove_Quantity_Uom{ get; set; }

        //variable length text field explaining in 
        //greater detail the reason for the 
        //removal or addition of inventory
        public string Reason{ get; set; }
        // public string RemoveReason { get; set; }  //OpenTHC Imported

        //Integer value representing the type of 
        //adjustment.
        public int Type { get; set; }

        //	license number of location
        public long Location { get; set; }
        //	variable length text field
        public string Strain { get; set; }
        public string Source_Id { get; set; }//text field, optional when within the 15 day period
        public string InventoryType { get; set; }




        //OpenTHC Imported
        public long Id { get; set; }
        public long ParentId { get; set; }
        public DateTime SessionTime { get; set; }

        public decimal Weight { get; set; }
        public long TransactionId { get; set; }
        public long PlantId { get; set; }

        public decimal RemainingWeight { get; set; }
        public int RequiresWeighing { get; set; }

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
      
        public int InventoryStatus { get; set; }
        public DateTime InventoryStatusTime { get; set; }
        public decimal InventoryParentIdPct { get; set; }
        public long Sample_Id { get; set; }
        public long TransactionId_Original { get; set; }
        public int Recalled { get; set; }
        public DateTime Created { get; set; }
        public long Day { get; set; } 
    }
}
