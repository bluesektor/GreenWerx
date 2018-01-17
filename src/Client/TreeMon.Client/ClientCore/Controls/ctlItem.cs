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
using TreeMon.Managers;
using TreeMon.Managers.General;
using TreeMon.Models.General;

namespace ClientCore.Controls
{
    public partial class ctlItem : UserControl
    {
        private AccountManager _accountManager;

        private UserManager _userManager;
        UnitOfMeasureManager _uomManager;
        CategoryManager _categoryManager;

        private Item _item;

        private string _connectionKey;

        private string _sessionKey;

        public ctlItem(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();

            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;

            _accountManager = new AccountManager(dbConnectionKey, sessionKey);
            _userManager = new UserManager(dbConnectionKey, sessionKey);

             _uomManager = new UnitOfMeasureManager(dbConnectionKey, sessionKey);

            _categoryManager = new CategoryManager(dbConnectionKey, sessionKey);
        }

        List<UnitOfMeasure> _unitsOfMeasure = new List<UnitOfMeasure>();
        List<Category> _categories = new List<Category>();

        public void Show(INode n)
        {
            if (n == null)
                return;
            
            _item = (Item)n;
            _unitsOfMeasure = _uomManager.GetUnitsOfMeasure(n.AccountUUID);

            chkCustom.Checked = _item.Custom;
            txtCost.Text = _item.Cost.ToString();
            txtCondition.Text = _item.Condition; /// New, used
            txtQuality.Text = _item.Quality;
            txtRating.Text = _item.Rating.ToString();
            chkVirtual.Checked = _item.Virtual;
            txtWeight.Text = _item.Weight.ToString();
            txtDiscount.Text = _item.Discount.ToString();
            txtMarkUp.Text = _item.MarkUp.ToString();
            txtPrice.Text = _item.Price.ToString();
            cboUnitsOfMeasure.DisplayMember = "Name";
            cboUnitsOfMeasure.Items.AddRange(_unitsOfMeasure.ToArray());
            cboUnitsOfMeasure.SelectedIndex = _unitsOfMeasure.IndexOf(_unitsOfMeasure.Single(w => w.UUID == _item.UOMUUID));
            txtSKU.Text = _item.SKU;
            txtExpires.Text = _item.Expires.ToString();

            txtSerialNumber.Text = _item.SerialNumber;
            //how many units were in the product 
            txtUnitsInProduct.Text = _item.UnitsInProduct.ToString();

            //Download,access,trade, jars etc..
            txtUnitType.Text = _item.UnitType;


             _categories=  _categoryManager.GetCategories(n.AccountUUID,false,true);
            cboCategories.DisplayMember = "Name";
            cboCategories.Items.AddRange(_categories.ToArray());
            cboCategories.SelectedIndex = _categories.IndexOf(_categories.Single(w => w.UUID == _item.CategoryUUID));

            /*
             * todo bookmark latest
             * 
             
        public string GroupUUID =  _item.GroupUUID

        public string DepartmentUUID =  _item.DepartmentUUID
        public string Description =  _item.Description
        /// <summary>
        /// percent, numeric, multiplier, function/formula (would have to figure this out first).
        /// </summary>
        public string MarkUpType =  _item.MarkUpType

        //Manufacturer
        [StringLength(32)]
        public string ManufacturerUUID =  _item.ManufacturerUUID

        //Account
        public string ManufacturerUUIDType =  _item.ManufacturerUUIDType

        


    */
            //txtName.Text = _node.Name;
            //txtStatus.Text = _node.Status;
            //txtSortOrder.Text = _node.SortOrder.ToString();
            //txtRoleWeight.Text = _node.RoleWeight.ToString();

            //chkActive.Checked = _node.Active;
            //chkDeleted.Checked = _node.Deleted;
            //chkPrivate.Checked = _node.Private;

            //lblCreatedBy.Text = _userManager.GetBy(_node.CreatedBy)?.Name;
            //lblDateCreated.Text = _node.DateCreated.ToShortDateString();

            //for (int i = 0; i < cboRoleOperation.Items.Count; i++)
            //{
            //    string cboItem = cboRoleOperation.Items[i].ToString();
            //    if (string.IsNullOrEmpty(cboItem))
            //        continue;

            //    if (cboItem.EqualsIgnoreCase(_node.RoleOperation, true))
            //    {
            //        cboRoleOperation.SelectedIndex = i;
            //        break;
            //    }
            //}
            //lblAccount.Text = _accountManager.GetBy(_node.AccountUUID)?.Name;


        }

        //public INode Get()
        //{
        //    if (_node == null)
        //    {
        //        _node = new Node();
        //        //return _node;
        //    }

        //    _node.Name = txtName.Text;
        //    _node.Status = txtStatus.Text;

        //    _node.SortOrder = StringEx.ConvertTo<int>(txtSortOrder.Text);
        //    _node.RoleWeight = StringEx.ConvertTo<int>(txtRoleWeight.Text);

        //    _node.Active = chkActive.Checked;
        //    _node.Deleted = chkDeleted.Checked;
        //    _node.Private = chkPrivate.Checked;

        //    _node.RoleOperation = cboRoleOperation.Text;
        //    return _node;
        //}
    }
}
