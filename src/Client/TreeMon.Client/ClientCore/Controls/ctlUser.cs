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
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using ClassLibrary1.Extensions;
using TreeMon.Models;
using TreeMon.Utilites.Extensions;

namespace ClientCore.Controls
{
    public partial class ctlUser : UserControl
    {
        private string _connectionKey;

        private string _sessionKey;

        ctlNode _nodeControl;

        private UserManager _userManager;

        User _selectedUser;

        public ctlUser(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;
         
            _userManager = new UserManager(dbConnectionKey, sessionKey);
        }
     
        //AccountUUID is passed in because the home control displays accounts.
        //As an account is selected we'll show the users for that account..
        //
        public void Show( string accountUUID )
        {
            lstAccountMembers.Items.Clear();

            AccountManager am = new AccountManager(_connectionKey, _sessionKey);
            List<User> users =  am.GetAccountMembers(accountUUID, false);

            if (users.Count == 0)
                return;


            List<INode> nodes = users.ConvertAll(new Converter<User, INode>(NodeEx.ObjectToNode));
            lstAccountMembers.Load(nodes);

            _nodeControl = new ctlNode(_connectionKey, _sessionKey);

            pnlNodeUser.Controls.Add(_nodeControl);

            _selectedUser = users[0];
            ShowDetails(users[0]);

        }

        protected void ShowDetails(User u) {
            if (u == null)
                return;

            _nodeControl.Show(u);
            lblLastLockoutDate.Text = u.LastLockoutDate.ToString();
            lblLastLoginDate.Text = u.LastLoginDate.ToString();
            lblLastPasswordChangedDate.Text = u.LastPasswordChangedDate.ToString();
            chkAnonymous.Checked = u.Anonymous;
            chkApproved.Checked = u.Approved;
            chkBanned.Checked = u.Banned;
            chkSiteAdmin.Checked = u.SiteAdmin;
            chkLockedOut.Checked = u.LockedOut;
            txtEmail.Text = u.Email;
            txtLicenseNumber.Text = u.LicenseNumber;
        }

        private void lstAccountMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstAccountMembers.Items.Count <= 0 ||
             this.lstAccountMembers.SelectedItems.Count <= 0)
                return;

            User u = (User)this.lstAccountMembers.SelectedItems[0].Tag;//[lstProperties.SelectedItems[0].Index].Tag;

            ShowDetails(u);
        }

        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (this.lstAccountMembers.SelectedItems.Count > 0)
                _selectedUser = (User)this.lstAccountMembers.SelectedItems[0].Tag;

            if (_selectedUser == null)
                return;

            _selectedUser = GetUserDetail();
   
            UserManager um = new UserManager(this._connectionKey, this._sessionKey);
            ServiceResult res;

            if (string.IsNullOrWhiteSpace(_selectedUser.UUID))
               res= um.Insert(_selectedUser);
            else
              res =  um.Update(_selectedUser);

            if (res.Code != 200)
                lblError.Text = res.Message;
        }

        protected User GetUserDetail()
        {
            _selectedUser = (User)_nodeControl.Get();
            _selectedUser.Email = txtEmail.Text;
            _selectedUser.Anonymous= chkAnonymous.Checked;
            _selectedUser.Approved= chkApproved.Checked;
            _selectedUser.Banned= chkBanned.Checked;
            _selectedUser.SiteAdmin=chkSiteAdmin.Checked;
            _selectedUser.LockedOut =chkLockedOut.Checked;
            _selectedUser.LicenseNumber = txtLicenseNumber.Text;
            return _selectedUser;
        }
    }
}
