using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Models;
using TreeMon.Utilites.Extensions;

namespace ClientCore.Extensions
{
    public static class TreeViewEx
    {
        public static TreeNode AddNode(this TreeView tree, INode node)
        {
            if (node == null)
                return null;

            TreeNode mainNode = new TreeNode();
            mainNode.Name = node.Name;
            mainNode.Text = node.Name;
            mainNode.Tag = (object)node;
            tree.Nodes.Add(mainNode);
            return mainNode;
        }

        public static TreeNode GetNode(string nodeName, TreeNodeCollection nodes)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Name.EqualsIgnoreCase(nodeName))
                    return n;
                else
                    return GetNode(nodeName, n.Nodes);
            }

            return null;
        }

        

    }

    
}
