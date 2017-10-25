using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginInterface;
using TreeMon.Models.App;
using TreeMon.Models;
using ClientCore.Extensions;
using TreeMon.Managers.Membership;
using TreeMon.Models.Membership;
using Newtonsoft.Json;
using AutoMapper;
using ClientCore.Controls;
using TreeMon.Utilites.Extensions;

namespace Home
{
    public partial class ctlHome: UserControl, IPlugin
    {
        TreeNode _currentNode = new TreeNode();
        ctlNode _nodeControl;

        public ctlHome()
        {
            InitializeComponent();
        }

        #region Plugin interface properties

        private IPluginHost myPluginHost = null;
        //  private string myPluginName = "Name";
        private string myPluginAuthor = "bluesektor.com";
        private string myPluginDescription = "descriptions";
        private string myPluginVersion = "1.0.0";
        private string myPluginShortName = "Home";

        private UserSession _session;
        private AppInfo _appSettings;

        void PluginInterface.IPlugin.Dispose()
        { }

        public string Description
        { get { return myPluginDescription; } }

        public string Author
        { get { return myPluginAuthor; } }

        public IPluginHost Host
        {
            get { return myPluginHost; }
            set { myPluginHost = value; }
        }

        public void Initialize(UserSession session, AppInfo appSettings)
        {
            _session = session;
            _appSettings = appSettings;

            this.treeAccounts.Nodes.Clear();
            AccountManager am = new AccountManager(this._appSettings.ActiveDbConnectionKey, _session.AuthToken);
            User u = JsonConvert.DeserializeObject<User>(_session.UserData);
            List<Account> accounts = am.GetUsersAccounts(u.UUID);

            LoadTreeViewDelegate delLoadTree = new LoadTreeViewDelegate(this.LoadTreeView);
            delLoadTree(accounts);
            treeAccounts.SelectedNode = this.treeAccounts.Nodes[0];
            _currentNode = treeAccounts.SelectedNode;
            _currentNode.BackColor = Color.LightSteelBlue;

            _nodeControl = new ctlNode(_appSettings.ActiveDbConnectionKey, _session.AuthToken);

            pnlNodeView.Controls.Add(_nodeControl);
            _nodeControl.Show(accounts[0]);

            List<User> users =  am.GetAccountMembers(accounts[0].AccountUUID);
            List<INode> nodes = users.ConvertAll(new Converter<User, INode>(NodeEx.ObjectToNode));
            LoadListViewDelegate delAccounts = new LoadListViewDelegate(LoadListView);
            delAccounts(lstAccountMembers, nodes);
        }

       
        public delegate void LoadListViewDelegate( ListView ctl, List<INode> lst);

        public void LoadListView(ListView ctl, List<INode> nodes)
        {
            if (ctl == null || nodes == null || nodes.Count == 0)
                return;

            if (ctl.InvokeRequired)
            {
                Invoke(new LoadListViewDelegate(LoadListView), ctl, nodes);
            }
            else
            {
                foreach (INode node in nodes)
                {
                    lstAccountMembers.Items.Add(new ListViewItem()
                    {
                         Text = node.Name,
                         Tag = node
                    });
                }
                ctl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); 
            }
        }

        public delegate void LoadTreeViewDelegate(List<Account> nodes);
        public void LoadTreeView(List<Account> nodes)
        {
            if (treeAccounts.InvokeRequired)
            {
                Invoke(new LoadTreeViewDelegate(LoadTreeView), nodes);
            }
            else
            {
                foreach (INode n in nodes)
                {
                    this.treeAccounts.AddNode(n);
                    List<Account> subNodes = nodes.Where(w => w.UUParentID == n.UUID).ToList();
                    LoadTreeView(subNodes);
                }

            }
        }


        protected void Run() //not mandatory, but good for a standard interface to main.
        { }

        public void ResizeControl()
        { }

        public UserControl MainInterface
        { get { return this; } }

        public string Version
        { get { return myPluginVersion; } }

        public string ShortName
        {
            get
            {
                return myPluginShortName;
            }
        }

        #endregion

        private void treeAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (e.Node == null)
                return;

            if (_currentNode == null)
            {
                _currentNode = e.Node;
                _currentNode.BackColor = Color.AliceBlue;
            }

            if (((INode)_currentNode.Tag).UUID != ((INode)e.Node.Tag).UUID) { 
                _currentNode.BackColor = Color.Transparent;
                _currentNode = treeAccounts.SelectedNode;
                _currentNode.BackColor = Color.AliceBlue;
            }

            _nodeControl.Show( (INode) e.Node.Tag );
        }

        private void treeAccounts_Click(object sender, EventArgs e)
        {
           
        }


        //private void trvObjectsClick(object sender, EventArgs e)
        //{
        //    //the clicked node is the previouse node (i.e. just before  the newly clicked node is
        //    //assigned.
        //    //get current from control.
        //    //       ObjectEx curDisplay = this.ctlObjectManager.GetCurrentObject();

        //    //this is an attempt to fix the select bug
        //    //if you select a trv node, then select a property from the control
        //    //then go back and select the same node again, the ctl display still shows the
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
}
