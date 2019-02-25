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
        public delegate void LoadTreeViewDelegate(TreeView ctl, List<INode> lst, string parentUUID, TreeNode parentNode = null );

        public static void Load(this TreeView ctl, List<INode> nodes,string parentUUID, TreeNode parentNode = null )
        {
            if (ctl == null || nodes == null || nodes.Count == 0)
                return;

            if (ctl.InvokeRequired)
            {
                ctl.Invoke(new LoadTreeViewDelegate(Load), ctl, nodes, parentUUID, parentNode);
            }
            else
            {
                var tmpNodes = nodes.Where(x => x.UUParentID == parentUUID);

                foreach (INode node in tmpNodes)
                {
                    if (string.IsNullOrWhiteSpace(node.Name))
                        continue;

                    TreeNode treeNode = new TreeNode();
                    treeNode.Name = node.UUID;
                    treeNode.Text = node.Name;
                    treeNode.Tag = (object)node;

                    if (parentNode != null)
                    {
                        parentNode.Nodes.Add(treeNode);
                    }
                    else
                    {
                        ctl.Nodes.Add(treeNode);
                    }
                    //load the subnodes
                    Load(ctl, nodes, node.UUID, treeNode);
                }
               
            }
        }

        public static TreeNode AddNode(this TreeView tree, INode node)
        {
            if (node == null)
                return null;

            if (string.IsNullOrWhiteSpace(node.UUParentID)){
                TreeNode mainNode = new TreeNode();
                mainNode.Name = node.UUID;
                mainNode.Text = node.Name;
                mainNode.Tag = (object)node;
          
                tree.Nodes.Add(mainNode);
                return mainNode;
            } else {
                //find parent
              TreeNode[] nodes =  tree.Nodes.Find(node.UUParentID, true);

                if (nodes.Length == 0)
                    return null;

                TreeNode subNode = new TreeNode();
                subNode.Name = node.UUID;
                subNode.Text = node.Name;
                subNode.Tag = (object)node;

                nodes[0].Nodes.Add(subNode);
                return subNode;
            }
        }

        public static void DeleteNode(this TreeView tree, INode node)
        {
            if (node == null)
                return;

            TreeNode[] nodes = TreeViewEx.FindNodes(tree.Nodes,node.UUID);

            if (nodes.Length > 0)
            {
                tree.Nodes.Remove(nodes[0]);
            }
        
        }

        public static TreeNode[] FindNodes(TreeNodeCollection nodeCollection, string uuid ) {
            List<TreeNode> nodes = new List<TreeNode>();

            foreach (TreeNode  n in nodeCollection) {

                if (n.Name == uuid)
                {
                    nodes.Add(n);
                }

                var tmpNodes = FindNodes(n.Nodes, uuid);
                if (tmpNodes.Count() > 0)
                    nodes.AddRange(tmpNodes);
            }

            return nodes.ToArray();
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
