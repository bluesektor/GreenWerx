namespace ClientCore.Controls
{
    partial class ctlInventory
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
            this.pnlNodeInventory = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboReferenceType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cboLocationType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblReferenceType = new System.Windows.Forms.Label();
            this.chkPublished = new System.Windows.Forms.CheckBox();
            this.lblDateType = new System.Windows.Forms.Label();
            this.lblItemDate = new System.Windows.Forms.Label();
            this.lblReferenceValue = new System.Windows.Forms.Label();
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
            this.pnlNodeList.Size = new System.Drawing.Size(143, 294);
            this.pnlNodeList.TabIndex = 63;
            this.pnlNodeList.ClientSizeChanged += new System.EventHandler(this.pnlNodeList_ClientSizeChanged);
            // 
            // pnlNodeInventory
            // 
            this.pnlNodeInventory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeInventory.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeInventory.Location = new System.Drawing.Point(152, 5);
            this.pnlNodeInventory.Name = "pnlNodeInventory";
            this.pnlNodeInventory.Size = new System.Drawing.Size(313, 294);
            this.pnlNodeInventory.TabIndex = 62;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(4, 302);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 64;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuantity.Location = new System.Drawing.Point(882, 12);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(49, 20);
            this.txtQuantity.TabIndex = 66;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(831, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 65;
            this.label6.Text = "Quantity:";
            // 
            // cboReferenceType
            // 
            this.cboReferenceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboReferenceType.FormattingEnabled = true;
            this.cboReferenceType.Location = new System.Drawing.Point(882, 113);
            this.cboReferenceType.Name = "cboReferenceType";
            this.cboReferenceType.Size = new System.Drawing.Size(121, 21);
            this.cboReferenceType.TabIndex = 67;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(846, 116);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 68;
            this.label10.Text = "Type:";
            // 
            // cboLocationType
            // 
            this.cboLocationType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboLocationType.FormattingEnabled = true;
            this.cboLocationType.Location = new System.Drawing.Point(882, 174);
            this.cboLocationType.Name = "cboLocationType";
            this.cboLocationType.Size = new System.Drawing.Size(121, 21);
            this.cboLocationType.TabIndex = 69;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(842, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 70;
            this.label1.Text = "Type:";
            // 
            // lblLocation
            // 
            this.lblLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(886, 158);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(0, 13);
            this.lblLocation.TabIndex = 71;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(829, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 72;
            this.label3.Text = "Location:";
            // 
            // lblReferenceType
            // 
            this.lblReferenceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReferenceType.AutoSize = true;
            this.lblReferenceType.Location = new System.Drawing.Point(866, 87);
            this.lblReferenceType.Name = "lblReferenceType";
            this.lblReferenceType.Size = new System.Drawing.Size(10, 13);
            this.lblReferenceType.TabIndex = 73;
            this.lblReferenceType.Text = "-";
            // 
            // chkPublished
            // 
            this.chkPublished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPublished.AutoSize = true;
            this.chkPublished.Location = new System.Drawing.Point(882, 38);
            this.chkPublished.Name = "chkPublished";
            this.chkPublished.Size = new System.Drawing.Size(72, 17);
            this.chkPublished.TabIndex = 75;
            this.chkPublished.Text = "Published";
            this.chkPublished.UseVisualStyleBackColor = true;
            // 
            // lblDateType
            // 
            this.lblDateType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateType.AutoSize = true;
            this.lblDateType.Location = new System.Drawing.Point(866, 60);
            this.lblDateType.Name = "lblDateType";
            this.lblDateType.Size = new System.Drawing.Size(10, 13);
            this.lblDateType.TabIndex = 76;
            this.lblDateType.Text = "-";
            // 
            // lblItemDate
            // 
            this.lblItemDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblItemDate.AutoSize = true;
            this.lblItemDate.Location = new System.Drawing.Point(886, 60);
            this.lblItemDate.Name = "lblItemDate";
            this.lblItemDate.Size = new System.Drawing.Size(10, 13);
            this.lblItemDate.TabIndex = 77;
            this.lblItemDate.Text = "-";
            // 
            // lblReferenceValue
            // 
            this.lblReferenceValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReferenceValue.AutoSize = true;
            this.lblReferenceValue.Location = new System.Drawing.Point(886, 87);
            this.lblReferenceValue.Name = "lblReferenceValue";
            this.lblReferenceValue.Size = new System.Drawing.Size(10, 13);
            this.lblReferenceValue.TabIndex = 78;
            this.lblReferenceValue.Text = "-";
            // 
            // pnlItem
            // 
            this.pnlItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlItem.BackColor = System.Drawing.SystemColors.Control;
            this.pnlItem.Location = new System.Drawing.Point(470, 7);
            this.pnlItem.Name = "pnlItem";
            this.pnlItem.Size = new System.Drawing.Size(335, 292);
            this.pnlItem.TabIndex = 79;
            // 
            // ctlInventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.pnlItem);
            this.Controls.Add(this.lblReferenceValue);
            this.Controls.Add(this.lblItemDate);
            this.Controls.Add(this.lblDateType);
            this.Controls.Add(this.chkPublished);
            this.Controls.Add(this.lblReferenceType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboLocationType);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cboReferenceType);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.pnlNodeList);
            this.Controls.Add(this.pnlNodeInventory);
            this.Name = "ctlInventory";
            this.Size = new System.Drawing.Size(1025, 319);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlNodeList;
        private System.Windows.Forms.Panel pnlNodeInventory;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboReferenceType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cboLocationType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblReferenceType;
        private System.Windows.Forms.CheckBox chkPublished;
        private System.Windows.Forms.Label lblDateType;
        private System.Windows.Forms.Label lblItemDate;
        private System.Windows.Forms.Label lblReferenceValue;
        private System.Windows.Forms.Panel pnlItem;
    }
}
