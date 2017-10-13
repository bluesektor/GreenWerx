using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Organizations
     {
       public string Org { get; set; } 
        public string Name { get; set; } 
        public long OrgiId { get; set; } 
      public bool  OrgActive { get; set; } 
       public string  OrgLicense { get; set; } 
       public long   FifteenDayStart { get; set; } 
    }
}
