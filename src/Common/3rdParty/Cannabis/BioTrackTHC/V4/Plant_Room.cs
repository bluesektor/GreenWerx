using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
    //This  contains  plant  room  information  as
    //previously  submitted.  It  is  license 
    //specific and can be queried with all records or only active ones.
    //
    public class Plant_Room
    {
        public string Name { get; set; }//	variable length text field
        public string Location{ get; set; }//	license number of location value
        public int Id	{ get; set; }//integer value
        //from openthc locations
        //  public long Id  { get; set; }
        //public long OrgiId { get; set; } 
        //public long LocationId { get; set; }
        //public string Name { get; set; } 
        //public string Address1 { get; set; } 
        //public string Address2 { get; set; } 
        //public string City { get; set; } 
        //public string State { get; set; } 
        //public string Zip { get; set; } 
        //public int Deleted { get; set; }
        //public int LocationType { get; set; } 
        //public string LicenseNum { get; set; } 
        //public long     LocationExp { get; set; } 
        //public long    LocationIssue { get; set; } 
        //public string   Status { get; set; } 
        //public string  DistrictCode { get; set; } 
        //public decimal    LocLatitude { get; set; }
        //public decimal   LocLongitude { get; set; } 
        //public long     LocUbi { get; set; } 
        //public bool   Producer { get; set; } 
        //public bool   Processor { get; set; } 
        //public bool   Retail { get; set; } 
        //public long TransactionId { get; set; }
        //public long TransactionId_Original { get; set; }
        //public long  Fifteenday_End { get; set; } 
        //public DateTime Delete_Time { get; set; } 
      
      
    
    }
}
