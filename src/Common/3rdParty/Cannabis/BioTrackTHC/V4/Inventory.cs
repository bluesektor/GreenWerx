using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
