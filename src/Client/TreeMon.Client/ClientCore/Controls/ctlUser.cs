using AutoMapper;
using ClientCore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;

namespace ClientCore.Controls
{
    public partial class ctlUser : UserControl, IUserControl
    {
        private string _connectionKey;

        private string _sessionKey;

        ctlNode _nodeControl;

        ctlNodeTree _nodeTree;

        private UserManager _userManager;

        User _selectedUser;

        public ctlUser(string dbConnectionKey, string sessionKey)
        {
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;
         
            _userManager = new UserManager(dbConnectionKey, sessionKey);
            _nodeTree = new ctlNodeTree(_connectionKey, _sessionKey, this);
            _nodeTree.TreeNodesAferSelect += _nodeTree_TreeNodesAferSelect;
            pnlNodeList.Controls.Add(_nodeTree);

            _nodeControl = new ctlNode(_connectionKey, _sessionKey);
            pnlNodeUser.Controls.Add(_nodeControl);
        }

        //AccountUUID is passed in because the home control displays accounts.
        //As an account is selected we'll show the users for that account..
        //
        public void Show( string accountUUID )
        {
            AccountManager am = new AccountManager(_connectionKey, _sessionKey);
            List<User> users = am.GetAccountMembers(accountUUID, false);

            List<INode> nodes = new List<INode>();
            if (users.Count > 0)
            {
                  nodes = users.ConvertAll(new Converter<User, INode>(NodeEx.ObjectToNode));
            }

            _nodeTree.Init(nodes, "");

            _nodeTree.Dock = DockStyle.Fill;
            _nodeControl.Dock = DockStyle.Fill;

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

        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            _selectedUser = GetUserDetail();
            Save(_selectedUser);
         
        }

        public void Save(INode n)
        {
            //have to map it to a User object or it'll blow up when inserting to db.
            //may need to move tis to the insert function
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<INode, User>();
            });
            if (string.IsNullOrWhiteSpace(n.AccountUUID))
            {
                n.AccountUUID =  new SessionManager(_connectionKey).GetAccountUUID(_sessionKey);
            }
            IMapper mapper = config.CreateMapper();
            User user = mapper.Map<INode, User>(n);

            UserManager um = new UserManager(this._connectionKey, this._sessionKey);
            ServiceResult res;

            if (string.IsNullOrWhiteSpace(user.UUID))
                res = um.Insert(user );
            else
                res = um.Update(user);

            if (res.Code != 200)
                lblError.Text = res.Message;

            SessionManager sm = new SessionManager(_connectionKey);
            UserSession us =   sm.GetSession(_sessionKey);

            AccountManager am = new AccountManager(_connectionKey, _sessionKey);
            am.AddUserToAccount(user.AccountUUID, user.UUID, JsonConvert.DeserializeObject<User>(us.UserData));
            _nodeTree.AddToTree(n);
        }

        public void Delete(INode n)
        {
            if (n == null)
                return;

            UserManager um = new UserManager(this._connectionKey, this._sessionKey);
            AccountManager am = new AccountManager(_connectionKey, _sessionKey);

            um.Delete(n);
            am.RemoveUserFromAccount(n.AccountUUID, n.UUID);
            _nodeTree.DeleteFromTree(n);
            List<User> subUsers = um.GetUsers(n.AccountUUID).Where(x => x.UUParentID == n.UUID).ToList();

            foreach (User u in subUsers)
            {
                um.Delete(u);
                am.RemoveUserFromAccount(u.AccountUUID, u.UUID);
                _nodeTree.DeleteFromTree(u);
                Delete(u);
            }
           
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

        private void pnlNodeList_ClientSizeChanged(object sender, EventArgs e)
        {
            _nodeTree.Dock = DockStyle.Fill;
            _nodeControl.Dock = DockStyle.Fill;
        }
    }
}
