using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Locations_Labs
     {
     public long Id  { get; set; }
        public long OrgiId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; } 
        public string Address2 { get; set; } 
        public string City { get; set; } 
        public string State { get; set; } 
        public string Zip { get; set; } 
        public int Deleted { get; set; } 
        public string LicenseNum { get; set; } 
          public long     LocationExp { get; set; } 
        public long    LocationIssue { get; set; } 
         public string   Status { get; set; } 
         public string  DistrictCode { get; set; }
          public long     LocUbi { get; set; } 
         public long TransactionId  { get; set; }
        public long TransactionId_Original { get; set; }
    }
}
