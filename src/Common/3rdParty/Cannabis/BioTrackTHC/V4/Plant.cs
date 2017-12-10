using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BioTrackTHC.V4
{
//    This contains plant information as previously submitted. It is license specific and 
//can be queried with all records or only active ones. Active records are considered 
//to be plants that have not been destroyed or moved into inventory.
    class Plant
    {
        public string Strain { get; set; }

        //license number of location
        public string Location	 { get; set; }
        public int Room	 { get; set; }

        //	text field representing unique identifier
        public string Source { get; set; }
        public decimal Quantity { get; set; }
        public int Mother { get; set; }

        //	Optional, 8 character birthdate in the following format: YYYYMMDD. If
        //not provided, the system will defaultto the current date.
        public DateTime BirthDate { get; set; }
                                            
        public string BarcodeId { get; set; }



        //openthc
        public long Id { get; set; }

  
        public DateTime SessionTime { get; set; }
        public long TransactionId { get; set; }
        public long Seed { get; set; }
        public long ParentId { get; set; }
        public int Deleted { get; set; }
        public DateTime DeleteTime { get; set; }
  
        public string State { get; set; }
        public bool CloneConverted { get; set; }
        public int Seized { get; set; }
        public long OrgiId { get; set; }
        public bool HarvestScheduled { get; set; }
        public DateTime HarvestScheduleTime { get; set; }
        public bool HarvestCollect { get; set; }
        public bool CureCollect { get; set; }
        public int RemoveScheduled { get; set; }
        public DateTime RemoveTime { get; set; }
        public string RemoveReason { get; set; }
        public long IdSerial { get; set; }
        public DateTime Modified { get; set; }
        public long TransactionId_Original { get; set; }

  

    }
}
