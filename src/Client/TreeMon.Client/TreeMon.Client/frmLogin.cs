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
using TreeMon.Managers.Membership;
using TreeMon.Models.App;

namespace TreeMon.Client
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.btnLogin.Enabled = false;
            this.txtUsername.Text = "";
            this.txtPassword.Text = "";

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

            UserManager userManager = new UserManager(dbKey, "");
            ServiceResult sr = userManager.AuthenticateUser(this.txtUsername.Text, this.txtPassword.Text, "todogetthisip");

            if (sr.Code != 200)
            {
                MessageBox.Show(sr.Message);
                this.btnLogin.Enabled = true;
                return;
            }
            this.DialogResult = DialogResult.OK;

            this.btnLogin.Enabled = true;

            //todo left off here..
          
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }

      

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                 this.DialogResult = DialogResult.Cancel;
        }
    }
}
