using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    //    This contains   basic   quality   assurance   sample   information   as   previously 
    //submitted. It is license specific and can be queried with all records or only active 
    //ones. As QA derived samples receive their own identifier; this list can be used 
    //to cross-reference said samples currently (or previously) in inventory.
    //
    class Inventory_QA_Sample
    {
        public string BarCodeId { get; set; }//inventory identifier
        public string  Lab_Id { get; set; } //variable length text field, license number of the QA facility
        public decimal Quantity{ get; set; }//decimal value, quantity of old  product before conversion

        //Valid values are: g, mg, kg, oz, lb, 
        //each. These represent: grams, 
        //milligrams, kilograms, ounces, pounds, each.
        public string Quantity_UOM { get; set; }

        //Optional. If the inventory type is 13 
        //(flower lot), this field should be 1 to 
        //indicate the lot will be used to 
        //convert to usable marijuana (type 28, 
        //e.g. pre-packs), or 0 to indicate it will 
        //be used for an extract. Converting 
        //directly to type 28 will trigger more 
        //rigorous QA test requirements
        public bool Use { get; set; }


        //OpenTHC Imported

        public long Id { get; set; }
        public long OrgiId { get; set; }
        public long Sample_Type { get; set; }
        public long Vendor_Id { get; set; } //vendor who grew/created the sample?
        public long InventoryId { get; set; }
       
        public DateTime SessionTime { get; set; }
        public long TransactionId { get; set; }
        public long Location { get; set; }
        public long TransactionId_Original { get; set; }
        public int Deleted { get; set; }

  
    }
}
