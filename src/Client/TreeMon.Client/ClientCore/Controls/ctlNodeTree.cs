using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Managers.Membership;
using TreeMon.Models;
using ClientCore.Extensions;
using ClientCore.Controls;
using ClientCore.Dialogs;
using ClientCore.Models;

namespace ClientCore.Controls
{
    public partial class ctlNodeTree : UserControl
    {
        private AccountManager _accountManager;

        private UserManager _userManager;

        public INode SelectedNode { get; set; }

        public TreeNode SelectedTreeNode { get { return treeNodes.SelectedNode; } set { treeNodes.SelectedNode = value; } }

        private string _connectionKey;

        private string _sessionKey;

        private IUserControl _parent;

        public ctlNodeTree(string dbConnectionKey, string sessionKey,  IUserControl parent)
        {
            InitializeComponent();
            _parent = parent;
            if (_parent == null)
            {
                MessageBox.Show("Parent window parameter is empty.", "Error!", MessageBoxButtons.OK);
                return;
            }
            // _selectedNode = selectedNode;
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;

            _accountManager = new AccountManager(dbConnectionKey, sessionKey);
            _userManager = new UserManager(dbConnectionKey, sessionKey);

         
        }

        public void Clear()
        {
            treeNodes.Nodes.Clear();
        }

        public void Init(List<INode> nodes, string parentUUID, TreeNode parentNode = null)
        {
            this.Clear();
            treeNodes.Load(nodes, parentUUID, parentNode);
        }



        public void AddToTree(INode node)
        {
            SelectedTreeNode = treeNodes.AddNode(node);
        }

        public void Update(INode node)
        {
            TreeNode[] foundNodes = TreeViewEx.FindNodes(treeNodes.TopNode.Nodes,node.UUID);

            foreach (TreeNode n in foundNodes)
            {
                n.Name = node.UUID;
                n.Text = node.Name;
                n.Tag = (object)node;
            }
        }

        public void DeleteFromTree(INode node)
        {
            treeNodes.DeleteNode(node);
        }

        private string GetAccountUUID()
        {
            if (SelectedNode != null)
                return SelectedNode.AccountUUID;

            if (treeNodes.Nodes.Count > 0)
                return ((INode)treeNodes.Nodes[0].Tag).AccountUUID;

            return string.Empty;
        }

        private void btnAddSubNode_Click(object sender, EventArgs e)
        {
            dlgNode dlgNewNode = new dlgNode(_connectionKey, _sessionKey);
           
            if (dlgNewNode.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            INode n = dlgNewNode.Get();

            if (n == null)
                return;

            n.UUID = "";
            n.AccountUUID = GetAccountUUID();
            n.UUParentID = SelectedNode?.UUID;
            n.UUParentIDType = SelectedNode?.UUIDType;
            n.ParentId = SelectedNode?.ParentId;
            _parent.Save(n);
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            dlgNode dlgNewNode = new dlgNode(_connectionKey, _sessionKey);

            if (dlgNewNode.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            INode n = dlgNewNode.Get();

            if (n == null)
                return;

            n.UUID = "";

            if (string.IsNullOrWhiteSpace(n.AccountUUID))
                n.AccountUUID = SelectedNode?.AccountUUID;

            _parent.Save(n);
        
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null)
            {
                MessageBox.Show("You must select a node.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this node and sub-nodes?") != DialogResult.OK)
            {
                return;
            }
            _parent.Delete(SelectedNode);
         
        }

        public event EventHandler TreeNodesAferSelect;
        protected virtual void OnTreeNodeAfterSelect(  TreeViewEventArgs e)
        {
            var handler = TreeNodesAferSelect;
            if (handler != null)
                handler(this, e);

        }

     

        #region event to parent

    ////    Raise event from Child:
    ////    public event EventHandler CloseButtonClicked;
    ////    protected virtual void OnCloseButtonClicked(EventArgs e)
    ////    {
    ////        var handler = CloseButtonClicked;
    ////        if (handler != null)
    ////            handler(this, e);
    ////    }
    ////    private void CloseButton_Click(object sender, EventArgs e)
    ////    {
    ////        //While you can call `this.ParentForm.Close()` it's better to raise an event
    ////        OnCloseButtonClicked(e);
    ////    }

    ////    Subscribe and use event in Parent:
    ////    Subscribe for event using designer or in form load
    ////    this.userControl11.CloseButtonClicked += userControl11_CloseButtonClicked;

    ////    //Close the form when you received the notification
    ////private void userControl11_CloseButtonClicked(object sender, EventArgs e)
    ////{
    ////    this.Close();
    ////}
    #endregion



    private void treeNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {

            //todo turn current node backcolor to default then update selected
            // _treeLocations.SelectedTreeNode.BackColor = Color.Transparent; //todo may need to see if the previouse selected node need to be set to transparent.
            this.treeNodes.SelectedNode.BackColor = Color.LightGreen;

            SelectedTreeNode = e.Node;
            SelectedNode = (INode)e.Node.Tag;
            OnTreeNodeAfterSelect(e);
        }

        private void treeNodes_Click(object sender, EventArgs e)
        {
            var tree = (TreeView)sender;
            SelectedTreeNode = tree.SelectedNode;
        }
    }

    //private void trvObjectsClick(object sender, EventArgs e)
    //{
    //    //the clicked node is the previouse node (i.e. just before  the newly clicked node is
    //    //assigned.
    //    //get current from control.
    //    //       ObjectEx curDisplay = this.ctlObjectManager.GetCurrentObject();

    //    //this is an attempt to fix the select bug
    //    //if you select a trv node, then select a property from the control
    //    //then go back and select the same node again, the ctl display still shows the4
    //    //property data.
    //    if (_currentNode.Tag == null)
    //        return;

    //    ObjectEx curNode = (ObjectEx)_currentNode.Tag;


    //    //if (curDisplay == null || curNode == null)
    //    //    return;

    //    //if (curNode.ID == curDisplay.ID && this.ctlObjectManager.ShowClass == "property")
    //    //{
    //    //    DisplayNode(_currentNode);
    //    //}

    //}


    //private TreeNode CreateRootNode()
    //{
    //ObjectEx root = new ObjectEx("root");
    //root.Title = "ROOT";
    //root.ID = NumericsHelper.GenerateId();
    //TreeNode mainNode = new TreeNode();
    //mainNode.Name = "ROOT";
    //mainNode.Text = "Profiles";
    //mainNode.Tag = (object)root;
    //this.trvProfilesData.Nodes.Add(mainNode);
    //return mainNode;
    //}

    //private TreeNode CreateRootNode(string projectName)
    //{
    //    ObjectEx root = new ObjectEx("root");
    //    root.Title = "ROOT";
    //    root.ID = NumericsHelper.GenerateId();
    //    TreeNode mainNode = new TreeNode();
    //    mainNode.Name = "ROOT";
    //    mainNode.Text = projectName;
    //    mainNode.Tag = (object)root;
    //    this.trvProfilesData.Nodes.Add(mainNode);
    //    return mainNode;
    //}


    //private void AddNodes(ObjectEx p, ref TreeNode parentNode)
    //{
    //    if (p == null)
    //        return;

    //    TreeNode node = new TreeNode();
    //    node.Text = p.Title;
    //    node.Name = p.Title;
    //    node.Tag = (object)p;
    //    parentNode.Nodes.Add(node);

    //    foreach (ObjectEx property in p.Nodes)
    //    {
    //        AddNode(property, ref node);
    //    }
    //}

    ////recursively add nodes...
    ////
    //private void AddNode(ObjectEx properties, ref TreeNode parentNode)
    //{
    //    if (properties == null)
    //        return;

    //    TreeNode node = new TreeNode();
    //    node.Text = properties.Title;
    //    node.Name = properties.Title;
    //    node.Tag = (object)properties;
    //    parentNode.Nodes.Add(node);

    //    if (properties.Nodes.Count > 0)
    //    {
    //        foreach (ObjectEx property in properties.Nodes)
    //        {
    //            AddNode(property, ref node);
    //        }
    //    }
    //}

    //private ObjectEx GetProfileForNode(TreeNode node)
    //{
    //    if (string.IsNullOrEmpty(node.Text))
    //    {
    //        //MessageBox.Show("You must select a node.");
    //        return null;
    //    }

    //    ObjectEx cur = (ObjectEx)node.Tag;
    //    if (cur.ParentID == -1 || node.Parent == null)
    //        return cur;

    //    ObjectEx parent = (ObjectEx)node.Parent.Tag;
    //    if (parent == null)
    //        return cur;

    //    if (cur.ParentID == parent.ID && parent.ParentID == -1)
    //        return parent;
    //    else
    //        return GetProfileForNode(node.Parent);//recurse up the tree.
    //}
}
