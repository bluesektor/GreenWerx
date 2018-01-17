using ClientCore.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeMon.Managers;
using TreeMon.Models;

namespace ClientCore.Dialogs
{
    public partial class dlgNode : Form
    {
        private string _connectionKey;

        private string _sessionKey;
        ctlNode _nodeControl;
       
        public INode Get()
        {
            INode n = _nodeControl.Get();
            n.AccountUUID = new SessionManager(_connectionKey).GetAccountUUID(_sessionKey);
            return n;
        }

        public dlgNode( string dbConnectionKey, string sessionKey)
        {
           
            InitializeComponent();
            _connectionKey = dbConnectionKey;
            _sessionKey = sessionKey;

            _nodeControl = new ctlNode(_connectionKey, _sessionKey);
            pnlNodeUser.Controls.Add(_nodeControl);
           

        }

        //Save the object in the parent window, call Get to return the object.
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
          
        }
    }
}
