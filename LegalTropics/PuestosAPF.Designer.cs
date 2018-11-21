namespace LegalTropics
{
    partial class PuestosAPF
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
            this.treeViewPuestos = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewPuestos
            // 
            this.treeViewPuestos.Location = new System.Drawing.Point(13, 13);
            this.treeViewPuestos.Name = "treeViewPuestos";
            this.treeViewPuestos.Size = new System.Drawing.Size(775, 425);
            this.treeViewPuestos.TabIndex = 0;
            this.treeViewPuestos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPuestos_AfterSelect);
            // 
            // PuestosAPF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeViewPuestos);
            this.Name = "PuestosAPF";
            this.Text = "Puestos APF";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewPuestos;
    }
}