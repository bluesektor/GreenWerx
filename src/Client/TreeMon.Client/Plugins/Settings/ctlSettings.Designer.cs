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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.lstPlugins = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveAppearance = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAppearance);
            this.tabControl1.Location = new System.Drawing.Point(0, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(311, 236);
            this.tabControl1.TabIndex = 1;
            // 
            // tabAppearance
            // 
            this.tabAppearance.Controls.Add(this.btnMoveDown);
            this.tabAppearance.Controls.Add(this.btnMoveUp);
            this.tabAppearance.Controls.Add(this.lstPlugins);
            this.tabAppearance.Controls.Add(this.label1);
            this.tabAppearance.Controls.Add(this.btnSaveAppearance);
            this.tabAppearance.Location = new System.Drawing.Point(4, 22);
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.Padding = new System.Windows.Forms.Padding(3);
            this.tabAppearance.Size = new System.Drawing.Size(303, 210);
            this.tabAppearance.TabIndex = 0;
            this.tabAppearance.Text = "Appearance";
            this.tabAppearance.UseVisualStyleBackColor = true;
            // 
            // lstPlugins
            // 
            this.lstPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lstPlugins.Location = new System.Drawing.Point(10, 47);
            this.lstPlugins.Name = "lstPlugins";
            this.lstPlugins.Size = new System.Drawing.Size(195, 145);
            this.lstPlugins.TabIndex = 5;
            this.lstPlugins.UseCompatibleStateImageBehavior = false;
            this.lstPlugins.View = System.Windows.Forms.View.Details;
             // 
            // colName
            // 
            this.colName.Text = "Plugin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Plugin Order";
            // 
            // btnSaveAppearance
            // 
            this.btnSaveAppearance.Location = new System.Drawing.Point(211, 169);
            this.btnSaveAppearance.Name = "btnSaveAppearance";
            this.btnSaveAppearance.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAppearance.TabIndex = 2;
            this.btnSaveAppearance.Text = "&Save";
            this.btnSaveAppearance.UseVisualStyleBackColor = true;
            this.btnSaveAppearance.Click += new System.EventHandler(this.btnSaveAppearance_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(211, 76);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(35, 23);
            this.btnMoveUp.TabIndex = 6;
            this.btnMoveUp.Text = "+";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(211, 105);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(35, 23);
            this.btnMoveDown.TabIndex = 7;
            this.btnMoveDown.Text = "-";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // ctlSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.tabControl1);
            this.Name = "ctlSettings";
            this.Size = new System.Drawing.Size(322, 260);
            this.tabControl1.ResumeLayout(false);
            this.tabAppearance.ResumeLayout(false);
            this.tabAppearance.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveAppearance;
        private System.Windows.Forms.ListView lstPlugins;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
    }
}
