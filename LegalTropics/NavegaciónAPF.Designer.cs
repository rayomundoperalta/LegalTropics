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
            this.textBoxOrgCadenaBusqueda = new System.Windows.Forms.TextBox();
            this.buttonOrgBuscarFuncionario = new System.Windows.Forms.Button();
            this.textBoxOrgBuscaID = new System.Windows.Forms.TextBox();
            this.buttonOrgTraeID = new System.Windows.Forms.Button();
            this.buttonOrgBuscaPuesto = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewAPF
            // 
            this.treeViewAPF.Location = new System.Drawing.Point(12, 12);
            this.treeViewAPF.Name = "treeViewAPF";
            this.treeViewAPF.Size = new System.Drawing.Size(936, 642);
            this.treeViewAPF.TabIndex = 1;
            this.treeViewAPF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAPF_AfterSelect);
            // 
            // textBoxOrgCadenaBusqueda
            // 
            this.textBoxOrgCadenaBusqueda.Location = new System.Drawing.Point(12, 661);
            this.textBoxOrgCadenaBusqueda.Name = "textBoxOrgCadenaBusqueda";
            this.textBoxOrgCadenaBusqueda.Size = new System.Drawing.Size(350, 22);
            this.textBoxOrgCadenaBusqueda.TabIndex = 2;
            this.textBoxOrgCadenaBusqueda.TextChanged += new System.EventHandler(this.textBoxOrgCadenaBusqueda_TextChanged);
            // 
            // buttonOrgBuscarFuncionario
            // 
            this.buttonOrgBuscarFuncionario.Location = new System.Drawing.Point(368, 661);
            this.buttonOrgBuscarFuncionario.Name = "buttonOrgBuscarFuncionario";
            this.buttonOrgBuscarFuncionario.Size = new System.Drawing.Size(142, 23);
            this.buttonOrgBuscarFuncionario.TabIndex = 3;
            this.buttonOrgBuscarFuncionario.Text = "Buscar Funcionario";
            this.buttonOrgBuscarFuncionario.UseVisualStyleBackColor = true;
            this.buttonOrgBuscarFuncionario.Click += new System.EventHandler(this.buttonOrgBuscarFuncionario_Click);
            // 
            // textBoxOrgBuscaID
            // 
            this.textBoxOrgBuscaID.Location = new System.Drawing.Point(658, 660);
            this.textBoxOrgBuscaID.Name = "textBoxOrgBuscaID";
            this.textBoxOrgBuscaID.Size = new System.Drawing.Size(142, 22);
            this.textBoxOrgBuscaID.TabIndex = 4;
            this.textBoxOrgBuscaID.TextChanged += new System.EventHandler(this.textBoxOrgBuscaID_TextChanged);
            // 
            // buttonOrgTraeID
            // 
            this.buttonOrgTraeID.Location = new System.Drawing.Point(806, 661);
            this.buttonOrgTraeID.Name = "buttonOrgTraeID";
            this.buttonOrgTraeID.Size = new System.Drawing.Size(142, 23);
            this.buttonOrgTraeID.TabIndex = 5;
            this.buttonOrgTraeID.Text = "Busca ID";
            this.buttonOrgTraeID.UseVisualStyleBackColor = true;
            this.buttonOrgTraeID.Click += new System.EventHandler(this.buttonOrgTraeID_Click);
            // 
            // buttonOrgBuscaPuesto
            // 
            this.buttonOrgBuscaPuesto.Location = new System.Drawing.Point(516, 660);
            this.buttonOrgBuscaPuesto.Name = "buttonOrgBuscaPuesto";
            this.buttonOrgBuscaPuesto.Size = new System.Drawing.Size(136, 23);
            this.buttonOrgBuscaPuesto.TabIndex = 6;
            this.buttonOrgBuscaPuesto.Text = "Busca Puesto";
            this.buttonOrgBuscaPuesto.UseVisualStyleBackColor = true;
            this.buttonOrgBuscaPuesto.Click += new System.EventHandler(this.buttonOrgBuscaPuesto_Click);
            // 
            // NavegaciónAPF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 695);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOrgBuscaPuesto);
            this.Controls.Add(this.buttonOrgTraeID);
            this.Controls.Add(this.textBoxOrgBuscaID);
            this.Controls.Add(this.buttonOrgBuscarFuncionario);
            this.Controls.Add(this.textBoxOrgCadenaBusqueda);
            this.Controls.Add(this.treeViewAPF);
            this.Name = "NavegaciónAPF";
            this.Text = "Organigrama de la APF";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewAPF;
        private System.Windows.Forms.TextBox textBoxOrgCadenaBusqueda;
        private System.Windows.Forms.Button buttonOrgBuscarFuncionario;
        private System.Windows.Forms.TextBox textBoxOrgBuscaID;
        private System.Windows.Forms.Button buttonOrgTraeID;
        private System.Windows.Forms.Button buttonOrgBuscaPuesto;
    }
}