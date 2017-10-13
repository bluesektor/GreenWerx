using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTrackTHC.V4
{
//    This contains all active quality assurance lab information (sans phone numbers). 
//This function will only
//return active entries.
    class QA_Lab
    {

        //from openthc locations
        public long Id { get; set; }
        public long OrgiId { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int Deleted { get; set; }
        public int LocationType { get; set; }
        public string LicenseNum { get; set; }
        public long LocationExp { get; set; }
        public long LocationIssue { get; set; }
        public string Status { get; set; }
        public string DistrictCode { get; set; }
        public decimal LocLatitude { get; set; }
        public decimal LocLongitude { get; set; } 
        //public long     LocUbi { get; set; } 
        //public bool   Producer { get; set; } 
        //public bool   Processor { get; set; } 
        //public bool   Retail { get; set; } 
        //public long TransactionId { get; set; }
        //public long TransactionId_Original { get; set; }
        //public long  Fifteenday_End { get; set; } 
        //public DateTime Delete_Time { get; set; } 

        //public bool OrgPublic { get; set; }
        //public bool OrgActive { get; set; }
        //public string OrgLicense { get; set; } 
        //public string Org { get; set; }
        //public long FifteenDayStart { get; set; } 

    }
}
