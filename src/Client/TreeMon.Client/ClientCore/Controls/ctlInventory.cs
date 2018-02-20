using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Models.Inventory;
using TreeMon.Models;
using ClientCore.Models;
using TreeMon.Managers.Inventory;
using TreeMon.Utilites.Extensions;
using AutoMapper;
using TreeMon.Models.App;
using TreeMon.Managers;
using Newtonsoft.Json;
using TreeMon.Managers.Store;
using TreeMon.Models.Geo;
using TreeMon.Managers.General;

namespace ClientCore.Controls
{
    public partial class ctlInventory : UserControl, IUserControl
    {
        private string _connectionKey;

        private string _sessionKey;

        InventoryItem _selectedItem;
        ctlNodeTree _nodeTree;
        ctlNode _nodeControl;
        ctlItem _itemControl;
       
        List<string> _dataTypes;
        List<string> _locationTypes;
        LocationManager _locationManager;

        public ctlInventory(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;
            AppManager am = new AppManager(this._connectionKey, "forms", this._sessionKey);
            ServiceResult sr = am.DataTypes();

            if (sr.Code != 200)
            {
                lblError.Text = sr.Message;
                return;
            }
            _nodeTree = new ctlNodeTree(_connectionKey, _sessionKey, this);
            _nodeTree.TreeNodesAferSelect += _nodeTree_TreeNodesAferSelect;
            pnlNodeList.Controls.Add(_nodeTree);

            _dataTypes = (List<string>)(sr.Result);

            _locationManager = new LocationManager(_connectionKey, _sessionKey);

        }

        public void Show(string accountUUID)
        {
            _locationTypes = _locationManager.GetLocationTypes(accountUUID);

            InventoryManager am = new InventoryManager(_connectionKey, _sessionKey);
            List<InventoryItem> items = am.GetAccountItems(accountUUID);

           
            List<INode> nodes = new List<INode>();
            if (items.Count > 0)
            {
                nodes = items.ConvertAll(new Converter<InventoryItem, INode>(NodeEx.ObjectToNode));
            }
      
            _nodeControl = new ctlNode(_connectionKey, _sessionKey);
            pnlNodeInventory.Controls.Add(_nodeControl);

            _itemControl = new ctlItem(_connectionKey, _sessionKey);
            pnlItem.Controls.Add(_itemControl); //todo bookmark latest check if works. see ctlProducts

            _nodeTree.Init(nodes, "", null);

            _nodeTree.Dock = DockStyle.Fill;
            _nodeControl.Dock = DockStyle.Fill;

            if (items.Count == 0)
                return;

            _selectedItem = items[0];
            ShowDetails(items[0]);

        }

      
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
        }

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            _selectedItem = GetDetail();
            Save(_selectedItem);

        }

        protected void ShowDetails(InventoryItem item)
        {
            if (item == null)
                return;

            _nodeControl.Show(item);
            _itemControl.Show(item);

            this.cboReferenceType.Items.AddRange(_dataTypes.ToArray());

            if (_selectedItem != null && !string.IsNullOrWhiteSpace(_selectedItem.ReferenceType))
                 cboReferenceType.SelectedIndex = _dataTypes.IndexOf(_dataTypes.Single(w => w == _selectedItem.ReferenceType));

            //// this.cboLocationType.DisplayMember = "Name";
            this.cboLocationType.Items.AddRange(_locationTypes.ToArray());
            if (_selectedItem != null && !string.IsNullOrWhiteSpace(_selectedItem.LocationType))
                cboLocationType.SelectedIndex = _locationTypes.IndexOf(_locationTypes.Single(w => w == _selectedItem.LocationType));

            LocationManager lm = new LocationManager(_connectionKey, _sessionKey);
            Location location = (Location)lm.Get(item.LocationUUID);
            lblLocation.Text = location?.Name;

            txtQuantity.Text =  item.Quantity.ToString();


            lblReferenceType.Text  = item.ReferenceType + ":";//  product, item, user, ballast, plant
            AttributeManager atm = new AttributeManager(_connectionKey, _sessionKey);
            List<TreeMon.Models.General.Attribute> atts = atm.GetAttributes(item.ReferenceUUID, item.ReferenceType, item.AccountUUID);
            if (atts.Count > 0) {
                lblReferenceValue.Text = atts[0].Value;
            }

            chkPublished.Checked =  item.Published; //display in web store.

            lblDateType.Text = item.DateType + ":";//expires, end of cycle ....
            lblItemDate.Text = item.ItemDate.ToString();
 


        }

        protected InventoryItem GetDetail()
        {
            _selectedItem = (InventoryItem)_nodeControl.Get();


            _selectedItem.ReferenceType = cboReferenceType.SelectedText;
            _selectedItem.LocationType = cboLocationType.SelectedText;
            float ftmp;
            if (!float.TryParse(txtQuantity.Text, out ftmp)) {
                this.lblError.Text = "Quantity must be a number.";
                return _selectedItem;
            }

            _selectedItem.Quantity = ftmp;
            _selectedItem.Published = chkPublished.Checked;

            return _selectedItem;
        }

        public void Save(INode n)
        {
            //have to map it to a User object or it'll blow up when inserting to db.
            //may need to move tis to the insert function
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<INode, InventoryItem>();
            });
            if (string.IsNullOrWhiteSpace(n.AccountUUID))
            {
                n.AccountUUID = new SessionManager(_connectionKey).GetAccountUUID(_sessionKey);
            }
            IMapper mapper = config.CreateMapper();
            InventoryItem item = mapper.Map<INode, InventoryItem>(n);

            InventoryManager im = new InventoryManager(this._connectionKey, this._sessionKey);
            ServiceResult res;

            if (string.IsNullOrWhiteSpace(item.UUID))
                res = im.Insert(item);
            else
                res = im.Update(item);

            if (res.Code != 200)
                lblError.Text = res.Message;
  
            _nodeTree.AddToTree(n);
        }

        public void Delete(INode n)
        {
            if (n == null)
                return;

            InventoryManager im = new InventoryManager(this._connectionKey, this._sessionKey);

            im.Delete(n);
            _nodeTree.DeleteFromTree(n);
            List<InventoryItem> subUsers = im.GetItems(n.AccountUUID).Where(x => x.UUParentID == n.UUID).ToList();

            foreach (InventoryItem u in subUsers)
            {
                im.Delete(u);
                _nodeTree.DeleteFromTree(u);
                Delete(u);
            }

        }

        private void pnlNodeList_ClientSizeChanged(object sender, EventArgs e)
        {
            if(_nodeTree !=  null)
                _nodeTree.Dock = DockStyle.Fill;

            if (_nodeControl != null)
                _nodeControl.Dock = DockStyle.Fill;
        }
    }
}
