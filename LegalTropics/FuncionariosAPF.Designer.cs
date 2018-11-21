namespace LegalTropics
{
    partial class FuncionariosAPF
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
            this.treeViewFuncionarios = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewFuncionarios
            // 
            this.treeViewFuncionarios.Location = new System.Drawing.Point(13, 13);
            this.treeViewFuncionarios.Name = "treeViewFuncionarios";
            this.treeViewFuncionarios.Size = new System.Drawing.Size(775, 425);
            this.treeViewFuncionarios.TabIndex = 0;
            this.treeViewFuncionarios.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFuncionarios_AfterSelect);
            // 
            // FuncionariosAPF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeViewFuncionarios);
            this.Name = "FuncionariosAPF";
            this.Text = "Funcionarios APF";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewFuncionarios;
    }
}