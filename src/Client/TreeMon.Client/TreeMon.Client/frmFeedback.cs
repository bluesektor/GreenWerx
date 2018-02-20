using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeMon.Client
{
    public partial class frmFeedback : Form
    {
        private System.Windows.Forms.Button butOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblPluginAuthor;
        private System.Windows.Forms.Label lblPluginVersion;
        private System.Windows.Forms.Label lblPluginName;
        private System.Windows.Forms.Label lblPluginDesc;
        private System.Windows.Forms.Label lblFeedback;

         public frmFeedback()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

     
        }

        private void butOk_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public string PluginAuthor
        {
            get { return this.lblPluginAuthor.Text; }
            set { this.lblPluginAuthor.Text = value; }
        }

        public string PluginDesc
        {
            get { return this.lblPluginDesc.Text; }
            set { this.lblPluginDesc.Text = value; }
        }

        public string PluginName
        {
            get { return this.lblPluginName.Text; }
            set { this.lblPluginName.Text = value; }
        }

        public string PluginVersion
        {
            get { return this.lblPluginVersion.Text; }
            set { this.lblPluginVersion.Text = value; }
        }
        public string Feedback
        {
            get { return this.lblFeedback.Text; }
            set { this.lblFeedback.Text = value; }
        }


    }
}
