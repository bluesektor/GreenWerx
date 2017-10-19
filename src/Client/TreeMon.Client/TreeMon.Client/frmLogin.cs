using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Client.Utilities;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Membership;

namespace TreeMon.Client
{
    public partial class frmLogin : Form
    {
        private UserSession _userSession;

        public UserSession Session { get { return _userSession; } }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.btnLogin.Enabled = false;
        
            if (string.IsNullOrWhiteSpace( this.txtUsername.Text)) {
                MessageBox.Show("Invalid username.");
                this.btnLogin.Enabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtPassword.Text)){
                MessageBox.Show("Invalid password.");
                this.btnLogin.Enabled = true;
                return;
            }

            string dbKey = string.Empty;

            if(ConfigurationManager.AppSettings["DefaultDbConnection"] != null)
                dbKey = ConfigurationManager.AppSettings["DefaultDbConnection"].ToString();


            string ipAddress = NetworkEx.GetIp();
            UserManager userManager = new UserManager(dbKey, "");
            ServiceResult sr = userManager.AuthenticateUser(this.txtUsername.Text, this.txtPassword.Text,ipAddress);

            if (sr.Code != 200)
            {
                MessageBox.Show(sr.Message);
                this.btnLogin.Enabled = true;
                return;
            }
            User user = (User)sr.Result;
         
            string userJson = JsonConvert.SerializeObject(user);

            SessionManager sm = new SessionManager(dbKey);
            _userSession = sm.SaveSession(ipAddress, user.UUID, userJson, false);

            if (_userSession == null)
            {
                MessageBox.Show("Unable to save session. Check your connections, settings and database.", "Critical Warning", MessageBoxButtons.OK);
                return;
            }
         
            this.DialogResult = DialogResult.OK;
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                 this.DialogResult = DialogResult.Cancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
