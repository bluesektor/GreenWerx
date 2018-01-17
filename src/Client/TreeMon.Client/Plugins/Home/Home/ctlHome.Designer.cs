using PluginInterface;
using System.Windows.Forms;

namespace Home
{
    partial class ctlHome : UserControl, IPlugin
    {
      
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMembers = new System.Windows.Forms.TabPage();
            this.tabLocations = new System.Windows.Forms.TabPage();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.tabProducts = new System.Windows.Forms.TabPage();
            this.pnlNodeView = new System.Windows.Forms.Panel();
            this.pnlNodeList = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Accounts";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabMembers);
            this.tabControl1.Controls.Add(this.tabLocations);
            this.tabControl1.Controls.Add(this.tabInventory);
            this.tabControl1.Controls.Add(this.tabProducts);
            this.tabControl1.Location = new System.Drawing.Point(3, 371);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(821, 320);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabMembers
            // 
            this.tabMembers.BackColor = System.Drawing.Color.Transparent;
            this.tabMembers.Location = new System.Drawing.Point(4, 22);
            this.tabMembers.Name = "tabMembers";
            this.tabMembers.Padding = new System.Windows.Forms.Padding(3);
            this.tabMembers.Size = new System.Drawing.Size(813, 294);
            this.tabMembers.TabIndex = 1;
            this.tabMembers.Text = "Members";
            this.tabMembers.ClientSizeChanged += new System.EventHandler(this.tabMembers_ClientSizeChanged);
            // 
            // tabLocations
            // 
            this.tabLocations.Location = new System.Drawing.Point(4, 22);
            this.tabLocations.Name = "tabLocations";
            this.tabLocations.Size = new System.Drawing.Size(813, 294);
            this.tabLocations.TabIndex = 2;
            this.tabLocations.Text = "Locations";
            this.tabLocations.UseVisualStyleBackColor = true;
            this.tabLocations.ClientSizeChanged += new System.EventHandler(this.tabLocations_ClientSizeChanged);
            // 
            // tabInventory
            // 
            this.tabInventory.Location = new System.Drawing.Point(4, 22);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Size = new System.Drawing.Size(813, 294);
            this.tabInventory.TabIndex = 3;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            this.tabInventory.ClientSizeChanged += new System.EventHandler(this.tabInventory_ClientSizeChanged);
            // 
            // tabProducts
            // 
            this.tabProducts.Location = new System.Drawing.Point(4, 22);
            this.tabProducts.Name = "tabProducts";
            this.tabProducts.Size = new System.Drawing.Size(813, 294);
            this.tabProducts.TabIndex = 4;
            this.tabProducts.Text = "Products";
            this.tabProducts.UseVisualStyleBackColor = true;
            this.tabProducts.ClientSizeChanged += new System.EventHandler(this.tabProducts_ClientSizeChanged);
            // 
            // pnlNodeView
            // 
            this.pnlNodeView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeView.Location = new System.Drawing.Point(174, 19);
            this.pnlNodeView.Name = "pnlNodeView";
            this.pnlNodeView.Size = new System.Drawing.Size(646, 346);
            this.pnlNodeView.TabIndex = 3;
            this.pnlNodeView.ClientSizeChanged += new System.EventHandler(this.pnlNodeView_ClientSizeChanged);
            // 
            // pnlNodeList
            // 
            this.pnlNodeList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeList.Location = new System.Drawing.Point(3, 19);
            this.pnlNodeList.Name = "pnlNodeList";
            this.pnlNodeList.Size = new System.Drawing.Size(165, 346);
            this.pnlNodeList.TabIndex = 62;
            this.pnlNodeList.ClientSizeChanged += new System.EventHandler(this.pnlNodeList_ClientSizeChanged);
            // 
            // ctlHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pnlNodeList);
            this.Controls.Add(this.pnlNodeView);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Name = "ctlHome";
            this.Size = new System.Drawing.Size(827, 694);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       

        #endregion
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabMembers;
        private TabPage tabLocations;
        private TabPage tabInventory;
        private TabPage tabProducts;
        private Panel pnlNodeView;
        private Panel pnlNodeList;
    }
}
