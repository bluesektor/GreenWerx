namespace ClientCore.Controls
{
    partial class ctlUser
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
            this.pnlNodeUser = new System.Windows.Forms.Panel();
            this.btnSaveUser = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLicenseNumber = new System.Windows.Forms.TextBox();
            this.lblLastPasswordChangedDate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblLastLockoutDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLastLoginDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSiteAdmin = new System.Windows.Forms.CheckBox();
            this.chkLockedOut = new System.Windows.Forms.CheckBox();
            this.chkAnonymous = new System.Windows.Forms.CheckBox();
            this.chkBanned = new System.Windows.Forms.CheckBox();
            this.chkApproved = new System.Windows.Forms.CheckBox();
            this.lblError = new System.Windows.Forms.Label();
            this.pnlNodeList = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlNodeUser
            // 
            this.pnlNodeUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeUser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlNodeUser.Location = new System.Drawing.Point(187, 5);
            this.pnlNodeUser.Name = "pnlNodeUser";
            this.pnlNodeUser.Size = new System.Drawing.Size(413, 281);
            this.pnlNodeUser.TabIndex = 18;
            // 
            // btnSaveUser
            // 
            this.btnSaveUser.Location = new System.Drawing.Point(768, 263);
            this.btnSaveUser.Name = "btnSaveUser";
            this.btnSaveUser.Size = new System.Drawing.Size(75, 23);
            this.btnSaveUser.TabIndex = 34;
            this.btnSaveUser.Text = "&Save";
            this.btnSaveUser.UseVisualStyleBackColor = true;
            this.btnSaveUser.Click += new System.EventHandler(this.btnSaveUser_Click);
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(606, 171);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(240, 20);
            this.txtEmail.TabIndex = 33;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(606, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Email:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(606, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "License Number:";
            // 
            // txtLicenseNumber
            // 
            this.txtLicenseNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicenseNumber.Location = new System.Drawing.Point(606, 214);
            this.txtLicenseNumber.Name = "txtLicenseNumber";
            this.txtLicenseNumber.Size = new System.Drawing.Size(237, 20);
            this.txtLicenseNumber.TabIndex = 30;
            // 
            // lblLastPasswordChangedDate
            // 
            this.lblLastPasswordChangedDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastPasswordChangedDate.AutoSize = true;
            this.lblLastPasswordChangedDate.Location = new System.Drawing.Point(728, 60);
            this.lblLastPasswordChangedDate.Name = "lblLastPasswordChangedDate";
            this.lblLastPasswordChangedDate.Size = new System.Drawing.Size(0, 13);
            this.lblLastPasswordChangedDate.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(606, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Last Password Changed:";
            // 
            // lblLastLockoutDate
            // 
            this.lblLastLockoutDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastLockoutDate.AutoSize = true;
            this.lblLastLockoutDate.Location = new System.Drawing.Point(701, 37);
            this.lblLastLockoutDate.Name = "lblLastLockoutDate";
            this.lblLastLockoutDate.Size = new System.Drawing.Size(0, 13);
            this.lblLastLockoutDate.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(606, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Last Lockout Date:";
            // 
            // lblLastLoginDate
            // 
            this.lblLastLoginDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastLoginDate.AutoSize = true;
            this.lblLastLoginDate.Location = new System.Drawing.Point(685, 13);
            this.lblLastLoginDate.Name = "lblLastLoginDate";
            this.lblLastLoginDate.Size = new System.Drawing.Size(0, 13);
            this.lblLastLoginDate.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(606, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Last Login Date:";
            // 
            // chkSiteAdmin
            // 
            this.chkSiteAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSiteAdmin.AutoSize = true;
            this.chkSiteAdmin.Location = new System.Drawing.Point(606, 130);
            this.chkSiteAdmin.Name = "chkSiteAdmin";
            this.chkSiteAdmin.Size = new System.Drawing.Size(76, 17);
            this.chkSiteAdmin.TabIndex = 23;
            this.chkSiteAdmin.Text = "Site Admin";
            this.chkSiteAdmin.UseVisualStyleBackColor = true;
            // 
            // chkLockedOut
            // 
            this.chkLockedOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLockedOut.AutoSize = true;
            this.chkLockedOut.Location = new System.Drawing.Point(696, 107);
            this.chkLockedOut.Name = "chkLockedOut";
            this.chkLockedOut.Size = new System.Drawing.Size(82, 17);
            this.chkLockedOut.TabIndex = 22;
            this.chkLockedOut.Text = "Locked Out";
            this.chkLockedOut.UseVisualStyleBackColor = true;
            // 
            // chkAnonymous
            // 
            this.chkAnonymous.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAnonymous.AutoSize = true;
            this.chkAnonymous.Location = new System.Drawing.Point(606, 84);
            this.chkAnonymous.Name = "chkAnonymous";
            this.chkAnonymous.Size = new System.Drawing.Size(81, 17);
            this.chkAnonymous.TabIndex = 21;
            this.chkAnonymous.Text = "Anonymous";
            this.chkAnonymous.UseVisualStyleBackColor = true;
            // 
            // chkBanned
            // 
            this.chkBanned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBanned.AutoSize = true;
            this.chkBanned.Location = new System.Drawing.Point(606, 107);
            this.chkBanned.Name = "chkBanned";
            this.chkBanned.Size = new System.Drawing.Size(63, 17);
            this.chkBanned.TabIndex = 20;
            this.chkBanned.Text = "Banned";
            this.chkBanned.UseVisualStyleBackColor = true;
            // 
            // chkApproved
            // 
            this.chkApproved.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkApproved.AutoSize = true;
            this.chkApproved.Location = new System.Drawing.Point(696, 84);
            this.chkApproved.Name = "chkApproved";
            this.chkApproved.Size = new System.Drawing.Size(72, 17);
            this.chkApproved.TabIndex = 19;
            this.chkApproved.Text = "Approved";
            this.chkApproved.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(184, 289);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 35;
            // 
            // pnlNodeList
            // 
            this.pnlNodeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlNodeList.Location = new System.Drawing.Point(3, 5);
            this.pnlNodeList.Name = "pnlNodeList";
            this.pnlNodeList.Size = new System.Drawing.Size(178, 281);
            this.pnlNodeList.TabIndex = 62;
            this.pnlNodeList.ClientSizeChanged += new System.EventHandler(this.pnlNodeList_ClientSizeChanged);
            // 
            // ctlUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlNodeList);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnSaveUser);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLicenseNumber);
            this.Controls.Add(this.lblLastPasswordChangedDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblLastLockoutDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLastLoginDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkSiteAdmin);
            this.Controls.Add(this.chkLockedOut);
            this.Controls.Add(this.chkAnonymous);
            this.Controls.Add(this.chkBanned);
            this.Controls.Add(this.chkApproved);
            this.Controls.Add(this.pnlNodeUser);
            this.Name = "ctlUser";
            this.Size = new System.Drawing.Size(852, 305);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlNodeUser;
        private System.Windows.Forms.Button btnSaveUser;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLicenseNumber;
        private System.Windows.Forms.Label lblLastPasswordChangedDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblLastLockoutDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLastLoginDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSiteAdmin;
        private System.Windows.Forms.CheckBox chkLockedOut;
        private System.Windows.Forms.CheckBox chkAnonymous;
        private System.Windows.Forms.CheckBox chkBanned;
        private System.Windows.Forms.CheckBox chkApproved;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Panel pnlNodeList;
    }
}
