namespace TreeMon.Client
{
    partial class frmInstall
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtDatabaseUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDatabasePassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboDatabaseProviders = new System.Windows.Forms.ComboBox();
            this.txtSiteDomain = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnInstallApp = new System.Windows.Forms.Button();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpUserAccount = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpDatabaseInstall = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPasswordQuestion = new System.Windows.Forms.TextBox();
            this.txtPasswordAnswer = new System.Windows.Forms.TextBox();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grpUserAccount.SuspendLayout();
            this.grpDatabaseInstall.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(4, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(657, 402);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtErrors);
            this.tabPage1.Controls.Add(this.grpDatabaseInstall);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.grpUserAccount);
            this.tabPage1.Controls.Add(this.txtSiteDomain);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(649, 376);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtDatabaseUser
            // 
            this.txtDatabaseUser.Location = new System.Drawing.Point(24, 138);
            this.txtDatabaseUser.Name = "txtDatabaseUser";
            this.txtDatabaseUser.Size = new System.Drawing.Size(183, 20);
            this.txtDatabaseUser.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Database User";
            // 
            // txtDatabasePassword
            // 
            this.txtDatabasePassword.Location = new System.Drawing.Point(24, 183);
            this.txtDatabasePassword.Name = "txtDatabasePassword";
            this.txtDatabasePassword.PasswordChar = '*';
            this.txtDatabasePassword.Size = new System.Drawing.Size(183, 20);
            this.txtDatabasePassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database Provider";
            // 
            // cboDatabaseProviders
            // 
            this.cboDatabaseProviders.FormattingEnabled = true;
            this.cboDatabaseProviders.Location = new System.Drawing.Point(24, 47);
            this.cboDatabaseProviders.Name = "cboDatabaseProviders";
            this.cboDatabaseProviders.Size = new System.Drawing.Size(183, 21);
            this.cboDatabaseProviders.TabIndex = 2;
            // 
            // txtSiteDomain
            // 
            this.txtSiteDomain.Location = new System.Drawing.Point(470, 37);
            this.txtSiteDomain.Name = "txtSiteDomain";
            this.txtSiteDomain.Size = new System.Drawing.Size(142, 20);
            this.txtSiteDomain.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(472, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "App. Name";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(649, 376);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnInstallApp
            // 
            this.btnInstallApp.Location = new System.Drawing.Point(24, 209);
            this.btnInstallApp.Name = "btnInstallApp";
            this.btnInstallApp.Size = new System.Drawing.Size(75, 23);
            this.btnInstallApp.TabIndex = 8;
            this.btnInstallApp.Text = "&Install";
            this.btnInstallApp.UseVisualStyleBackColor = true;
            this.btnInstallApp.Click += new System.EventHandler(this.btnInstallApp_Click);
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(24, 93);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(183, 20);
            this.txtDatabaseName.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Database Name:";
            // 
            // grpUserAccount
            // 
            this.grpUserAccount.Controls.Add(this.txtPasswordAnswer);
            this.grpUserAccount.Controls.Add(this.txtPasswordQuestion);
            this.grpUserAccount.Controls.Add(this.label10);
            this.grpUserAccount.Controls.Add(this.label8);
            this.grpUserAccount.Controls.Add(this.label9);
            this.grpUserAccount.Controls.Add(this.txtConfirmPassword);
            this.grpUserAccount.Controls.Add(this.txtPassword);
            this.grpUserAccount.Controls.Add(this.label7);
            this.grpUserAccount.Controls.Add(this.txtUserName);
            this.grpUserAccount.Controls.Add(this.label6);
            this.grpUserAccount.Location = new System.Drawing.Point(6, 20);
            this.grpUserAccount.Name = "grpUserAccount";
            this.grpUserAccount.Size = new System.Drawing.Size(195, 301);
            this.grpUserAccount.TabIndex = 17;
            this.grpUserAccount.TabStop = false;
            this.grpUserAccount.Text = "User Account";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Confirm Password";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Location = new System.Drawing.Point(3, 138);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(183, 20);
            this.txtConfirmPassword.TabIndex = 21;
            this.txtConfirmPassword.TextChanged += new System.EventHandler(this.ValidateUserAccount);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(3, 94);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(180, 20);
            this.txtPassword.TabIndex = 20;
            this.txtPassword.TextChanged += new System.EventHandler(this.ValidateUserAccount);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Password";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(3, 47);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(183, 20);
            this.txtUserName.TabIndex = 18;
            this.txtUserName.TextChanged += new System.EventHandler(this.ValidateUserAccount);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Username";
            // 
            // grpDatabaseInstall
            // 
            this.grpDatabaseInstall.Controls.Add(this.txtDatabaseName);
            this.grpDatabaseInstall.Controls.Add(this.label5);
            this.grpDatabaseInstall.Controls.Add(this.cboDatabaseProviders);
            this.grpDatabaseInstall.Controls.Add(this.btnInstallApp);
            this.grpDatabaseInstall.Controls.Add(this.label2);
            this.grpDatabaseInstall.Controls.Add(this.txtDatabaseUser);
            this.grpDatabaseInstall.Controls.Add(this.label3);
            this.grpDatabaseInstall.Controls.Add(this.label4);
            this.grpDatabaseInstall.Controls.Add(this.txtDatabasePassword);
            this.grpDatabaseInstall.Enabled = false;
            this.grpDatabaseInstall.Location = new System.Drawing.Point(207, 20);
            this.grpDatabaseInstall.Name = "grpDatabaseInstall";
            this.grpDatabaseInstall.Size = new System.Drawing.Size(223, 301);
            this.grpDatabaseInstall.TabIndex = 18;
            this.grpDatabaseInstall.TabStop = false;
            this.grpDatabaseInstall.Text = "Database Install";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 170);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Password hint question:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 219);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(113, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Password hint answer:";
            // 
            // txtPasswordQuestion
            // 
            this.txtPasswordQuestion.Location = new System.Drawing.Point(3, 186);
            this.txtPasswordQuestion.Name = "txtPasswordQuestion";
            this.txtPasswordQuestion.Size = new System.Drawing.Size(183, 20);
            this.txtPasswordQuestion.TabIndex = 23;
            this.txtPasswordQuestion.TextChanged += new System.EventHandler(this.ValidateUserAccount);
            // 
            // txtPasswordAnswer
            // 
            this.txtPasswordAnswer.Location = new System.Drawing.Point(3, 235);
            this.txtPasswordAnswer.Name = "txtPasswordAnswer";
            this.txtPasswordAnswer.Size = new System.Drawing.Size(183, 20);
            this.txtPasswordAnswer.TabIndex = 24;
            this.txtPasswordAnswer.TextChanged += new System.EventHandler(this.ValidateUserAccount);
            // 
            // txtErrors
            // 
            this.txtErrors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtErrors.ForeColor = System.Drawing.Color.Red;
            this.txtErrors.Location = new System.Drawing.Point(3, 327);
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.Size = new System.Drawing.Size(643, 13);
            this.txtErrors.TabIndex = 19;
            // 
            // frmInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 462);
            this.Controls.Add(this.tabControl1);
            this.MinimizeBox = false;
            this.Name = "frmInstall";
            this.Text = "Install";
            this.Load += new System.EventHandler(this.frmInstall_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.grpUserAccount.ResumeLayout(false);
            this.grpUserAccount.PerformLayout();
            this.grpDatabaseInstall.ResumeLayout(false);
            this.grpDatabaseInstall.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtSiteDomain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDatabaseProviders;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDatabasePassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDatabaseUser;
        private System.Windows.Forms.Button btnInstallApp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.GroupBox grpUserAccount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpDatabaseInstall;
        private System.Windows.Forms.TextBox txtPasswordAnswer;
        private System.Windows.Forms.TextBox txtPasswordQuestion;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtErrors;
    }
}