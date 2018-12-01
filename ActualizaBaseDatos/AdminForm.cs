using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MSAccess;
using Globales;
using System.Text.RegularExpressions;
using System.Drawing;
using CifradoPeta;
using System.Security.Policy;
using PetaPublish;

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
        
        bool DatosPersonalesModificados = false;
        DataRow[] funcionarios;
        DataRow[] escolaridad;
        DataRow[] AP;
        DataRow[] INFO;
        DataRow[] Puestos;

        string NewFotoFileName = string.Empty;

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

            tabControlAdministracionBaseDatos.Selected += TabControlAdministracionBaseDatos_Selected;
            tabControlInformación.Selected += TabControlInformación_Selected;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bDSecretarias1DataSet.Escolaridad' table. You can move, or remove it, as needed.
            funcionarios = AccessUtility.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            if (funcionarioMostrado.Length > 0)
            {
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
            }
            else
            {
                LimpiaInformación();
                buttonInserta.Enabled = true;
                buttonModifica.Enabled = false;
            }
        }

        private void AdminForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private string DespliegaInformación(int index)
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
            LlenaEscolaridad(IDFuncionario);

            AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP(IDFuncionario);

            INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO(IDFuncionario);

            Puestos = AccessUtility.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos(IDFuncionario);

            GetTabShown();
            return IDFuncionario;
        }

        private string GetNextUsableID()
        {
            if (funcionarios.Length == 0)
            {
                return "A0";
            }
            else
            {
                string llave = string.Empty;
                var myRegex = new Regex(@"[0-9]+");
                int Numero = 0;
                int Max = 0;

                for (int i = 0; i < funcionarios.Length; i++)
                {
                    llave = funcionarios[i]["ID"].ToString();
                    MatchCollection AllMatches = myRegex.Matches(llave);
                    if (AllMatches.Count > 0)
                    {
                        foreach (Match someMatch in AllMatches)
                        {   
                            Numero = Int32.Parse(someMatch.Groups[0].Value);
                            break;
                        }
                        Max = Numero > Max ? Numero : Max;
                    }
                    else
                        MessageBox.Show("Error en los ID de la base de datos");
                }
                return "A" + (Max + 1);
            }
        }

        private void LimpiaInformación()
        {
            // Datos Personales
            textBoxPrimerNombre.Text = string.Empty;
            textBoxSegundoNombre.Text = string.Empty;
            textBoxApellidoPaterno.Text = string.Empty;
            textBoxApellidoMaterno.Text = string.Empty;
            textBoxNacionalidad.Text = string.Empty;
            textBoxFechaNacimiento.Text = string.Empty;

            IDFuncionario = GetNextUsableID();

            using (var ms = new MemoryStream(Defines.ImagenDefault))
            {
                pictureBox1.Image = Image.FromStream(ms);
            }

            NewFotoFileName = string.Empty;

            LimpiaEscolaridad(IDFuncionario);

            LimpiaAP(IDFuncionario);
            
            LimpiaINFO(IDFuncionario);
            
            LimpiaPuestos(IDFuncionario);

            GetTabShown();
        }

        private bool AcceptedFileType(string PhotoFileName)
        {
            if (PhotoFileName != string.Empty)
            {
                switch (Path.GetExtension(PhotoFileName).ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                    case ".bmp":
                    case ".wmf":
                    case ".png":
                        if (File.Exists(PhotoFileName))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        
                    default:
                        return false;
                }
            }
            else
                return false;
        }

        private void LoadPhoto(string PhotoFileName)
        {
            if (AcceptedFileType(PhotoFileName))
            {
                pictureBox1.ImageLocation = PhotoFileName;
                pictureBox1.Load();
                // ancho 172 alto 199
                float prop = (float)pictureBox1.Width / (float)pictureBox1.Height;
                pictureBox1.Height = 199;
                pictureBox1.Width = (int)(prop * 199);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
                MessageBox.Show("Problema con el archivo " + PhotoFileName);
        }

        private void LlenaEscolaridad(string ID)
        {
            // Escolaridad
            if (indexEscolaridad.Length == 0)
            {
                LimpiaEscolaridad(ID);
            }
            else
            {
                textBoxID.Text = escolaridad[indexEscolaridad.Pos]["ID"].ToString();
                textBoxFechaDeInicio.Text = escolaridad[indexEscolaridad.Pos]["FechaDeInicio"].ToString();
                textBoxFechaDeFin.Text = escolaridad[indexEscolaridad.Pos]["FechaDeFin"].ToString();
                textBoxUniversidad.Text = escolaridad[indexEscolaridad.Pos]["Universidad"].ToString();
                textBoxGrado.Text = escolaridad[indexEscolaridad.Pos]["Grado"].ToString();
                labelEscolaridadPos.Text = (indexEscolaridad.Pos + 1).ToString();
                labelEscolaridadLength.Text = "de " + indexEscolaridad.Length.ToString();
            }
        }

        private void LimpiaEscolaridad(string ID)
        {
            textBoxID.Text = ID;
            textBoxFechaDeInicio.Text = string.Empty;
            textBoxFechaDeFin.Text = string.Empty;
            textBoxUniversidad.Text = string.Empty;
            textBoxGrado.Text = string.Empty;
            labelEscolaridadPos.Text = string.Empty;
            labelEscolaridadLength.Text = string.Empty;
        }
        
        private void LlenaAP(string ID)
        {
            // Adscripción Política
            if (indexAP.Length == 0)
            {
                LimpiaAP(ID);
            }
            else
            {
                textBoxAPID.Text = AP[indexAP.Pos]["ID"].ToString();
                textBoxAPFechaDeInicio.Text = AP[indexAP.Pos]["FechaDeInicio"].ToString();
                textBoxAPFechaDeFin.Text = AP[indexAP.Pos]["FechaDeFin"].ToString();
                textBoxAPPartido.Text = AP[indexAP.Pos]["NombreDelPartido"].ToString();
                labelAPPos.Text = (indexAP.Pos + 1).ToString();
                labelAPLength.Text = "de " + indexAP.Length.ToString();
            }
        }

        private void LimpiaAP(string ID)
        {
            textBoxAPID.Text = ID;
            textBoxAPFechaDeInicio.Text = string.Empty;
            textBoxAPFechaDeFin.Text = string.Empty;
            textBoxAPPartido.Text = string.Empty;
            labelAPPos.Text = string.Empty;
            labelAPLength.Text = string.Empty;
        }

        private void LlenaINFO(string ID)
        {
            // Información General
            if (indexINFO.Length == 0)
            {
                LimpiaINFO(ID);
            }
            else
            {
                textBoxINFOID.Text = INFO[indexINFO.Pos]["ID"].ToString();
                textBoxINFOTipoDeInformacion.Text = INFO[indexINFO.Pos]["TipoDeInformación"].ToString();
                textBoxINFOReferencia.Multiline = true;
                textBoxINFOReferencia.Text = INFO[indexINFO.Pos]["Referencia"].ToString();
                labelINFOPos.Text = (indexINFO.Pos + 1).ToString();
                labelINFOLength.Text = "de " + indexINFO.Length.ToString();
            }
        }

        private void LimpiaINFO(string ID)
        {
            textBoxINFOID.Text = ID;
            textBoxINFOTipoDeInformacion.Text = string.Empty;
            textBoxINFOReferencia.Multiline = true;
            textBoxINFOReferencia.Text = string.Empty;
            labelINFOPos.Text = string.Empty;
            labelINFOLength.Text = string.Empty;
        }

        private void LlenaPuestos(string ID)
        {
            // Puestos
            if (indexPuestos.Length == 0)
            {
                LimpiaPuestos(ID);
            }
            else
            {
                textBoxPuestosID.Text = Puestos[indexPuestos.Pos]["ID"].ToString();
                textBoxPuestosDependencia.Text = Puestos[indexPuestos.Pos]["DependenciaEntidad"].ToString();
                textBoxPuestosPuesto.Text = Puestos[indexPuestos.Pos]["Puesto"].ToString();
                textBoxPuestosSuperior.Text = Puestos[indexPuestos.Pos]["JefeInmediantoSuperior"].ToString();
                textBoxPuestosFechaDeInicio.Text = Puestos[indexPuestos.Pos]["FechaDeInicio"].ToString();
                textBoxPuestosFechaDeFin.Text = Puestos[indexPuestos.Pos]["FechaDeFin"].ToString();
                labelPuestosPos.Text = (indexPuestos.Pos + 1).ToString();
                labelPuestosLength.Text = "de " + indexPuestos.Length.ToString();
            }
        }

        private void LimpiaPuestos(string ID)
        {
            textBoxPuestosID.Text = ID;
            textBoxPuestosDependencia.Text = string.Empty;
            textBoxPuestosPuesto.Text = string.Empty;
            textBoxPuestosSuperior.Text = string.Empty;
            textBoxPuestosFechaDeInicio.Text = string.Empty;
            textBoxPuestosFechaDeFin.Text = string.Empty;
            labelPuestosPos.Text = string.Empty;
            labelPuestosLength.Text = string.Empty;
        }

        private void TabControlInformación_Selected(object sender, TabControlEventArgs e)
        {
            GetTabShown();
            switch (TabPageInformación.Text)
            {
                case "Escolaridad":
                    LlenaEscolaridad(IDFuncionario);
                    break;
                case "Adscripción Política":
                    LlenaAP(IDFuncionario);
                    break;
                case "Información General":
                    LlenaINFO(IDFuncionario);
                    break;
                case "Puestos":
                    LlenaPuestos(IDFuncionario);
                    break;
                default:
                    break;
            }
            tabControlInformación.Focus();
        }

        private void TabControlAdministracionBaseDatos_Selected(object sender, TabControlEventArgs e)
        {
            GetTabShown();
            switch (TabPageAdministración.Text)
            {
                case "Fichas":
                case "Organigrama APF":
                    break;
                case "Publica Info":
                    PetaSecure cipher = new PetaSecure();
                    labelSTATUS.Text = "Trabajando";
                    cipher.FileEncrypt(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileNameEncriptado,
                        System.Text.Encoding.UTF8.GetString(Defines.ImagenDefault).Substring(Defines.PosInicial,Defines.PosFinal));
                    PetaPublish.Publisher.UploadInfoAPFDB(Defines.DataBasePath, Defines.DataBaseFileNameEncriptado);
                    labelSTATUS.Text = "Terminado";
                    break;
                default:
                    break;
            }
        }
        
        private void buttonBusca_Click(object sender, EventArgs e)
        {

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
            if (funcionarioMostrado.Length > 0)
            {
                funcionarioMostrado.Previous();
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
            }
            else
            {
                buttonInserta.Enabled = true;
                buttonModifica.Enabled = false;
            }
        }

        private void buttonSiguiente_Click(object sender, EventArgs e)
        {
            if (funcionarios.Length > 0)
            {
                funcionarioMostrado.Next();
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
            }
            else
            {
                buttonInserta.Enabled = true;
                buttonModifica.Enabled = false;
            }
        }

        private void buttonEscolaridadInicial_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Inicial();
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadSiguiente_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Next();
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadPrevio_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Previous();
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadFinal_Click(object sender, EventArgs e)
        {
            indexEscolaridad.Final();
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadLimpia_Click(object sender, EventArgs e)
        {
            LimpiaEscolaridad(IDFuncionario);
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
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroEscolaridad(escolaridad[indexEscolaridad.Pos]["Id1"].ToString());
            escolaridad = AccessUtility.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonAPInicial_Click(object sender, EventArgs e)
        {
            indexAP.Inicial();
            LlenaAP(IDFuncionario);
        }

        private void buttonAPPrevious_Click(object sender, EventArgs e)
        {
            indexAP.Previous();
            LlenaAP(IDFuncionario);
        }

        private void buttonAPSiguiente_Click(object sender, EventArgs e)
        {
            indexAP.Next();
            LlenaAP(IDFuncionario);
        }

        private void buttonAPFinal_Click(object sender, EventArgs e)
        {
            indexAP.Final();
            LlenaAP(IDFuncionario);
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
            LlenaAP(IDFuncionario);
        }

        private void buttonAPLimpia_Click(object sender, EventArgs e)
        {
            LimpiaAP(IDFuncionario);
        }

        private void buttonAPElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroAP(AP[indexAP.Pos]["Id1"].ToString());
            AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP(IDFuncionario);
        }

        private void buttonINFOInicial_Click(object sender, EventArgs e)
        {
            indexINFO.Inicial();
            LlenaINFO(IDFuncionario);
        }

        private void buttonINFOPrevious_Click(object sender, EventArgs e)
        {
            indexINFO.Previous();
            LlenaINFO(IDFuncionario);
        }

        private void buttonINFOSiguiente_Click(object sender, EventArgs e)
        {
            indexINFO.Next();
            LlenaINFO(IDFuncionario);
        }

        private void buttonINFOFinal_Click(object sender, EventArgs e)
        {
            indexINFO.Final();
            LlenaINFO(IDFuncionario);
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
            LlenaINFO(IDFuncionario);
        }

        private void buttonINFOLimpia_Click(object sender, EventArgs e)
        {
            LimpiaINFO(IDFuncionario);
        }

        private void buttonINFOElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroINFO(INFO[indexINFO.Pos]["Id1"].ToString());
            INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO(IDFuncionario);
        }

        private void buttonPuestosInicial_Click(object sender, EventArgs e)
        {
            indexPuestos.Inicial();
            LlenaPuestos(IDFuncionario);
        }

        private void buttonPuestosPrevious_Click(object sender, EventArgs e)
        {
            indexPuestos.Previous();
            LlenaPuestos(IDFuncionario);
        }

        private void buttonPuestosSiguiente_Click(object sender, EventArgs e)
        {
            indexPuestos.Next();
            LlenaPuestos(IDFuncionario);
        }

        private void buttonPuestosFinal_Click(object sender, EventArgs e)
        {
            indexPuestos.Final();
            LlenaPuestos(IDFuncionario);
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
            LlenaPuestos(IDFuncionario);
        }

        private void buttonPuestosLimpia_Click(object sender, EventArgs e)
        {
            LimpiaPuestos(IDFuncionario);
        }

        private void buttonPuestosElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroPuestos(Puestos[indexPuestos.Pos]["Id1"].ToString());
            Puestos = AccessUtility.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos(IDFuncionario);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialogFoto.ShowDialog();
            NewFotoFileName = openFileDialogFoto.FileName;
            LoadPhoto(NewFotoFileName);
            DatosPersonalesModificados = true;
        }

        private void buttonNuevo_Click(object sender, EventArgs e)
        {
            LimpiaInformación();
            buttonInserta.Enabled = true;
        }

        private void textBoxPrimerNombre_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void textBoxSegundoNombre_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void textBoxApellidoPaterno_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void textBoxApellidoMaterno_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void textBoxNacionalidad_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void textBoxFechaNacimiento_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
        }

        private void buttonInserta_Click(object sender, EventArgs e)
        {
            string ID = GetNextUsableID();
            AccessUtility.InsertFuncionario(ID, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text, textBoxFechaNacimiento.Text);
            AccessUtility.SubeFoto(ID, NewFotoFileName);
            buttonInserta.Enabled = false;
            buttonModifica.Enabled = true;
        }

        private void buttonModifica_Click(object sender, EventArgs e)
        {
            AccessUtility.UpdateFuncionario(textBoxID.Text, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text, textBoxFechaNacimiento.Text);
            AccessUtility.SubeFoto(textBoxID.Text, NewFotoFileName);
            buttonModifica.Enabled = false;
        }
    }
}
