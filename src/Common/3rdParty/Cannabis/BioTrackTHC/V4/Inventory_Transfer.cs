using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This  contains  inventory  transfer  information  as  previously  submitted.  It  is 
//license specific and can be queried with all records or only active ones.

    class Inventory_Transfer
    {
        //decimal value, new quantity of combined items
        public decimal Lot_Quantity { get; set; }


        
        //Valid values are: g, mg, kg, oz, lb, each. These 
        //represent: grams, milligrams, 
        //kilograms, ounces, pounds, each.
        public string Lot_Quantity_UOM { get; set; }

        //Array of 1 or more nodes containing 
        //inventory information
        public string Data { get; set; }

        //inventory identifier  
         public string BarcodeId { get; set; }
        //public long InventoryId { get; set; }


         // quantity to remove. 
         //Does not need to be remaining 
         //quantity (can be a partial 
         //combination).
        public decimal Remove_Quantity { get; set; }

        //variable length text field. 
        //Valid values are: g, mg, kg, oz, lb, each. These 
        //represent: grams, milligrams, kilo
        //grams, ounces, pounds, each
        //
        public string Remove_Quantity_UOM { get; set; }




        #region OUTBOUND TRANSFER

        //manifest identifier obtained from 
        //previously filed manifest
        //
         public string         Manifest_Id{ get; set; }

         //Optional if inter-UBI transfer, decimal value that indicates how 
         //much the item was sold for before any applicable taxes.
        //
        public decimal Price { get; set; }
        #endregion

        #region INBOUND Transfer
        public string Location { get; set; }        //license number of location
       
        public decimal Quantity { get; set; }        //Quantity or amount received

        //variable length text field. Valid values 
        //are: g, mg, kg, oz, lb, each. These 
        //represent: grams, milligrams, 
        //kilograms, ounces, pounds, each
        public string UOM { get; set; }     
        
#endregion 


        //public long Id { get; set; }
      
        //public string Strain { get; set; }
        //public decimal Weight { get; set; }
        //public long TransactionId { get; set; }
        //public long Location { get; set; }
        //public int Direction { get; set; }
        //public int RequiresWeighing { get; set; }
        //public int TransferType { get; set; }
        //public long OrgiId { get; set; }
        //public long ParentId { get; set; }

        //public string InventoryType { get; set; }
        //public decimal UsableWeight { get; set; }
        //public string Outbound_License { get; set; }
        //public string Inbound_License { get; set; }
        //public string Description { get; set; }
        //public decimal SalePrice { get; set; }
        //public long ManifestId { get; set; }
        //public bool Manifest_Stop { get; set; }
        //public bool Received { get; set; }
        //public decimal ReceiveWeight { get; set; }
        //public int Deleted { get; set; }
        //public decimal UnitPrice { get; set; }
        //public bool Is_Refund { get; set; }
        //public decimal Refund_Amount { get; set; }
        //public long Inbound_Location { get; set; }
        //public long TransactionId_Original { get; set; }
        //public long Inbound_OrgId { get; set; }
        //public long Transactionid_Original_Inbound { get; set; }
    }
}
