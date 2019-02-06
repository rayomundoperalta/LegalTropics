using System;
using System.Windows.Forms;
using Peta;

namespace ActualizaBaseDatos
{
    public partial class MuestraCapturaFecha : UserControl
    {
        private int año = 1900;
        private int mes = 1;
        private int dia = 1;

        public string StringFecha
        {
            get
            {
                return labelFecha.Text;
            }
        }

        public int Año
        {
            get
            {
                return this.año;
            }
            set
            {
                this.año = value;
                labelFecha.Text = Fecha.FechaString(this.año.ToString(),
                this.mes.ToString(),
                this.dia.ToString(), "Fecha: no disponible");
            }
        }

        public int Mes
        {
            get
            {
                return this.mes;
            }
            set
            {
                this.mes = value;
                labelFecha.Text = Fecha.FechaString(this.año.ToString(),
                this.mes.ToString(),
                this.dia.ToString(), "Fecha: no disponible");
            }
        }

        public int Dia
        {
            get
            {
                return this.dia;
            }
            set
            {
                this.dia = value;
                labelFecha.Text = Fecha.FechaString(año.ToString(),
                this.mes.ToString(),
                this.dia.ToString(), "Fecha: no disponible");
            }
        }
        public MuestraCapturaFecha()
        {
            InitializeComponent();
            dateTimePickerPeta.Enabled = false;
            dateTimePickerPeta.Hide();
            buttonHecho.Enabled = false;
            buttonHecho.Hide();
        }

        public void SetFecha(int Año, int Mes, int Dia)
        {
            this.año = Año;
            this.mes = Mes;
            this.dia = Dia;
            labelFecha.Text = Fecha.FechaString(this.año.ToString(),
                this.mes.ToString(),
                this.dia.ToString(), "Fecha: no disponible");
        }

        private void MuestraCapturaFecha_Load(object sender, EventArgs e)
        {
            dateTimePickerPeta.MinDate = new DateTime(1900, 1, 1);
            dateTimePickerPeta.MaxDate = new DateTime(2100, 1, 1);
            dateTimePickerPeta.CustomFormat = "yyyy-MMM-dd";
            dateTimePickerPeta.Format = DateTimePickerFormat.Custom;
            
            labelFecha.Text = Fecha.FechaString(dateTimePickerPeta.Value.Year.ToString(),
                dateTimePickerPeta.Value.Month.ToString(),
                dateTimePickerPeta.Value.Day.ToString(), "Fecha: no disponible");
        }

        private void buttonHecho_Click(object sender, EventArgs e)
        {
            dateTimePickerPeta.Enabled = false;
            dateTimePickerPeta.Hide();
            buttonHecho.Enabled = false;
            buttonHecho.Hide();
        }

        private void dateTimePickerPeta_ValueChanged(object sender, EventArgs e)
        {
            this.año = dateTimePickerPeta.Value.Year;
            this.mes = dateTimePickerPeta.Value.Month;
            this.dia = dateTimePickerPeta.Value.Day;
            labelFecha.Text = Fecha.FechaString(dateTimePickerPeta.Value.Year.ToString(),
                dateTimePickerPeta.Value.Month.ToString(),
                dateTimePickerPeta.Value.Day.ToString(), "Fecha: no disponible");
        }

        private void labelFecha_Click(object sender, EventArgs e)
        {
            dateTimePickerPeta.Enabled = true;
            dateTimePickerPeta.Show();
            buttonHecho.Enabled = true;
            buttonHecho.Show();
        }
    }
}
