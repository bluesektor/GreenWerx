using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            this.items = new List<MenuItem>();
        }

        public string label { get; set; }

        public string icon { get; set; }

        public string type { get; set; }

        public int SortOrder { get; set; }

        public List<MenuItem> items { get; set; }
    }
}
