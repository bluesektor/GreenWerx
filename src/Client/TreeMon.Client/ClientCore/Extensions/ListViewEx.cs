using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Models;

namespace ClassLibrary1.Extensions
{
    public static class ListViewEx  
    {
        public delegate void LoadListViewDelegate(ListView ctl, List<INode> lst);

        public static void Load(this ListView ctl, List<INode> nodes)
        {
            if (ctl == null || nodes == null || nodes.Count == 0)
                return;

            if (ctl.InvokeRequired)
            {
               ctl.Invoke(new LoadListViewDelegate(Load), ctl, nodes);
            }
            else
            {
                foreach (INode node in nodes)
                {
                    ctl.Items.Add(new ListViewItem()
                    {
                        Text = node.Name,
                        Tag = node
                    });
                }
                ctl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }
    }
}
