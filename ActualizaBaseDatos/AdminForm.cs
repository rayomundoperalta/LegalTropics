using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MSAccess;
using Globales;

namespace ActualizaBaseDatos
{
    public partial class AdminForm : Form
    {
        string IDFuncionario;
        IndiceBD funcionarioMostrado;
        IndiceBD indexEscolaridad;
        IndiceBD indexAP;
        IndiceBD indexINFO;
        IndiceBD indexPuestos;
        string NewFotoFileName;

        public AdminForm()
        {
            string[] PhotoFiles = Directory.GetFiles(Defines.FotoTempBasePath, "*.*");

            for (int i = 0; i < PhotoFiles.Length; i++)
            {
                FileInfo fi = new FileInfo(PhotoFiles[i]);
                fi.Delete();
            }
            InitializeComponent();
            this.FormClosed += AdminForm_FormClosed;
        }

        private void AdminForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        DataRow[] funcionarios;
        DataRow[] escolaridad;
        DataRow[] AP;
        DataRow[] INFO;
        DataRow[] Puestos;

        private void DespliegaInformación(int index)
        {
            // Datos Personales

            textBoxPrimerNombre.Text = funcionarios[index]["PrimerNombre"].ToString();
            textBoxSegundoNombre.Text = funcionarios[index]["SegundoNombre"].ToString();
            textBoxApellidoPaterno.Text = funcionarios[index]["ApellidoPaterno"].ToString();
            textBoxApellidoMaterno.Text = funcionarios[index]["ApellidoMaterno"].ToString();
            textBoxNacionalidad.Text = funcionarios[index]["Nacionalidad"].ToString();
            textBoxFechaNacimiento.Text = funcionarios[index]["FechaDeNacimiento"].ToString();

            IDFuncionario = funcionarios[index]["ID"].ToString();

            LoadPhoto(AccessUtility.GetFoto(IDFuncionario));

            escolaridad = AccessUtility.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad();

            AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP();

            INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO();

            Puestos = AccessUtility.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos();

            GetTabShown();

            tabControlAdministracionBaseDatos.Selected += TabControlAdministracionBaseDatos_Selected;
            tabControlInformación.Selected += TabControlInformación_Selected;
        }

        private void LoadPhoto(string PhotoFileName)
        {
            pictureBox1.ImageLocation = PhotoFileName;
            pictureBox1.Load();
            // ancho 172 alto 199
            float prop = (float)pictureBox1.Width / (float)pictureBox1.Height;
            pictureBox1.Height = 199;
            pictureBox1.Width = (int) (prop * 199);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void LlenaEscolaridad()
        {
            // Escolaridad
            textBoxID.Text = escolaridad[indexEscolaridad.Pos]["ID"].ToString();
            textBoxFechaDeInicio.Text = escolaridad[indexEscolaridad.Pos]["FechaDeInicio"].ToString();
            textBoxFechaDeFin.Text = escolaridad[indexEscolaridad.Pos]["FechaDeFin"].ToString();
            textBoxUniversidad.Text = escolaridad[indexEscolaridad.Pos]["Universidad"].ToString();
            textBoxGrado.Text = escolaridad[indexEscolaridad.Pos]["Grado"].ToString();
            labelEscolaridadPos.Text = (indexEscolaridad.Pos + 1).ToString();
            labelEscolaridadLength.Text = "de " + indexEscolaridad.Length.ToString();
        }
        
        private void LlenaAP()
        {
            // Adscripción Política
            
            textBoxAPID.Text = AP[indexAP.Pos]["ID"].ToString();
            textBoxAPFechaDeInicio.Text = AP[indexAP.Pos]["FechaDeInicio"].ToString();
            textBoxAPFechaDeFin.Text = AP[indexAP.Pos]["FechaDeFin"].ToString();
            textBoxAPPartido.Text = AP[indexAP.Pos]["NombreDelPartido"].ToString();
            labelAPPos.Text = (indexAP.Pos + 1).ToString();
            labelAPLength.Text = "de " + indexAP.Length.ToString();
        }

        private void LlenaINFO()
        {
            // Información General
            
            textBoxINFOID.Text = INFO[indexINFO.Pos]["ID"].ToString();
            textBoxINFOTipoDeInformacion.Text = INFO[indexINFO.Pos]["TipoDeInformación"].ToString();
            textBoxINFOReferencia.Multiline = true;
            textBoxINFOReferencia.Text = INFO[indexINFO.Pos]["Referencia"].ToString();
            labelINFOPos.Text = (indexINFO.Pos + 1).ToString();
            labelINFOLength.Text = "de " + indexINFO.Length.ToString();
        }

        private void LlenaPuestos()
        {
            // Puestos
            
            textBoxPuestosID.Text = Puestos[indexPuestos.Pos]["ID"].ToString();
            textBoxPuestosDependencia.Text = Puestos[indexPuestos.Pos]["DependenciaEntidad"].ToString();
            textBoxPuestosPuesto.Text = Puestos[indexPuestos.Pos]["Puesto"].ToString();
            textBoxPuestosSuperior.Text = Puestos[indexPuestos.Pos]["JefeInmediantoSuperior"].ToString();
            textBoxPuestosFechaDeInicio.Text = Puestos[indexPuestos.Pos]["FechaDeInicio"].ToString();
            textBoxPuestosFechaDeFin.Text = Puestos[indexPuestos.Pos]["FechaDeFin"].ToString();
            labelPuestosPos.Text = (indexPuestos.Pos + 1).ToString();
            labelPuestosLength.Text = "de " + indexPuestos.Length.ToString();
        }

        private void TabControlInformación_Selected(object sender, TabControlEventArgs e)
        {
            GetTabShown();
            switch (TabPageInformación.Text)
            {
                case "Escolaridad":
                    LlenaEscolaridad();
                    break;
                case "Adscripción Política":
                    LlenaAP();
                    break;
                case "Información General":
                    LlenaINFO();
                    break;
                case "Puestos":
                    LlenaPuestos();
                    break;
                default:
                    break;
            }
            tabControlInformación.Focus();
        }

        private void TabControlAdministracionBaseDatos_Selected(object sender, TabControlEventArgs e)
        {
            GetTabShown();
        }
        
        private void buttonBusca_Click(object sender, EventArgs e)
        {

        }
        
        private void AdminForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bDSecretarias1DataSet.Escolaridad' table. You can move, or remove it, as needed.
            funcionarios = AccessUtility.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            DespliegaInformación(funcionarioMostrado.Pos);
            
        }

        private void tabPageEscolaridad_Click(object sender, EventArgs e)
        {

        }

        TabPage TabPageAdministración;
        TabPage TabPageInformación;

        private void GetTabShown()
        {
            TabPageAdministración = tabControlAdministracionBaseDatos.SelectedTab;
            //TabPageAdministración.Text
            TabPageInformación = tabControlInformación.SelectedTab;
        }

        private void textBoxPuestosPuesto_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void buttonAnterior_Click(object sender, EventArgs e)
        {
            funcionarioMostrado.Previous();
            DespliegaInformación(funcionarioMostrado.Pos);
        }

        private void buttonSiguiente_Click(object sender, EventArgs e)
        {
            funcionarioMostrado.Next();
            DespliegaInformación(funcionarioMostrado.Pos);
        }

        private void buttonEscolaridadInicial_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Inicial();
            LlenaEscolaridad();
        }

        private void buttonEscolaridadSiguiente_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Next();
            LlenaEscolaridad();
        }

        private void buttonEscolaridadPrevio_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Previous();
            LlenaEscolaridad();
        }

        private void buttonEscolaridadFinal_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Final();
            LlenaEscolaridad();
        }

        private void buttonEscolaridadLimpia_Click(object sender, EventArgs e)
        {
            // Escolaridad
            textBoxID.Text = escolaridad[indexEscolaridad.Pos]["ID"].ToString();
            textBoxFechaDeInicio.Text = String.Empty;
            textBoxFechaDeFin.Text = String.Empty;
            textBoxUniversidad.Text = String.Empty;
            textBoxGrado.Text = String.Empty;
            labelEscolaridadPos.Text = string.Empty;
            labelEscolaridadLength.Text = string.Empty;
        }

        private void buttonEscolaridadInserta_Click(object sender, EventArgs e)
        {
            bool universidad = textBoxUniversidad.Text != string.Empty;
            bool grado = textBoxGrado.Text != string.Empty;
            if (universidad && grado)
            {
                AccessUtility.InsertRegistroEscolaridad(textBoxID.Text, textBoxFechaDeInicio.Text, textBoxFechaDeFin.Text,
                textBoxUniversidad.Text, textBoxGrado.Text);
            }
            else
                MessageBox.Show("Debe proporcionarse al menos la información de la Universidad y el Grado");
            escolaridad = AccessUtility.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad();
        }

        private void buttonEscolaridadElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroEscolaridad(escolaridad[indexEscolaridad.Pos]["Id1"].ToString());
            escolaridad = AccessUtility.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad();
        }

        private void buttonAPInicial_Click(object sender, EventArgs e)
        {
            indexAP.Inicial();
            LlenaAP();
        }

        private void buttonAPPrevious_Click(object sender, EventArgs e)
        {
            indexAP.Previous();
            LlenaAP();
        }

        private void buttonAPSiguiente_Click(object sender, EventArgs e)
        {
            indexAP.Next();
            LlenaAP();
        }

        private void buttonAPFinal_Click(object sender, EventArgs e)
        {
            indexAP.Final();
            LlenaAP();
        }

        private void buttonAPInserta_Click(object sender, EventArgs e)
        {
            if (textBoxAPPartido.Text != string.Empty)
            {
                AccessUtility.InsertRegistroAP(textBoxAPID.Text, textBoxAPFechaDeInicio.Text, textBoxAPFechaDeFin.Text,
                textBoxAPPartido.Text);
            }
            else
                MessageBox.Show("Debe proporcionarse al menos la información del Partido de adscripción");
            AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP();
        }

        private void buttonAPLimpia_Click(object sender, EventArgs e)
        {
            // Adscripción Política

            textBoxAPFechaDeInicio.Text = String.Empty;
            textBoxAPFechaDeFin.Text = String.Empty;
            textBoxAPPartido.Text = String.Empty;
            labelAPPos.Text = string.Empty;
            labelAPLength.Text = string.Empty;
        }

        private void buttonAPElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroAP(AP[indexAP.Pos]["Id1"].ToString());
            AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP();
        }

        private void buttonINFOInicial_Click(object sender, EventArgs e)
        {
            indexINFO.Inicial();
            LlenaINFO();
        }

        private void buttonINFOPrevious_Click(object sender, EventArgs e)
        {
            indexINFO.Previous();
            LlenaINFO();
        }

        private void buttonINFOSiguiente_Click(object sender, EventArgs e)
        {
            indexINFO.Next();
            LlenaINFO();
        }

        private void buttonINFOFinal_Click(object sender, EventArgs e)
        {
            indexINFO.Final();
            LlenaINFO();
        }

        private void buttonINFOInserta_Click(object sender, EventArgs e)
        {
            bool tipoInfo = textBoxINFOTipoDeInformacion.Text != string.Empty;
            bool referencia = textBoxINFOReferencia.Text != String.Empty;
            if (tipoInfo && referencia)
            {
                AccessUtility.InsertRegistroINFO(textBoxINFOID.Text, textBoxINFOTipoDeInformacion.Text, textBoxINFOReferencia.Text);
            }
            else
                MessageBox.Show("Debe proporcionarse al menos la información del tipo de referencia y la referencia");
            INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO();
        }

        private void buttonINFOLimpia_Click(object sender, EventArgs e)
        {
            textBoxINFOTipoDeInformacion.Text = String.Empty;
            textBoxINFOReferencia.Multiline = true;
            textBoxINFOReferencia.Text = String.Empty;
            labelINFOPos.Text = string.Empty;
            labelINFOLength.Text = string.Empty;
        }

        private void buttonINFOElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroINFO(INFO[indexINFO.Pos]["Id1"].ToString());
            INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO();
        }

        private void buttonPuestosInicial_Click(object sender, EventArgs e)
        {
            indexPuestos.Inicial();
            LlenaPuestos();
        }

        private void buttonPuestosPrevious_Click(object sender, EventArgs e)
        {
            indexPuestos.Previous();
            LlenaPuestos();
        }

        private void buttonPuestosSiguiente_Click(object sender, EventArgs e)
        {
            indexPuestos.Next();
            LlenaPuestos();
        }

        private void buttonPuestosFinal_Click(object sender, EventArgs e)
        {
            indexPuestos.Final();
            LlenaPuestos();
        }

        private void buttonPuestosInserta_Click(object sender, EventArgs e)
        {
            bool dependencia = textBoxPuestosDependencia.Text != string.Empty;
            bool puesto = textBoxPuestosPuesto.Text != String.Empty;
            bool jefe = textBoxPuestosSuperior.Text != string.Empty;
            bool res = dependencia && puesto && jefe;
            if (res)
            {
                AccessUtility.InsertRegistroPuestos(textBoxPuestosID.Text, textBoxPuestosFechaDeInicio.Text,
                    textBoxPuestosFechaDeFin.Text, textBoxPuestosDependencia.Text, textBoxPuestosPuesto.Text,
                    textBoxPuestosSuperior.Text);
            }
            else
                MessageBox.Show("Debe proporcionarse la información de la dependencia/entidad, puesto y jefe inmediato");
            Puestos = AccessUtility.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos();
        }

        private void buttonPuestosLimpia_Click(object sender, EventArgs e)
        {
            textBoxPuestosDependencia.Text = String.Empty;
            textBoxPuestosPuesto.Text = String.Empty;
            textBoxPuestosSuperior.Text = String.Empty;
            textBoxPuestosFechaDeInicio.Text = String.Empty;
            textBoxPuestosFechaDeFin.Text = String.Empty;
            labelPuestosPos.Text = string.Empty;
            labelPuestosLength.Text = string.Empty;
        }

        private void buttonPuestosElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroPuestos(Puestos[indexPuestos.Pos]["Id1"].ToString());
            Puestos = AccessUtility.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialogFoto.ShowDialog();
            NewFotoFileName = openFileDialogFoto.FileName;
            LoadPhoto(NewFotoFileName);
        }

        private void buttonNuevo_Click(object sender, EventArgs e)
        {

        }
    }
}
