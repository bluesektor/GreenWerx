using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Labresults_Solvent_Screening
     {
           public long Id  { get; set; }
        public long Sample_Id { get; set; } 
        public string Name { get; set; }
       
        public decimal Value  { get; set; }
        public bool Failure  { get; set; }
        public long Location { get; set; } 
        public long OrgiId { get; set; }
         public bool Lab_Provided { get; set; }

    }
}
