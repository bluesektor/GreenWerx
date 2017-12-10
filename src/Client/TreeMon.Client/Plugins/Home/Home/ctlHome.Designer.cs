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
            this.treeAccounts = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMembers = new System.Windows.Forms.TabPage();
            this.tabLocations = new System.Windows.Forms.TabPage();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.pnlNodeInventoryItem = new System.Windows.Forms.Panel();
            this.tabProducts = new System.Windows.Forms.TabPage();
            this.pnlNodeProduct = new System.Windows.Forms.Panel();
            this.pnlNodeView = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabInventory.SuspendLayout();
            this.tabProducts.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeAccounts
            // 
            this.treeAccounts.Location = new System.Drawing.Point(3, 19);
            this.treeAccounts.Name = "treeAccounts";
            this.treeAccounts.ShowRootLines = false;
            this.treeAccounts.Size = new System.Drawing.Size(121, 346);
            this.treeAccounts.TabIndex = 0;
            this.treeAccounts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeAccounts_AfterSelect);
            this.treeAccounts.Click += new System.EventHandler(this.treeAccounts_Click);
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
            this.tabControl1.Size = new System.Drawing.Size(746, 320);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabMembers
            // 
            this.tabMembers.Location = new System.Drawing.Point(4, 22);
            this.tabMembers.Name = "tabMembers";
            this.tabMembers.Padding = new System.Windows.Forms.Padding(3);
            this.tabMembers.Size = new System.Drawing.Size(738, 294);
            this.tabMembers.TabIndex = 1;
            this.tabMembers.Text = "Members";
            this.tabMembers.UseVisualStyleBackColor = true;
            // 
            // tabLocations
            // 
            this.tabLocations.Location = new System.Drawing.Point(4, 22);
            this.tabLocations.Name = "tabLocations";
            this.tabLocations.Size = new System.Drawing.Size(738, 294);
            this.tabLocations.TabIndex = 2;
            this.tabLocations.Text = "Locations";
            this.tabLocations.UseVisualStyleBackColor = true;
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.pnlNodeInventoryItem);
            this.tabInventory.Location = new System.Drawing.Point(4, 22);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Size = new System.Drawing.Size(738, 294);
            this.tabInventory.TabIndex = 3;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            // 
            // pnlNodeInventoryItem
            // 
            this.pnlNodeInventoryItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeInventoryItem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeInventoryItem.Location = new System.Drawing.Point(228, 7);
            this.pnlNodeInventoryItem.Name = "pnlNodeInventoryItem";
            this.pnlNodeInventoryItem.Size = new System.Drawing.Size(282, 281);
            this.pnlNodeInventoryItem.TabIndex = 17;
            // 
            // tabProducts
            // 
            this.tabProducts.Controls.Add(this.pnlNodeProduct);
            this.tabProducts.Location = new System.Drawing.Point(4, 22);
            this.tabProducts.Name = "tabProducts";
            this.tabProducts.Size = new System.Drawing.Size(738, 294);
            this.tabProducts.TabIndex = 4;
            this.tabProducts.Text = "Products";
            this.tabProducts.UseVisualStyleBackColor = true;
            // 
            // pnlNodeProduct
            // 
            this.pnlNodeProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeProduct.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeProduct.Location = new System.Drawing.Point(228, 7);
            this.pnlNodeProduct.Name = "pnlNodeProduct";
            this.pnlNodeProduct.Size = new System.Drawing.Size(282, 281);
            this.pnlNodeProduct.TabIndex = 17;
            // 
            // pnlNodeView
            // 
            this.pnlNodeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeView.Location = new System.Drawing.Point(130, 19);
            this.pnlNodeView.Name = "pnlNodeView";
            this.pnlNodeView.Size = new System.Drawing.Size(619, 346);
            this.pnlNodeView.TabIndex = 3;
            // 
            // ctlHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlNodeView);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeAccounts);
            this.Name = "ctlHome";
            this.Size = new System.Drawing.Size(752, 694);
            this.tabControl1.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tabProducts.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeAccounts;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabMembers;
        private TabPage tabLocations;
        private TabPage tabInventory;
        private TabPage tabProducts;
        private Panel pnlNodeView;
        private Panel pnlNodeInventoryItem;
        private Panel pnlNodeProduct;
    }
}
