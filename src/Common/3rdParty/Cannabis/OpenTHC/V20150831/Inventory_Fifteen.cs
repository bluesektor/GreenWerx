﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTHC.V20150831
 {
    public class Inventory_Fifteen
     {
        public long Id  { get; set; }
        public long InventoryId  { get; set; }
        public long OrgiId { get; set; } 
        public long Location { get; set; } 
        public int Deleted { get; set; }
        public DateTime SessionTime { get; set; }
        public decimal Quantity { get; set; }
        public string InventoryType { get; set; }
        public string Strain  { get; set; }
    }
}