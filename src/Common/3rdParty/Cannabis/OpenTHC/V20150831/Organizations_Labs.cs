using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Organizations_Labs
    {
        public long OrgiId { get; set; }
        public bool OrgPublic  { get; set; } 
        public string Name { get; set; } 
        public bool  OrgActive { get; set; } 
        public string  OrgLicense { get; set; } 
    }
}
