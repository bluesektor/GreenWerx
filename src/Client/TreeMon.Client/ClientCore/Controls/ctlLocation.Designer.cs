namespace ClientCore.Controls
{
    partial class ctlLocation
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
            this.trvLocations = new System.Windows.Forms.TreeView();
            this.pnlNodeLocation = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.txtAbbr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLatitude = new System.Windows.Forms.TextBox();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.txtLongitude = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cboCity = new System.Windows.Forms.ComboBox();
            this.cboState = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtPostal = new System.Windows.Forms.TextBox();
            this.cboCountry = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkIsBillingAddress = new System.Windows.Forms.CheckBox();
            this.chkDispensary = new System.Windows.Forms.CheckBox();
            this.chkCultivation = new System.Windows.Forms.CheckBox();
            this.chkManufacturing = new System.Windows.Forms.CheckBox();
            this.chkLab = new System.Windows.Forms.CheckBox();
            this.chkProcessor = new System.Windows.Forms.CheckBox();
            this.chkRetailer = new System.Windows.Forms.CheckBox();
            this.chkVirtual = new System.Windows.Forms.CheckBox();
            this.chkisDefault = new System.Windows.Forms.CheckBox();
            this.chkOnlineStore = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // trvLocations
            // 
            this.trvLocations.Location = new System.Drawing.Point(4, 4);
            this.trvLocations.Name = "trvLocations";
            this.trvLocations.ShowRootLines = false;
            this.trvLocations.Size = new System.Drawing.Size(159, 294);
            this.trvLocations.TabIndex = 20;
            this.trvLocations.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvLocations_NodeMouseClick);
            // 
            // pnlNodeLocation
            // 
            this.pnlNodeLocation.Location = new System.Drawing.Point(169, 4);
            this.pnlNodeLocation.Name = "pnlNodeLocation";
            this.pnlNodeLocation.Size = new System.Drawing.Size(377, 294);
            this.pnlNodeLocation.TabIndex = 21;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(878, 275);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(3, 393);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 23;
            // 
            // txtAbbr
            // 
            this.txtAbbr.Location = new System.Drawing.Point(615, 8);
            this.txtAbbr.Name = "txtAbbr";
            this.txtAbbr.Size = new System.Drawing.Size(121, 20);
            this.txtAbbr.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(577, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Abbr.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(574, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Code:";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(615, 34);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(121, 20);
            this.txtCode.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(561, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Latitude:";
            // 
            // txtLatitude
            // 
            this.txtLatitude.Location = new System.Drawing.Point(615, 60);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.Size = new System.Drawing.Size(121, 20);
            this.txtLatitude.TabIndex = 29;
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(552, 90);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(57, 13);
            this.lblLongitude.TabIndex = 30;
            this.lblLongitude.Text = "Longitude:";
            // 
            // txtLongitude
            // 
            this.txtLongitude.Location = new System.Drawing.Point(615, 86);
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.Size = new System.Drawing.Size(121, 20);
            this.txtLongitude.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(766, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "First Name:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(832, 7);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(121, 20);
            this.txtFirstName.TabIndex = 33;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(765, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Last Name:";
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(832, 33);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(121, 20);
            this.txtLastName.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(778, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "Address:";
            // 
            // txtAddress1
            // 
            this.txtAddress1.Location = new System.Drawing.Point(832, 59);
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(121, 20);
            this.txtAddress1.TabIndex = 37;
            // 
            // txtAddress2
            // 
            this.txtAddress2.Location = new System.Drawing.Point(832, 85);
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(121, 20);
            this.txtAddress2.TabIndex = 39;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(750, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "Address Cont.:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(799, 169);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 40;
            this.label8.Text = "City:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(791, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 42;
            this.label9.Text = "State:";
            // 
            // cboCity
            // 
            this.cboCity.FormattingEnabled = true;
            this.cboCity.Location = new System.Drawing.Point(832, 165);
            this.cboCity.Name = "cboCity";
            this.cboCity.Size = new System.Drawing.Size(121, 21);
            this.cboCity.TabIndex = 43;
            this.cboCity.SelectedIndexChanged += new System.EventHandler(this.cboCity_SelectedIndexChanged);
            // 
            // cboState
            // 
            this.cboState.FormattingEnabled = true;
            this.cboState.Location = new System.Drawing.Point(832, 138);
            this.cboState.Name = "cboState";
            this.cboState.Size = new System.Drawing.Size(121, 21);
            this.cboState.TabIndex = 44;
            this.cboState.SelectedIndexChanged += new System.EventHandler(this.cboState_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(759, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Postal Code:";
            // 
            // txtPostal
            // 
            this.txtPostal.Location = new System.Drawing.Point(832, 192);
            this.txtPostal.Name = "txtPostal";
            this.txtPostal.Size = new System.Drawing.Size(121, 20);
            this.txtPostal.TabIndex = 48;
            // 
            // cboCountry
            // 
            this.cboCountry.FormattingEnabled = true;
            this.cboCountry.Location = new System.Drawing.Point(832, 111);
            this.cboCountry.Name = "cboCountry";
            this.cboCountry.Size = new System.Drawing.Size(121, 21);
            this.cboCountry.TabIndex = 50;
            this.cboCountry.SelectedIndexChanged += new System.EventHandler(this.cboCountry_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(780, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 49;
            this.label10.Text = "Country:";
            // 
            // chkIsBillingAddress
            // 
            this.chkIsBillingAddress.AutoSize = true;
            this.chkIsBillingAddress.Location = new System.Drawing.Point(832, 221);
            this.chkIsBillingAddress.Name = "chkIsBillingAddress";
            this.chkIsBillingAddress.Size = new System.Drawing.Size(94, 17);
            this.chkIsBillingAddress.TabIndex = 51;
            this.chkIsBillingAddress.Text = "Billing Address";
            this.chkIsBillingAddress.UseVisualStyleBackColor = true;
            // 
            // chkDispensary
            // 
            this.chkDispensary.AutoSize = true;
            this.chkDispensary.Location = new System.Drawing.Point(564, 126);
            this.chkDispensary.Name = "chkDispensary";
            this.chkDispensary.Size = new System.Drawing.Size(78, 17);
            this.chkDispensary.TabIndex = 52;
            this.chkDispensary.Text = "Dispensary";
            this.chkDispensary.UseVisualStyleBackColor = true;
            // 
            // chkCultivation
            // 
            this.chkCultivation.AutoSize = true;
            this.chkCultivation.Location = new System.Drawing.Point(664, 126);
            this.chkCultivation.Name = "chkCultivation";
            this.chkCultivation.Size = new System.Drawing.Size(95, 17);
            this.chkCultivation.TabIndex = 53;
            this.chkCultivation.Text = "Grow Location";
            this.chkCultivation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkCultivation.UseVisualStyleBackColor = true;
            // 
            // chkManufacturing
            // 
            this.chkManufacturing.AutoSize = true;
            this.chkManufacturing.Location = new System.Drawing.Point(564, 149);
            this.chkManufacturing.Name = "chkManufacturing";
            this.chkManufacturing.Size = new System.Drawing.Size(94, 17);
            this.chkManufacturing.TabIndex = 54;
            this.chkManufacturing.Text = "Manufacturing";
            this.chkManufacturing.UseVisualStyleBackColor = true;
            // 
            // chkLab
            // 
            this.chkLab.AutoSize = true;
            this.chkLab.Location = new System.Drawing.Point(664, 149);
            this.chkLab.Name = "chkLab";
            this.chkLab.Size = new System.Drawing.Size(44, 17);
            this.chkLab.TabIndex = 55;
            this.chkLab.Text = "Lab";
            this.chkLab.UseVisualStyleBackColor = true;
            // 
            // chkProcessor
            // 
            this.chkProcessor.AutoSize = true;
            this.chkProcessor.Location = new System.Drawing.Point(564, 172);
            this.chkProcessor.Name = "chkProcessor";
            this.chkProcessor.Size = new System.Drawing.Size(78, 17);
            this.chkProcessor.TabIndex = 56;
            this.chkProcessor.Text = "Processing";
            this.chkProcessor.UseVisualStyleBackColor = true;
            // 
            // chkRetailer
            // 
            this.chkRetailer.AutoSize = true;
            this.chkRetailer.Location = new System.Drawing.Point(664, 172);
            this.chkRetailer.Name = "chkRetailer";
            this.chkRetailer.Size = new System.Drawing.Size(53, 17);
            this.chkRetailer.TabIndex = 57;
            this.chkRetailer.Text = "Retail";
            this.chkRetailer.UseVisualStyleBackColor = true;
            // 
            // chkVirtual
            // 
            this.chkVirtual.AutoSize = true;
            this.chkVirtual.Location = new System.Drawing.Point(564, 221);
            this.chkVirtual.Name = "chkVirtual";
            this.chkVirtual.Size = new System.Drawing.Size(55, 17);
            this.chkVirtual.TabIndex = 58;
            this.chkVirtual.Text = "Virtual";
            this.chkVirtual.UseVisualStyleBackColor = true;
            // 
            // chkisDefault
            // 
            this.chkisDefault.AutoSize = true;
            this.chkisDefault.Location = new System.Drawing.Point(664, 221);
            this.chkisDefault.Name = "chkisDefault";
            this.chkisDefault.Size = new System.Drawing.Size(60, 17);
            this.chkisDefault.TabIndex = 59;
            this.chkisDefault.Text = "Default";
            this.chkisDefault.UseVisualStyleBackColor = true;
            // 
            // chkOnlineStore
            // 
            this.chkOnlineStore.AutoSize = true;
            this.chkOnlineStore.Location = new System.Drawing.Point(564, 244);
            this.chkOnlineStore.Name = "chkOnlineStore";
            this.chkOnlineStore.Size = new System.Drawing.Size(84, 17);
            this.chkOnlineStore.TabIndex = 60;
            this.chkOnlineStore.Text = "Online Store";
            this.chkOnlineStore.UseVisualStyleBackColor = true;
            // 
            // ctlLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkOnlineStore);
            this.Controls.Add(this.chkisDefault);
            this.Controls.Add(this.chkVirtual);
            this.Controls.Add(this.chkRetailer);
            this.Controls.Add(this.chkProcessor);
            this.Controls.Add(this.chkLab);
            this.Controls.Add(this.chkManufacturing);
            this.Controls.Add(this.chkCultivation);
            this.Controls.Add(this.chkDispensary);
            this.Controls.Add(this.chkIsBillingAddress);
            this.Controls.Add(this.cboCountry);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtPostal);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cboState);
            this.Controls.Add(this.cboCity);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtAddress2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtAddress1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLongitude);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.txtLatitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAbbr);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnlNodeLocation);
            this.Controls.Add(this.trvLocations);
            this.Name = "ctlLocation";
            this.Size = new System.Drawing.Size(959, 308);
            this.Load += new System.EventHandler(this.ctlLocation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView trvLocations;
        private System.Windows.Forms.Panel pnlNodeLocation;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtAbbr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLatitude;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.TextBox txtLongitude;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAddress1;
        private System.Windows.Forms.TextBox txtAddress2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboCity;
        private System.Windows.Forms.ComboBox cboState;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtPostal;
        private System.Windows.Forms.ComboBox cboCountry;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkIsBillingAddress;
        private System.Windows.Forms.CheckBox chkDispensary;
        private System.Windows.Forms.CheckBox chkCultivation;
        private System.Windows.Forms.CheckBox chkManufacturing;
        private System.Windows.Forms.CheckBox chkLab;
        private System.Windows.Forms.CheckBox chkProcessor;
        private System.Windows.Forms.CheckBox chkRetailer;
        private System.Windows.Forms.CheckBox chkVirtual;
        private System.Windows.Forms.CheckBox chkisDefault;
        private System.Windows.Forms.CheckBox chkOnlineStore;
    }
}
