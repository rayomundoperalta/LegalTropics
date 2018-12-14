using System;
using System.Windows.Forms;
using FuncionesAuxiliares;

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
                return textBoxFecha.Text;
            }
        }

        public int Año
        {
            get
            {
                return dateTimePickerPeta.Value.Year;
            }
            set
            {
                this.año = value;
                textBoxFecha.Text = FormatoFecha.FechaString(año.ToString(),
                mes.ToString(),
                dia.ToString(), "Fecha: no disponible");
            }
        }

        public int Mes
        {
            get
            {
                return dateTimePickerPeta.Value.Month;
            }
            set
            {
                this.mes = value;
                textBoxFecha.Text = FormatoFecha.FechaString(año.ToString(),
                mes.ToString(),
                dia.ToString(), "Fecha: no disponible");
            }
        }

        public int Dia
        {
            get
            {
                return dateTimePickerPeta.Value.Day;
            }
            set
            {
                this.dia = value;
                textBoxFecha.Text = FormatoFecha.FechaString(año.ToString(),
                mes.ToString(),
                dia.ToString(), "Fecha: no disponible");
            }
        }
        public MuestraCapturaFecha()
        {
            InitializeComponent();
            textBoxFecha.Enabled = true;
            dateTimePickerPeta.Enabled = false;
            buttonHecho.Enabled = false;
        }

        public void SetFecha(int Año, int Mes, int Dia)
        {
            this.Año = Año;
            this.Mes = Mes;
            this.Dia = Dia;
        }

        private void MuestraCapturaFecha_Load(object sender, EventArgs e)
        {
            dateTimePickerPeta.MinDate = new DateTime(1900, 1, 1);
            dateTimePickerPeta.MaxDate = DateTime.Today;
            dateTimePickerPeta.CustomFormat = "yyyy-MMM-dd, yyyy - dddd";
            dateTimePickerPeta.Format = DateTimePickerFormat.Custom;
            textBoxFecha.Text = FormatoFecha.FechaString(dateTimePickerPeta.Value.Year.ToString(),
                dateTimePickerPeta.Value.Month.ToString(),
                dateTimePickerPeta.Value.Day.ToString(), "Fecha: no disponible");
        }

        private void textBoxFecha_TextChanged(object sender, EventArgs e)
        {
            textBoxFecha.Enabled = false;
            dateTimePickerPeta.Enabled = true;
            buttonHecho.Enabled = false;
        }

        private void buttonHecho_Click(object sender, EventArgs e)
        {
            dateTimePickerPeta.Enabled = true;
            textBoxFecha.Enabled = true;
            buttonHecho.Enabled = false;
        }

        private void dateTimePickerPeta_ValueChanged(object sender, EventArgs e)
        {
            textBoxFecha.Text = FormatoFecha.FechaString(dateTimePickerPeta.Value.Year.ToString(),
                dateTimePickerPeta.Value.Month.ToString(),
                dateTimePickerPeta.Value.Day.ToString(), "Fecha: no disponible");
        }
    }
}
