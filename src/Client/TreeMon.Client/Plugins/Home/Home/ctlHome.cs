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
using ClientCore.Models;
using TreeMon.Managers;

namespace Home
{
    public partial class ctlHome: UserControl, IPlugin,IUserControl
    {
        ctlNode _nodeControl;
        ctlNodeTree _nodeTree;
        ctlUser _userControl;
        ctlLocation _locationControl;
        ctlInventory _inventoryControl;
        ctlProduct _productControl;

        public ctlHome()
        {
            InitializeComponent();
        
        }

        #region Plugin interface properties

        private IPluginHost myPluginHost = null;
        //  private string myPluginName = "Name";
        private string myPluginAuthor = "TreeMon.org";
        private string myPluginDescription = "Opensource cannabis system.";
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

            AccountManager am = new AccountManager(this._appSettings.ActiveDbConnectionKey, _session.AuthToken);
            User u = JsonConvert.DeserializeObject<User>(_session.UserData);
            List<Account> accounts = am.GetUsersAccounts(u.UUID);

            List<INode> nodes = accounts.ConvertAll(new Converter<Account, INode>(NodeEx.ObjectToNode));
            _nodeTree = new ctlNodeTree(_appSettings.ActiveDbConnectionKey, _session.AuthToken, this);
            _nodeTree.TreeNodesAferSelect += _nodeTree_TreeNodesAferSelect;
            pnlNodeList.Controls.Add(_nodeTree);

            _nodeTree.Init(nodes, "", null);

           _nodeControl = new ctlNode(_appSettings.ActiveDbConnectionKey, _session.AuthToken);

            pnlNodeView.Controls.Add(_nodeControl);
            _nodeControl.Show(accounts[0]);
          
            _userControl = new ctlUser(_appSettings.ActiveDbConnectionKey, _session.AuthToken);
            tabMembers.Controls.Add(_userControl);

            //todo add controls for location, inventory, products
            _locationControl = new ctlLocation(_appSettings.ActiveDbConnectionKey, _session.AuthToken);
            tabLocations.Controls.Add(_locationControl);

            _inventoryControl = new ctlInventory(_appSettings.ActiveDbConnectionKey, _session.AuthToken);
            tabInventory.Controls.Add(_inventoryControl);

            _productControl = new ctlProduct(_appSettings.ActiveDbConnectionKey, _session.AuthToken);
            tabProducts.Controls.Add(_productControl);

            ShowDetail();
        }

        protected void ShowDetail() {

            if (_nodeTree == null|| _nodeTree.SelectedTreeNode == null || _nodeTree.SelectedTreeNode.Tag == null)
                return;

            string name =  tabControl1.SelectedTab.Name;
            switch (name)
            {
                case "tabMembers"://todo bookmark latest. when updating its still showing users from other account
                    _userControl.Show(((Account)_nodeTree.SelectedTreeNode.Tag).UUID);
                    break;
                case "tabLocations":
                    _locationControl.Show(((Account)_nodeTree.SelectedTreeNode.Tag).UUID);
                    break;
                case "tabInventory":
                    _inventoryControl.Show(((Account)_nodeTree.SelectedTreeNode.Tag).UUID);
                    break;
                case "tabProducts":
                    _productControl.Show(((Account)_nodeTree.SelectedTreeNode.Tag).UUID);
                    break;
                default:
                    _userControl.Show(((Account)_nodeTree.SelectedTreeNode.Tag).UUID);
                    tabControl1.SelectTab("tabMembers");
                    break;
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

       
        private void _nodeTree_TreeNodesAferSelect(object sender, System.EventArgs e)
        {
            if (e == null)
                return;
            TreeViewEventArgs arg = (TreeViewEventArgs)e;

            if (((INode)_nodeTree.SelectedTreeNode.Tag).UUID != ((INode)arg.Node.Tag).UUID)
            {
                _nodeTree.SelectedTreeNode.BackColor = Color.Transparent;
                _nodeTree.SelectedTreeNode.BackColor = Color.AliceBlue;
            }

            _nodeControl.Show((INode)arg.Node.Tag);
            ShowDetail();//propagate to lower windows.
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDetail();
        }

        public void Save(INode n)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<INode, Account>();
            });

            SessionManager sm = new SessionManager(this._appSettings.ActiveDbConnectionKey);
            

            if (string.IsNullOrWhiteSpace(n.AccountUUID))
            {
                n.AccountUUID = sm.GetAccountUUID(this._session.AuthToken);
            }
            IMapper mapper = config.CreateMapper();
            Account account = mapper.Map<INode, Account>(n);
            AccountManager um = new AccountManager(this._appSettings.ActiveDbConnectionKey, this._session.AuthToken);
            ServiceResult res;

            
            if (string.IsNullOrWhiteSpace(account.UUID))
            {

                User u = JsonConvert.DeserializeObject<User>(_session.UserData);
                res = um.Insert(account, false);
                um.AddUserToAccount(account.UUID, u.UUID, u);
            }
            else
                res = um.Update(account);


            if (res.Code != 200)
                MessageBox.Show(res.Message, "Error!");

            _nodeTree.AddToTree(n);
        }

        public void Delete(INode n)
        {
            if (n == null)
                return;

            AccountManager am = new AccountManager(this._appSettings.ActiveDbConnectionKey, this._session.AuthToken);

            am.Delete(n);
            _nodeTree.DeleteFromTree(n);
            List<Account> subAccounts = am.GetAccounts(n.AccountUUID).Where(x => x.UUParentID == n.UUID).ToList();

            foreach (Account a in subAccounts)
            {
                am.Delete(a);
                _nodeTree.DeleteFromTree(a);
                Delete(a);
            }
        }

        private void tabLocations_ClientSizeChanged(object sender, EventArgs e)
        {
            _locationControl.Dock = DockStyle.Fill;
        }

        private void tabInventory_ClientSizeChanged(object sender, EventArgs e)
        {
            _inventoryControl.Dock = DockStyle.Fill;
        }

        private void tabMembers_ClientSizeChanged(object sender, EventArgs e)
        {
            _userControl.Dock = DockStyle.Fill;
        }

        private void tabProducts_ClientSizeChanged(object sender, EventArgs e)
        {
            _productControl.Dock = DockStyle.Fill;
        }

        private void pnlNodeView_ClientSizeChanged(object sender, EventArgs e)
        {
            _nodeControl.Dock = DockStyle.Fill;
        }

        private void pnlNodeList_ClientSizeChanged(object sender, EventArgs e)
        {
            _nodeTree.Dock = DockStyle.Fill;
        }
    }
}
