namespace ClientCore.Controls
{
    partial class ctlProduct
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
            this.pnlNodeList = new System.Windows.Forms.Panel();
            this.pnlNodeProduct = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.pnlItem = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlNodeList
            // 
            this.pnlNodeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeList.BackColor = System.Drawing.SystemColors.Control;
            this.pnlNodeList.Location = new System.Drawing.Point(3, 4);
            this.pnlNodeList.Name = "pnlNodeList";
            this.pnlNodeList.Size = new System.Drawing.Size(180, 294);
            this.pnlNodeList.TabIndex = 65;
            this.pnlNodeList.ClientSizeChanged += new System.EventHandler(this.pnlNodeList_ClientSizeChanged);
            // 
            // pnlNodeProduct
            // 
            this.pnlNodeProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeProduct.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeProduct.BackColor = System.Drawing.SystemColors.Control;
            this.pnlNodeProduct.Location = new System.Drawing.Point(189, 7);
            this.pnlNodeProduct.Name = "pnlNodeProduct";
            this.pnlNodeProduct.Size = new System.Drawing.Size(428, 291);
            this.pnlNodeProduct.TabIndex = 64;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.SystemColors.Control;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(6, 301);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 66;
            // 
            // pnlItem
            // 
            this.pnlItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlItem.BackColor = System.Drawing.SystemColors.Control;
            this.pnlItem.Location = new System.Drawing.Point(623, 7);
            this.pnlItem.Name = "pnlItem";
            this.pnlItem.Size = new System.Drawing.Size(377, 292);
            this.pnlItem.TabIndex = 67;
            // 
            // ctlProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlItem);
            this.Controls.Add(this.pnlNodeList);
            this.Controls.Add(this.pnlNodeProduct);
            this.Controls.Add(this.lblError);
            this.Name = "ctlProduct";
            this.Size = new System.Drawing.Size(1005, 318);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlNodeList;
        private System.Windows.Forms.Panel pnlNodeProduct;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Panel pnlItem;
    }
}
