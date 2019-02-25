namespace ClientCore.Controls
{
    partial class ctlNodeTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlNodeTree));
            this.btnDeleteNode = new System.Windows.Forms.Button();
            this.btnAddNode = new System.Windows.Forms.Button();
            this.btnAddSubNode = new System.Windows.Forms.Button();
            this.treeNodes = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btnDeleteNode
            // 
            this.btnDeleteNode.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnDeleteNode.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteNode.Image")));
            this.btnDeleteNode.Location = new System.Drawing.Point(115, 252);
            this.btnDeleteNode.Name = "btnDeleteNode";
            this.btnDeleteNode.Size = new System.Drawing.Size(28, 28);
            this.btnDeleteNode.TabIndex = 41;
            this.btnDeleteNode.UseVisualStyleBackColor = true;
            this.btnDeleteNode.Click += new System.EventHandler(this.btnDeleteNode_Click);
            // 
            // btnAddNode
            // 
            this.btnAddNode.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddNode.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNode.Image")));
            this.btnAddNode.Location = new System.Drawing.Point(39, 252);
            this.btnAddNode.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddNode.Name = "btnAddNode";
            this.btnAddNode.Size = new System.Drawing.Size(28, 28);
            this.btnAddNode.TabIndex = 40;
            this.btnAddNode.UseVisualStyleBackColor = true;
            this.btnAddNode.Click += new System.EventHandler(this.btnAddNode_Click);
            // 
            // btnAddSubNode
            // 
            this.btnAddSubNode.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddSubNode.Image = ((System.Drawing.Image)(resources.GetObject("btnAddSubNode.Image")));
            this.btnAddSubNode.Location = new System.Drawing.Point(8, 252);
            this.btnAddSubNode.Name = "btnAddSubNode";
            this.btnAddSubNode.Size = new System.Drawing.Size(28, 28);
            this.btnAddSubNode.TabIndex = 39;
            this.btnAddSubNode.UseVisualStyleBackColor = true;
            this.btnAddSubNode.Click += new System.EventHandler(this.btnAddSubNode_Click);
            // 
            // treeNodes
            // 
            this.treeNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeNodes.Location = new System.Drawing.Point(6, 4);
            this.treeNodes.Name = "treeNodes";
            this.treeNodes.Size = new System.Drawing.Size(146, 242);
            this.treeNodes.TabIndex = 42;
            this.treeNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeNodes_AfterSelect);
            // 
            // ctlNodeTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.treeNodes);
            this.Controls.Add(this.btnDeleteNode);
            this.Controls.Add(this.btnAddNode);
            this.Controls.Add(this.btnAddSubNode);
            this.Name = "ctlNodeTree";
            this.Size = new System.Drawing.Size(155, 289);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteNode;
        private System.Windows.Forms.Button btnAddNode;
        private System.Windows.Forms.Button btnAddSubNode;
        private System.Windows.Forms.TreeView treeNodes;
    }
}
