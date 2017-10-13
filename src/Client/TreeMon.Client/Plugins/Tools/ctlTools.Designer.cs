namespace Tools
{
    partial class ctlTools
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
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.prgStatus = new System.Windows.Forms.ProgressBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeo = new System.Windows.Forms.TabPage();
            this.btnGetCooridnates = new System.Windows.Forms.Button();
            this.tabStrains = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabGeo.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(3, 304);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(654, 108);
            this.txtStatus.TabIndex = 11;
            this.txtStatus.Text = "";
            // 
            // prgStatus
            // 
            this.prgStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgStatus.Location = new System.Drawing.Point(3, 288);
            this.prgStatus.Name = "prgStatus";
            this.prgStatus.Size = new System.Drawing.Size(654, 10);
            this.prgStatus.TabIndex = 10;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-15, -15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabGeo);
            this.tabControl1.Controls.Add(this.tabStrains);
            this.tabControl1.Location = new System.Drawing.Point(3, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(653, 265);
            this.tabControl1.TabIndex = 15;
            // 
            // tabGeo
            // 
            this.tabGeo.Controls.Add(this.btnGetCooridnates);
            this.tabGeo.Location = new System.Drawing.Point(4, 22);
            this.tabGeo.Name = "tabGeo";
            this.tabGeo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeo.Size = new System.Drawing.Size(645, 239);
            this.tabGeo.TabIndex = 0;
            this.tabGeo.Text = "Geo";
            this.tabGeo.UseVisualStyleBackColor = true;
            // 
            // btnGetCooridnates
            // 
            this.btnGetCooridnates.Location = new System.Drawing.Point(6, 29);
            this.btnGetCooridnates.Name = "btnGetCooridnates";
            this.btnGetCooridnates.Size = new System.Drawing.Size(133, 23);
            this.btnGetCooridnates.TabIndex = 16;
            this.btnGetCooridnates.Text = "&Get Coordinates";
            this.btnGetCooridnates.UseVisualStyleBackColor = true;
            this.btnGetCooridnates.Click += new System.EventHandler(this.btnGetCooridnates_Click);
            // 
            // tabStrains
            // 
            this.tabStrains.Location = new System.Drawing.Point(4, 22);
            this.tabStrains.Name = "tabStrains";
            this.tabStrains.Padding = new System.Windows.Forms.Padding(3);
            this.tabStrains.Size = new System.Drawing.Size(645, 239);
            this.tabStrains.TabIndex = 1;
            this.tabStrains.Text = "Strains";
            this.tabStrains.UseVisualStyleBackColor = true;
            // 
            // ctlTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.prgStatus);
            this.Name = "ctlTools";
            this.Size = new System.Drawing.Size(659, 416);
            this.tabControl1.ResumeLayout(false);
            this.tabGeo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.ProgressBar prgStatus;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeo;
        private System.Windows.Forms.Button btnGetCooridnates;
        private System.Windows.Forms.TabPage tabStrains;
    }
}
