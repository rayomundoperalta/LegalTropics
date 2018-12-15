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
            this.buttonHecho = new System.Windows.Forms.Button();
            this.labelFecha = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dateTimePickerPeta
            // 
            this.dateTimePickerPeta.Location = new System.Drawing.Point(3, 31);
            this.dateTimePickerPeta.Name = "dateTimePickerPeta";
            this.dateTimePickerPeta.Size = new System.Drawing.Size(204, 22);
            this.dateTimePickerPeta.TabIndex = 0;
            this.dateTimePickerPeta.ValueChanged += new System.EventHandler(this.dateTimePickerPeta_ValueChanged);
            // 
            // buttonHecho
            // 
            this.buttonHecho.Location = new System.Drawing.Point(150, 3);
            this.buttonHecho.Name = "buttonHecho";
            this.buttonHecho.Size = new System.Drawing.Size(57, 23);
            this.buttonHecho.TabIndex = 2;
            this.buttonHecho.Text = "Hecho";
            this.buttonHecho.UseVisualStyleBackColor = true;
            this.buttonHecho.Click += new System.EventHandler(this.buttonHecho_Click);
            // 
            // labelFecha
            // 
            this.labelFecha.AutoSize = true;
            this.labelFecha.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFecha.Location = new System.Drawing.Point(3, 6);
            this.labelFecha.Name = "labelFecha";
            this.labelFecha.Size = new System.Drawing.Size(141, 19);
            this.labelFecha.TabIndex = 3;
            this.labelFecha.Text = "Fecha: no disponible";
            this.labelFecha.Click += new System.EventHandler(this.labelFecha_Click);
            // 
            // MuestraCapturaFecha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelFecha);
            this.Controls.Add(this.buttonHecho);
            this.Controls.Add(this.dateTimePickerPeta);
            this.Name = "MuestraCapturaFecha";
            this.Size = new System.Drawing.Size(211, 59);
            this.Load += new System.EventHandler(this.MuestraCapturaFecha_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerPeta;
        private System.Windows.Forms.Button buttonHecho;
        private System.Windows.Forms.Label labelFecha;
    }
}
