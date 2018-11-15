namespace LegalTropics
{
    partial class NavegaciónAPF
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
            this.treeViewAPF = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewAPF
            // 
            this.treeViewAPF.Location = new System.Drawing.Point(12, 12);
            this.treeViewAPF.Name = "treeViewAPF";
            this.treeViewAPF.Size = new System.Drawing.Size(532, 426);
            this.treeViewAPF.TabIndex = 1;
            this.treeViewAPF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAPF_AfterSelect);
            // 
            // NavegaciónAPF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 450);
            this.ControlBox = false;
            this.Controls.Add(this.treeViewAPF);
            this.Name = "NavegaciónAPF";
            this.Text = "Organigrama de la APF";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewAPF;
    }
}