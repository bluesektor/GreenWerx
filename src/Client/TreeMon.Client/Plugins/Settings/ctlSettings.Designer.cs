namespace Settings
{
    partial class ctlSettings
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnImportDataset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(258, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "&Install";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnImportDataset
            // 
            this.btnImportDataset.Location = new System.Drawing.Point(114, 296);
            this.btnImportDataset.Name = "btnImportDataset";
            this.btnImportDataset.Size = new System.Drawing.Size(113, 23);
            this.btnImportDataset.TabIndex = 1;
            this.btnImportDataset.Text = "&Import Dataset";
            this.btnImportDataset.UseVisualStyleBackColor = true;
            this.btnImportDataset.Click += new System.EventHandler(this.btnImportDataset_Click);
            // 
            // ctlSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.btnImportDataset);
            this.Controls.Add(this.button1);
            this.Name = "ctlSettings";
            this.Size = new System.Drawing.Size(842, 366);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnImportDataset;
    }
}
