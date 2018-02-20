using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Models;
using ClientCore.Models;
using TreeMon.Models.Store;
using TreeMon.Managers.Store;
using TreeMon.Utilites.Extensions;
using AutoMapper;
using TreeMon.Managers;
using TreeMon.Models.App;

namespace ClientCore.Controls
{
    public partial class ctlProduct : UserControl, IUserControl
    {
        private string _connectionKey;
        string _currentAccountUUID;
        private string _sessionKey;
        ctlNode _nodeControl;
        ctlNodeTree _nodeTree;
        ctlItem _itemControl;
        Product _selectedProduct;

        public ctlProduct(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;
            _nodeTree = new ctlNodeTree(_connectionKey, _sessionKey, this);
            _nodeTree.TreeNodesAferSelect += _nodeTree_TreeNodesAferSelect;
            pnlNodeList.Controls.Add(_nodeTree);

        }

        public void Show(string accountUUID)
        {
            _currentAccountUUID = accountUUID;
            ProductManager pm = new ProductManager(_connectionKey, _sessionKey);
            List<Product> products = pm.GetAccountProducts(accountUUID);

            List<INode> nodes = new List<INode>();
            if (products.Count > 0)
            {
                nodes = products.ConvertAll(new Converter<Product, INode>(NodeEx.ObjectToNode));
            }


            _nodeTree.Init(nodes, "", null);

                _nodeControl = new ctlNode(_connectionKey, _sessionKey);
            pnlNodeProduct.Controls.Add(_nodeControl);

            _itemControl = new ctlItem(_connectionKey, _sessionKey);
            pnlItem.Controls.Add(_itemControl);

            _nodeTree.Dock = DockStyle.Fill;
            _nodeControl.Dock = DockStyle.Fill;

            if (products.Count == 0)
                return;

            _selectedProduct = products[0];
            ShowDetails(products[0]);

        }

        protected void ShowDetails(Product p)
        {
            if (p == null)
                return;

            _nodeControl.Show(p);
            _itemControl.Show(p);
        }

        public void Save(INode n)
        {
            //have to map it to a User object or it'll blow up when inserting to db.
            //may need to move tis to the insert function
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<INode, Product>();
            });
            if (string.IsNullOrWhiteSpace(n.AccountUUID))
            {
                n.AccountUUID = new SessionManager(_connectionKey).GetAccountUUID(_sessionKey);
            }
            IMapper mapper = config.CreateMapper();
            Product item = mapper.Map<INode, Product>(n);

            ProductManager im = new ProductManager(this._connectionKey, this._sessionKey);
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

            ProductManager pm = new ProductManager(_connectionKey, _sessionKey);

            pm.Delete(n);
            _nodeTree.DeleteFromTree(n);
            List<Product> subProducts= pm.GetProducts(n.AccountUUID).Where(x => x.UUParentID == n.UUID).ToList();

            foreach (Product p in subProducts)
            {
                pm.Delete(p);
                _nodeTree.DeleteFromTree(p);
                Delete(p);
            }
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

        private void pnlNodeList_ClientSizeChanged(object sender, EventArgs e)
        {
            if (_nodeTree != null)
                _nodeTree.Dock = DockStyle.Fill;

            if (_nodeControl != null)
                _nodeControl.Dock = DockStyle.Fill;

            if (_itemControl != null)
                _itemControl.Dock = DockStyle.Fill;
        }
    }
}
