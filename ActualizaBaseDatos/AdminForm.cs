using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MSAccess;
using Globales;
using System.Text.RegularExpressions;
using System.Drawing;
using CifradoPeta;
using OrganigramaAdmin;
using Arboles;
using APFInfo;
using System.Collections.Generic;
using FuncionesAuxiliares;

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
        bool FotoModificada = false;
        DataRow[] funcionarios;
        DataRow[] escolaridad;
        DataRow[] AP;
        DataRow[] INFO;
        DataRow[] Puestos;

        string NewFotoFileName = string.Empty;

        bool BusquedaEnProceso = false;

        public Node<Registro> APF;
        Dictionary<string, Node<Registro>> ListaDeNodosPorID = new Dictionary<string, Node<Registro>>();
        public Node<Registro> nodoSeleccionado = null;
        TreeNode NodoDeArbolMostrado = null;
        Parser p;

        System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();

        public Node<Registro> NodoSeleccionado
        {
            get
            {
                if (nodoSeleccionado == null)
                {
                    t.ToString();
                }
                return nodoSeleccionado;
            }
            set
            {
                t.ToString();
                nodoSeleccionado = value;
            }
        }

        Dictionary<string, string> NivelSIguiente = new Dictionary<string, string>();

        public AdminForm()
        {

            string[] PhotoFiles = Directory.GetFiles(Defines.FotoTempBasePath, "*.*");

            for (int i = 0; i < PhotoFiles.Length; i++)
            {
                FileInfo fi = new FileInfo(PhotoFiles[i]);
                fi.Delete();
            }
            p = new Parser(ImprimeConsola);
            InitializeComponent();
            this.FormClosed += AdminForm_FormClosed;

            tabControlAdministracionBaseDatos.Selected += TabControlAdministracionBaseDatos_Selected;
            tabControlInformación.Selected += TabControlInformación_Selected;
            treeViewOrganigramaAPF.ItemDrag += TreeViewOrganigramaAPF_ItemDrag;
            treeViewOrganigramaAPF.DragEnter += TreeViewOrganigramaAPF_DragEnter;
            treeViewOrganigramaAPF.DragDrop += TreeViewOrganigramaAPF_DragDrop;
            NivelSIguiente.Add("Presidencia", "S");
            NivelSIguiente.Add("S", "SS");
            NivelSIguiente.Add("SS", "DG");
            NivelSIguiente.Add("DG", "Dir");
            NivelSIguiente.Add("Dir", "nadie");
            checkedListBoxTipoINFO.ItemCheck += checkedListBoxTipoINFO_ItemCheck;
            checkedListBoxTipoInformacion.ItemCheck += CheckedListBoxTipoInformacion_ItemCheck;
        }

        private void TreeViewOrganigramaAPF_DragDrop(object sender, DragEventArgs e)
        {
            Node<Registro> APFDraggedNode = null;
            Node<Registro> APFTargetNode = null;
            TreeNode targetNode = null;
            TreeNode draggedNode;
            // Retrieve the client coordinates of the drop location.
            Point targetPoint = treeViewOrganigramaAPF.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            targetNode = treeViewOrganigramaAPF.GetNodeAt(targetPoint);
            int TargetPos = targetNode.Text.IndexOf('_');
            string TargetID = targetNode.Text.Substring(TargetPos + 1, targetNode.Text.Length - TargetPos - 1);
            if (ListaDeNodosPorID.ContainsKey(TargetID))
            {
                APFTargetNode = ListaDeNodosPorID[TargetID];
                // Retrieve the node that was dragged.
                draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                int pos = draggedNode.Text.IndexOf('_');
                string ID = draggedNode.Text.Substring(pos + 1, draggedNode.Text.Length - pos - 1);
                if (ListaDeNodosPorID.ContainsKey(ID))
                {
                    APFDraggedNode = ListaDeNodosPorID[ID];
                    // NodoDeArbolMostrado es draggedNode
                    // Confirm that the node at the drop location is not 
                    // the dragged node and that target node isn't null
                    // (for example if you drag outside the control)
                    if (!draggedNode.Equals(targetNode) && targetNode != null)
                    {
                        // Hay que quitar el nodo y todos sus hijos
                        if (APFDraggedNode.Padre != null)
                        {
                            APFDraggedNode.Padre.Sons.Remove(APFDraggedNode);
                            LimpiaListaDeNodosPorID(APFDraggedNode);
                            //quitamos el nodo del TreeView mostrado
                            draggedNode.Remove();
                            // ----------------------------------
                            int NivelDelJefe;
                            if ((NivelDelJefe = NivelGerarquico(APFTargetNode)) == -1)
                            {
                                MessageBox.Show("Error de nivel de puesto");
                            }
                            else
                            {
                                // Ponemos el nodo en su nuevo lugar
                                APFDraggedNode.Padre = APFTargetNode;
                                ActualizaTipoDePuestoYPoda(APFDraggedNode, NivelDelJefe);
                                // Insertamos en el arbol de la APF
                                APFTargetNode.Sons.InsertaHijo(APFDraggedNode);
                                LlenaListaDeNodosPorID(APFDraggedNode);
                                // Hay que mostrar el nuevo subarbol en el TreeView
                                // Actualizamos el TreeView
                                targetNode.Nodes.Add(draggedNode);
                                // Expand the node at the location 
                                // to show the dropped node.
                                targetNode.Expand();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se puede mover la Presidencia");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error grave comunicarse con el desarrollador Drag & Drop draggedNode");
                }
            }
            else
            {
                MessageBox.Show("Error en el nodo de destino");
            }
        }

        private void TreeViewOrganigramaAPF_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeViewOrganigramaAPF_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private int ImprimeConsola(string line)
        {
            Console.WriteLine(line);
            return 0;
        }

        Organigrama organigrama = new Organigrama();
        TreeNode RaizTreeView = null;

        private void InicializaDS()
        {
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0");
            APF = new Node<Registro>(Presidente);
            ListaDeNodosPorID.Add("A0", APF);
            p.Parsea(APF, 0, ListaDeNodosPorID);

            funcionarios = AccessUtility.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            if (funcionarioMostrado.Length > 0)
            {
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                BusquedaEnProceso = false;
            }
            else
            {
                LimpiaInformación();
                buttonInserta.Enabled = true;
                buttonModifica.Enabled = false;
                BusquedaEnProceso = false;
            }
            organigrama.LlenaTreeAPF(treeViewOrganigramaAPF.Nodes, APF, 0, true, ref RaizTreeView);
            //organigrama.PrintTreeAPF(APF, ImprimeConsola);
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            InicializaDS();
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
            muestraCapturaFechaNacimiento.SetFecha(IntParse.Numero(funcionarios[index]["AñoNacimiento"].ToString()),
                IntParse.Numero(funcionarios[index]["MesNacimiento"].ToString()),
                IntParse.Numero(funcionarios[index]["DiaNacimiento"].ToString()));

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
            DatosPersonalesModificados = false;
            FotoModificada = false;
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
            muestraCapturaFechaNacimiento.SetFecha(1900, 1, 1);

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
            BusquedaEnProceso = false;
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
                if (pictureBox1.Width > 172)
                {
                    pictureBox1.Width = 172;
                    pictureBox1.Height = (int)(172 / prop);
                }
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
                muestraCapturaFechaInicio.SetFecha(IntParse.Numero(escolaridad[indexEscolaridad.Pos]["AñoInicial"].ToString()),
                    IntParse.Numero(escolaridad[indexEscolaridad.Pos]["MesInicial"].ToString()),
                    IntParse.Numero(escolaridad[indexEscolaridad.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaFin.SetFecha(IntParse.Numero(escolaridad[indexEscolaridad.Pos]["AñoFinal"].ToString()),
                    IntParse.Numero(escolaridad[indexEscolaridad.Pos]["MesFinal"].ToString()),
                    IntParse.Numero(escolaridad[indexEscolaridad.Pos]["DiaFinal"].ToString()));
                textBoxUniversidad.Text = escolaridad[indexEscolaridad.Pos]["Universidad"].ToString();
                textBoxGrado.Text = escolaridad[indexEscolaridad.Pos]["Grado"].ToString();
                labelEscolaridadPos.Text = (indexEscolaridad.Pos + 1).ToString();
                labelEscolaridadLength.Text = "de " + indexEscolaridad.Length.ToString();
            }
        }

        private void LimpiaEscolaridad(string ID)
        {
            textBoxID.Text = ID;
            muestraCapturaFechaInicio.SetFecha(1900, 1, 1);
            muestraCapturaFechaFin.SetFecha(1900, 1, 1);
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
                // adscripción politica solo tiene año no maneja mes y día
                muestraCapturaFechaAPInicio.SetFecha(IntParse.Numero(AP[indexAP.Pos]["FechaDeInicio"].ToString()), 1, 1);
                muestraCapturaFechaAPFin.SetFecha(IntParse.Numero(AP[indexAP.Pos]["FechaDeFin"].ToString()), 1, 1);
                textBoxAPPartido.Text = AP[indexAP.Pos]["NombreDelPartido"].ToString();
                labelAPPos.Text = (indexAP.Pos + 1).ToString();
                labelAPLength.Text = "de " + indexAP.Length.ToString();
            }
        }

        private void LimpiaAP(string ID)
        {
            textBoxAPID.Text = ID;
            muestraCapturaFechaAPInicio.SetFecha(1900, 1, 1);
            muestraCapturaFechaAPFin.SetFecha(1900, 1, 1);
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
                muestraCapturaFechaPuestoInicio.SetFecha(IntParse.Numero(Puestos[indexPuestos.Pos]["AñoInicial"].ToString()),
                    IntParse.Numero(Puestos[indexPuestos.Pos]["MesInicial"].ToString()),
                    IntParse.Numero(Puestos[indexPuestos.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaPuestoFin.SetFecha(IntParse.Numero(Puestos[indexPuestos.Pos]["AñoFinal"].ToString()),
                    IntParse.Numero(Puestos[indexPuestos.Pos]["MesFinal"].ToString()),
                    IntParse.Numero(Puestos[indexPuestos.Pos]["DiaFinal"].ToString()));

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
            muestraCapturaFechaPuestoInicio.SetFecha(1900, 1, 1);
            muestraCapturaFechaPuestoFin.SetFecha(1900, 1, 1);
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
                    NodoSeleccionado = null;
                    break;
                case "Publica Info":
                    PetaSecure cipher = new PetaSecure();
                    labelSTATUS.Text = "Trabajando";
                    cipher.FileEncrypt(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileNameEncriptado,
                        System.Text.Encoding.UTF8.GetString(Defines.ImagenDefault).Substring(Defines.PosInicial, Defines.PosFinal));
                    PetaPublish.Publisher.UploadInfoAPFDB(Defines.DataBasePath, Defines.DataBaseFileNameEncriptado);
                    labelSTATUS.Text = "Terminado";
                    break;
                default:
                    break;
            }
        }

        private string SinA(string Cadena)
        {
            return Cadena.ToLower().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u")
                .Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
        }

        Busqueda BusquedaActiva;

        private void buttonBusca_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (BusquedaEnProceso)
            {
                i = funcionarioMostrado.Pos + 1;
            }
            else // nueva busqueda
            {
                BusquedaActiva = new Busqueda();
                DatosPersonalesModificados = false;
                FotoModificada = false;
                if (!SinA(textBoxApellidoPaterno.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoPaterno.Text);
                if (!SinA(textBoxApellidoMaterno.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoMaterno.Text);
                if (!SinA(textBoxPrimerNombre.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxPrimerNombre.Text);
                if (!SinA(textBoxSegundoNombre.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxSegundoNombre.Text);
                if (!SinA(textBoxNacionalidad.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxNacionalidad.Text);
                //if (!SinA(textBoxFechaNacimiento.Text).Equals(string.Empty)) BusquedaActiva.Add(textBoxFechaNacimiento.Text);
            }
            while (i < funcionarioMostrado.Length &&
                    !BusquedaActiva.SatisfaceCriterio(SinA(funcionarios[i]["PrimerNombre"].ToString()) + " " +
                        SinA(funcionarios[i]["SegundoNombre"].ToString()) + " " +
                        SinA(funcionarios[i]["ApellidoPaterno"].ToString()) + " " +
                        SinA(funcionarios[i]["ApellidoMaterno"].ToString()) + " " +
                        SinA(funcionarios[i]["Nacionalidad"].ToString()) + " " +
                        SinA(funcionarios[i]["AñoNacimiento"].ToString()))) i++;
            if (i < funcionarioMostrado.Length)
            {
                funcionarioMostrado.Pos = i;
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                BusquedaEnProceso = true;
            }
            else
            {
                BusquedaEnProceso = false;
                MessageBox.Show("No se encontró un registro con los datos señalados");
            }
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
            BusquedaEnProceso = false;
            if (DatosPersonalesModificados)
            {
                if (MessageBox.Show("¿Desea ignorar las modificaciones hechas a la ficha?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // YES
                    if (funcionarioMostrado.Length > 0)
                    {
                        funcionarioMostrado.Previous();
                        DespliegaInformación(funcionarioMostrado.Pos);
                        buttonInserta.Enabled = false;
                        buttonModifica.Enabled = true;
                        DatosPersonalesModificados = false;
                        FotoModificada = false;
                    }
                    else
                    {
                        LimpiaInformación();
                        buttonInserta.Enabled = true;
                        buttonModifica.Enabled = false;
                    }
                    DatosPersonalesModificados = false;
                    FotoModificada = false;
                }
                else
                {
                    // NO
                }
            }
            else
            {
                if (funcionarioMostrado.Length > 0)
                {
                    funcionarioMostrado.Previous();
                    DespliegaInformación(funcionarioMostrado.Pos);
                    buttonInserta.Enabled = false;
                    buttonModifica.Enabled = true;
                    DatosPersonalesModificados = false;
                    FotoModificada = false;
                }
                else
                {
                    LimpiaInformación();
                    buttonInserta.Enabled = true;
                    buttonModifica.Enabled = false;
                }
            }
        }

        private void buttonSiguiente_Click(object sender, EventArgs e)
        {
            BusquedaEnProceso = false;
            if (DatosPersonalesModificados)
            {
                if (MessageBox.Show("¿Desea ignorar las modificaciones hechas a la ficha?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // YES
                    if (funcionarioMostrado.Length > 0)
                    {
                        funcionarioMostrado.Next();
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
                    DatosPersonalesModificados = false;
                    FotoModificada = false;
                }
                else
                {
                    // NO
                }
            }
            else
            {
                if (funcionarioMostrado.Length > 0)
                {
                    funcionarioMostrado.Next();
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
                DatosPersonalesModificados = false;
                FotoModificada = false;
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
                AccessUtility.InsertRegistroEscolaridad(textBoxID.Text, muestraCapturaFechaInicio.StringFecha, muestraCapturaFechaFin.StringFecha,
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
                AccessUtility.InsertRegistroAP(textBoxAPID.Text, muestraCapturaFechaAPInicio.StringFecha, muestraCapturaFechaAPFin.StringFecha,
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
                AccessUtility.InsertRegistroPuestos(textBoxPuestosID.Text, muestraCapturaFechaPuestoInicio.StringFecha,
                    muestraCapturaFechaPuestoFin.StringFecha, textBoxPuestosDependencia.Text, textBoxPuestosPuesto.Text,
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
            if (File.Exists(openFileDialogFoto.FileName) && AcceptedFileType(openFileDialogFoto.FileName))
            {
                NewFotoFileName = openFileDialogFoto.FileName;
                LoadPhoto(NewFotoFileName);
                DatosPersonalesModificados = true;
                FotoModificada = true;
            }
        }

        private void buttonNuevo_Click(object sender, EventArgs e)
        {
            if (DatosPersonalesModificados)
            {
                if (MessageBox.Show("¿Desea ignorar las modificaciones hechas a la ficha?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    LimpiaInformación();
                    buttonInserta.Enabled = true;
                    DatosPersonalesModificados = false;
                    FotoModificada = false;
                    BusquedaEnProceso = false;
                }
                else
                {

                }
            }
            else
            {
                LimpiaInformación();
                buttonInserta.Enabled = true;
                DatosPersonalesModificados = false;
                FotoModificada = false;
                BusquedaEnProceso = false;
            }
        }

        private void textBoxPrimerNombre_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void textBoxSegundoNombre_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void textBoxApellidoPaterno_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void textBoxApellidoMaterno_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void textBoxNacionalidad_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void textBoxFechaNacimiento_TextChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
            //textBoxFechaNacimiento.Text = FormatoFecha.FechaString(fecha.Año.ToString(), fecha.Mes.ToString(), fecha.Dia.ToString(), "Fecha: no disponible");
        }

        private void buttonInserta_Click(object sender, EventArgs e)
        {
            if (!NewFotoFileName.Equals(string.Empty))
            {
                string ID = GetNextUsableID();
                AccessUtility.InsertFuncionario(ID, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text, muestraCapturaFechaNacimiento.StringFecha);
                AccessUtility.SubeFoto(ID, NewFotoFileName);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                DatosPersonalesModificados = false;
                FotoModificada = false;
                BusquedaEnProceso = false;
            }
            else
            {
                MessageBox.Show("No se puede insertar una ficha sin foto");
            }
        }

        private void buttonModifica_Click(object sender, EventArgs e)
        {
            AccessUtility.UpdateFuncionario(textBoxID.Text, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text, muestraCapturaFechaNacimiento.StringFecha);
            if (FotoModificada) AccessUtility.SubeFoto(textBoxID.Text, NewFotoFileName);
            buttonModifica.Enabled = false;
            DatosPersonalesModificados = false;
            FotoModificada = false;
            BusquedaEnProceso = false;
        }

        private void buttonCargaBD_Click(object sender, EventArgs e)
        {
            funcionarios = AccessUtility.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            if (funcionarioMostrado.Length > 0)
            {
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                BusquedaEnProceso = false;
            }
            else
            {
                LimpiaInformación();
                buttonInserta.Enabled = true;
                buttonModifica.Enabled = false;
                BusquedaEnProceso = false;
            }
            DatosPersonalesModificados = false;
            FotoModificada = false;
        }

        private void treeViewOrganigramaAPF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            if (ListaDeNodosPorID.ContainsKey(ID))
            {
                NodoSeleccionado = ListaDeNodosPorID[ID];
                NodoDeArbolMostrado = treeViewOrganigramaAPF.SelectedNode;
                //ImprimeConsola("-->  " + NodoSeleccionado.Data.ToString() + " # " + e.Node.Text);
            }
            else
            {
                MessageBox.Show("Error grave comunicarse con el desarrollador");
            }
            if (NodoSeleccionado == null) MessageBox.Show("Error error");
        }

        private void buttonOrgMuestraFuncionario_Click(object sender, EventArgs e)
        {
            String ID = NodoSeleccionado.Data.ID;
            int j = 0;
            for (int i = 0; i < funcionarios.Length; i++)
            {
                if (funcionarios[i]["ID"].ToString().Equals(ID))
                {
                    j = i;
                    break;
                }
            }
            if (j < funcionarios.Length)
            {
                funcionarioMostrado.Pos = j;
                DespliegaInformación(funcionarioMostrado.Pos);
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                BusquedaEnProceso = true;
                tabControlAdministracionBaseDatos.SelectedTab = tabPageFichas;
            }
            else
            {
                MessageBox.Show("Error muy importante comunicarse con el desarrollador");
            }
            treeViewOrganigramaAPF.SelectedNode = null;
            NodoDeArbolMostrado = null;
            NodoSeleccionado = null;
        }

        // Modifica
        private void buttonOrgActualizaFuncionario_Click(object sender, EventArgs e)
        {
            String IDMostrado = NodoSeleccionado.Data.ID;
            string ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
            if (!(NodoSeleccionado == null) && !ListaDeNodosPorID.ContainsKey(IDMostrado))
            {
                NodoDeArbolMostrado.Text = textBoxOrgNombrePuestoModificado.Text + " - " + AccessUtility.GetNombreFuncionario(ID) + "_" + ID;
                Registro NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, textBoxOrgNombrePuestoModificado.Text, ID);
                MessageBox.Show(NuevoRegistro.ToString());
                Node<Registro> NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                ListaDeNodosPorID.Add(ID, NuevoNode);
                // tengo que actualizar APF
                ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                ListaDeNodosPorID.Remove(IDMostrado);
                treeViewOrganigramaAPF.SelectedNode = null;
                NodoDeArbolMostrado = null;
                NodoSeleccionado = null;
                organigrama.PrintTreeAPF(APF, ImprimeConsola);
                textBoxOrgNombrePuestoModificado.Text = string.Empty;
            }
        }

        // Inserta
        private void buttonOrgInsertaPuesto_Click(object sender, EventArgs e)
        {
            string ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
            if (NodoSeleccionado == null)
            {
                MessageBox.Show("Nodo Selecccionado es nulo");
            }
            if (!textBoxOrgNombrePuesto.Text.Equals(string.Empty) && !ListaDeNodosPorID.ContainsKey(ID) && !(NodoSeleccionado == null))
            {
                // Hay que modificar APF y ListadeNodosPorID
                // siempre vamos a inserta el nuevo registro como un hijo, y el único nodo que no puedo borrar es el raiz (el presidente)
                NodeList<Registro> SinHijos = new NodeList<Registro>();
                Registro NuevoRegistro = new Registro(NivelSIguiente[NodoSeleccionado.Data.TipoRegistro], textBoxOrgNombrePuesto.Text, ID);
                Node<Registro> NuevoPuesto = new Node<Registro>(NuevoRegistro, SinHijos, NodoSeleccionado);
                ListaDeNodosPorID.Add(ID, NuevoPuesto);
                //ImprimeConsola("+-> Contenido nodo insertado: " + NodoSeleccionado.Data.ToString());
                NodoSeleccionado.Sons.InsertaHijo(NuevoPuesto);
                // Actualizamos el TreeView
                TreeNode newNode = new TreeNode(textBoxOrgNombrePuesto.Text + " - " + AccessUtility.GetNombreFuncionario(ID) + "_" + ID);
                NodoDeArbolMostrado.Nodes.Add(newNode);
                NodoDeArbolMostrado.Expand();

                treeViewOrganigramaAPF.SelectedNode = null;
                NodoDeArbolMostrado = null;
                NodoSeleccionado = null;
                organigrama.PrintTreeAPF(APF, ImprimeConsola);
                textBoxOrgNombrePuesto.Text = string.Empty;
            }
        }

        private void buttonPrintTree_Click(object sender, EventArgs e)
        {
            ImprimeConsola("=================================");
            organigrama.PrintTreeAPF(APF, ImprimeConsola);
            ImprimeConsola(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><");
        }

        private void buttonListaDeNodosPorID_Click(object sender, EventArgs e)
        {
            ImprimeConsola("-------------------------------------------------");
            ImprimeConsola("Tamaño de la lista: " + ListaDeNodosPorID.Count);
            foreach (var entry in ListaDeNodosPorID)
            {
                ImprimeConsola(entry.Key + " -- " + entry.Value.Data.ToString());
            }
            ImprimeConsola("///////////////////////////////////////////////////////");
        }

        private void buttonOrgBorraEntrada_Click(object sender, EventArgs e)
        {

        }

        Node<Registro> NodoCortado = null;

        public void LimpiaListaDeNodosPorID(Node<Registro> raiz)
        {
            ListaDeNodosPorID.Remove(raiz.Data.ID);
            foreach (Node<Registro> nodo in raiz.Sons)
            {
                ListaDeNodosPorID.Remove(nodo.Data.ID);
            }
        }

        public void LlenaListaDeNodosPorID(Node<Registro> raiz)
        {
            ListaDeNodosPorID.Add(raiz.Data.ID, raiz);
            foreach (Node<Registro> nodo in raiz.Sons)
            {
                ListaDeNodosPorID.Add(nodo.Data.ID, nodo);
            }
        }

        private void buttonOrgCortar_Click(object sender, EventArgs e)
        {
            String IDMostrado = NodoSeleccionado.Data.ID;
            if (!(NodoSeleccionado == null) && ListaDeNodosPorID.ContainsKey(IDMostrado))
            {
                // quitamos el nodo del arbol de la APF
                NodoCortado = NodoSeleccionado;
                // Hay que quitar el nodo y todos sus hijos
                if (NodoSeleccionado.Padre != null)
                {
                    NodoSeleccionado.Padre.Sons.Remove(NodoSeleccionado);
                    LimpiaListaDeNodosPorID(NodoSeleccionado);
                    //quitamos el nodo del TreeView mostrado
                    treeViewOrganigramaAPF.Nodes.Remove(treeViewOrganigramaAPF.SelectedNode);
                }
                else
                {
                    MessageBox.Show("No se puede remover la Presidencia");
                }
            }
            else
                MessageBox.Show("Se debe seleccionar el puesto que se va a cortar");
        }

        private int NivelGerarquico(Node<Registro> nodo)
        {
            //ImprimeConsola("--->  " + nodo.Data.TipoRegistro);
            for (int i = 0; i < p.pila.Length; i++)
            {
                if (p.pila[i].Equals(nodo.Data.TipoRegistro)) return i;
            }
            return -1;
        }

        private void ActualizaTipoDePuestoYPoda(Node<Registro> raiz, int NivelDelSuperior)
        {

            if ((NivelDelSuperior + 1) < p.pila.Length)
            {
                raiz.Data.TipoRegistro = p.pila[NivelDelSuperior + 1];
                foreach (Node<Registro> nodo in raiz.Sons)
                {
                    ActualizaTipoDePuestoYPoda(nodo, NivelDelSuperior + 1);
                }
            }
            else
            {
                raiz.Padre.Sons.Remove(raiz);
            }
        }

        private void buttonOrgPegar_Click(object sender, EventArgs e)
        {
            int NivelDelJefe;
            String IDMostrado = NodoSeleccionado.Data.ID;
            if (!(NodoSeleccionado == null) && ListaDeNodosPorID.ContainsKey(IDMostrado))
            {
                if ((NivelDelJefe = NivelGerarquico(NodoSeleccionado)) == -1)
                {
                    MessageBox.Show("Error de nivel de puesto");
                }
                else
                {
                    NodoCortado.Padre = NodoSeleccionado;
                    ActualizaTipoDePuestoYPoda(NodoCortado, NivelDelJefe);
                    // Insertamos en el arbol de la APF
                    NodoSeleccionado.Sons.InsertaHijo(NodoCortado);
                    LlenaListaDeNodosPorID(NodoCortado);
                    // Hay que mostrar el nuevo subarbol en el TreeView
                    // Actualizamos el TreeView
                    TreeNode Auxiliar = null;
                    organigrama.LlenaTreeAPF(NodoDeArbolMostrado.Nodes, NodoCortado, NodoDeArbolMostrado.Nodes.Count, false, ref Auxiliar);
                    NodoDeArbolMostrado.Expand();
                }
            }
            else
                MessageBox.Show("Se debe seleccionar el puesto al que se le van a insertar los hijos");
        }

        private void tabPageOrganigrama_Click(object sender, EventArgs e)
        {

        }

        //-------------------------------
        private void MoveUp(TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);
                }
            }
            else if (node.TreeView.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index > 0)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index - 1, node);
                }
            }
        }

        private void MoveDown(TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);
                }
            }
            else if (view != null && view.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index < view.Nodes.Count - 1)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index + 1, node);
                }
            }
        }
        //-------------------------------

        private void buttonOrgSubir_Click(object sender, EventArgs e)
        {
            if ((NodoSeleccionado != null) && (NodoSeleccionado.Padre != null))
            {
                int IndiceSeleccionado = NodoSeleccionado.Padre.Sons.IndexOf(NodoSeleccionado);
                if (IndiceSeleccionado > 0)
                {
                    NodoSeleccionado.Padre.Sons.RemoveAt(IndiceSeleccionado);
                    NodoSeleccionado.Padre.Sons.Insert(IndiceSeleccionado - 1, NodoSeleccionado);

                    MoveUp(NodoDeArbolMostrado);

                }
            }
        }

        private void buttonOrgBajar_Click(object sender, EventArgs e)
        {
            if ((NodoSeleccionado != null) && (NodoSeleccionado.Padre != null))
            {
                int IndiceSeleccionado = NodoSeleccionado.Padre.Sons.IndexOf(NodoSeleccionado);
                if (IndiceSeleccionado < (NodoSeleccionado.Padre.Sons.Count - 1))
                {
                    NodoSeleccionado.Padre.Sons.RemoveAt(IndiceSeleccionado);
                    NodoSeleccionado.Padre.Sons.Insert(IndiceSeleccionado + 1, NodoSeleccionado);

                    MoveDown(NodoDeArbolMostrado);
                }
            }
        }

        private void buttonOrgAtras_Click(object sender, EventArgs e)
        {
            System.IO.File.Copy(Defines.DataBasePath + Defines.BackupDataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileName, true);
            treeViewOrganigramaAPF.Nodes.Remove(RaizTreeView);
            ListaDeNodosPorID.Clear();
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0");
            APF = new Node<Registro>(Presidente);
            ListaDeNodosPorID.Add("A0", APF);
            p.InitTokens();
            p.Parsea(APF, 0, ListaDeNodosPorID);
            ImprimeConsola("---------- ATRAS --------------");
            organigrama.PrintTreeAPF(APF, ImprimeConsola);
            organigrama.LlenaTreeAPF(treeViewOrganigramaAPF.Nodes, APF, 0, true, ref RaizTreeView);
        }

        private void buttonOrgBackup_Click(object sender, EventArgs e)
        {
            System.IO.File.Copy(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + Defines.BackupDataBaseFileName, true);
        }

        private void buttonOrgGuardar_Click(object sender, EventArgs e)
        {
            ImprimeConsola("Guardamos el Organigrama");
            string MaxId1 = AccessUtility.OrganigramaMaxId1();
            organigrama.SalvaTreeAPF(APF, ImprimeConsola, false);
            AccessUtility.DeleteOrganigrama(MaxId1);
        }

        private void dateTimePickerFechaNacimiento_ValueChanged(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        private void muestraCapturaFechaNacimiento_Load(object sender, EventArgs e)
        {
            buttonModifica.Enabled = true;
            DatosPersonalesModificados = true;
            BusquedaEnProceso = false;
        }

        int PosTipoINFO = -1;

        private void checkedListBoxTipoINFO_SelectedIndexChanged(object sender, EventArgs e)
        {
            PosTipoINFO = checkedListBoxTipoINFO.SelectedIndex;
        }

        private void checkedListBoxTipoINFO_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                for (int ix = 0; ix < checkedListBoxTipoINFO.Items.Count; ++ix)
                    if (e.Index != ix) checkedListBoxTipoINFO.SetItemChecked(ix, false);
        }

        int PosTipoInformacion = -1;

        private void checkedListBoxTipoInformacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            PosTipoInformacion = checkedListBoxTipoInformacion.SelectedIndex;
        }

        private void CheckedListBoxTipoInformacion_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                for (int ix = 0; ix < checkedListBoxTipoInformacion.Items.Count; ++ix)
                    if (e.Index != ix) checkedListBoxTipoInformacion.SetItemChecked(ix, false);
        }

    }
}
// TODO: Agrupacion de arbol
// TODO: Revisar Campos Nulos
// TODO: Acabar de implementar Datos de Contacto y Circulo Cercano
// TODO: Buscar por ID
// TODO: quitar la fecha de la busqueda o implementarla bien
// fabricación de mosaicos de pasta en hermosillo