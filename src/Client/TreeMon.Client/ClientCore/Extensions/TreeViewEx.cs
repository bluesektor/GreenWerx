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
        public delegate void LoadTreeViewDelegate(TreeView ctl, List<INode> lst, TreeNode parentNode = null);

        public static void Load(this TreeView ctl, List<INode> nodes, TreeNode parentNode = null)
        {
            if (ctl == null || nodes == null || nodes.Count == 0)
                return;

            if (ctl.InvokeRequired)
            {
                ctl.Invoke(new LoadTreeViewDelegate(Load), ctl, nodes,parentNode);
            }
            else
            {
                foreach (INode node in nodes)
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode.Name = node.Name;
                    treeNode.Text = node.Name;
                    treeNode.Tag = (object)node;

                    if (parentNode != null)
                    {
                        parentNode.Nodes.Add(treeNode);
                        continue;
                    }
                    ctl.Nodes.Add(treeNode);
                    //load the subnodes
                    IEnumerable<INode> subNodes = nodes.Where(w => w.UUParentID == node.UUID);
                    Load(ctl,subNodes.ToList(), treeNode);
                }
            }
        }

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
