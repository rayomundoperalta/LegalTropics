namespace ActualizaBaseDatos
{
    partial class MuestraCapturaFecha
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
            this.dateTimePickerPeta = new System.Windows.Forms.DateTimePicker();
            this.textBoxFecha = new System.Windows.Forms.TextBox();
            this.buttonHecho = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePickerPeta
            // 
            this.dateTimePickerPeta.Location = new System.Drawing.Point(3, 31);
            this.dateTimePickerPeta.Name = "dateTimePickerPeta";
            this.dateTimePickerPeta.Size = new System.Drawing.Size(264, 22);
            this.dateTimePickerPeta.TabIndex = 0;
            this.dateTimePickerPeta.ValueChanged += new System.EventHandler(this.dateTimePickerPeta_ValueChanged);
            // 
            // textBoxFecha
            // 
            this.textBoxFecha.Location = new System.Drawing.Point(3, 3);
            this.textBoxFecha.Name = "textBoxFecha";
            this.textBoxFecha.Size = new System.Drawing.Size(264, 22);
            this.textBoxFecha.TabIndex = 1;
            this.textBoxFecha.TextChanged += new System.EventHandler(this.textBoxFecha_TextChanged);
            // 
            // buttonHecho
            // 
            this.buttonHecho.Location = new System.Drawing.Point(87, 59);
            this.buttonHecho.Name = "buttonHecho";
            this.buttonHecho.Size = new System.Drawing.Size(75, 23);
            this.buttonHecho.TabIndex = 2;
            this.buttonHecho.Text = "Hecho";
            this.buttonHecho.UseVisualStyleBackColor = true;
            this.buttonHecho.Click += new System.EventHandler(this.buttonHecho_Click);
            // 
            // MuestraCapturaFecha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonHecho);
            this.Controls.Add(this.textBoxFecha);
            this.Controls.Add(this.dateTimePickerPeta);
            this.Name = "MuestraCapturaFecha";
            this.Size = new System.Drawing.Size(271, 85);
            this.Load += new System.EventHandler(this.MuestraCapturaFecha_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerPeta;
        private System.Windows.Forms.TextBox textBoxFecha;
        private System.Windows.Forms.Button buttonHecho;
    }
}
