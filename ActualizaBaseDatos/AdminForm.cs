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
        IndiceBD indexCirculoCercano;
        IndiceBD indexDatosContacto;

        bool DatosPersonalesModificados = false;
        bool FotoModificada = false;
        DataRow[] funcionarios;
        DataRow[] escolaridad;
        DataRow[] AP;
        DataRow[] INFO;
        DataRow[] Puestos;
        DataRow[] CirculoCercano;
        DataRow[] DatosContacto;

        string NewFotoFileName = string.Empty;

        bool BusquedaEnProceso = false;

        public Node<Registro> APF;
        Dictionary<string, Node<Registro>> ListaDeNodosPorID = new Dictionary<string, Node<Registro>>();
        public Node<Registro> nodoSeleccionado = null;
        TreeNode NodoDeArbolMostrado = null;
        Parser p;
        string Layer = string.Empty;

        string AbogadoIrresponsable
        {
            get
            {
                return Layer;
            }
            set
            {
                Layer = value;
                labelLoginAs1.Text = Layer;
                labelLoginAs2.Text = Layer;
            }
        }

        int indiceModificación = 0;

        int IndiceModificación
        {
            get
            {
                return indiceModificación;
            }
            set
            {
                if (value == 0)
                {
                    checkBoxAgrupaciónModifica.Checked = false;
                    checkBoxActualizaPuesto.Checked = false;
                    checkBoxActualizeFuncionario.Checked = false;
                }
                indiceModificación = value;
            }
        }

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
            treeViewOrganigramaAPF.BeforeSelect += TreeViewOrganigramaAPF_BeforeSelect;
            int niveles = 0;
            for (niveles = 0; niveles < p.pila.Length - 1; niveles++)
                NivelSIguiente.Add(p.pila[niveles], p.pila[niveles + 1]);
            NivelSIguiente.Add(p.pila[niveles], "nadie");
            checkedListBoxTipoINFO.ItemCheck += checkedListBoxTipoINFO_ItemCheck;
            checkedListBoxTipoInformacion.ItemCheck += CheckedListBoxTipoInformacion_ItemCheck;
        }

        private void TreeViewOrganigramaAPF_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeViewOrganigramaAPF.SelectedNode != null) 
                treeViewOrganigramaAPF.SelectedNode.BackColor = Color.White;
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
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", "El sistema");
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
            muestraCapturaFechaNacimiento.SetFecha(Ut.Numero(funcionarios[index]["AñoNacimiento"].ToString()),
                Ut.Numero(funcionarios[index]["MesNacimiento"].ToString()),
                Ut.Numero(funcionarios[index]["DiaNacimiento"].ToString()));

            IDFuncionario = funcionarios[index]["ID"].ToString();
            labelAbogadoResponsable.Text = funcionarios[index]["Abogado"].ToString().Equals("")?"     ": funcionarios[index]["Abogado"].ToString();

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

            CirculoCercano = AccessUtility.GetCirculoCercano(IDFuncionario);
            indexCirculoCercano = new IndiceBD(CirculoCercano.Length);
            LlenaCirculoCercano(IDFuncionario);

            DatosContacto = AccessUtility.GetDatosContacto(IDFuncionario);
            indexDatosContacto = new IndiceBD(DatosContacto.Length);
            LlenaDatosContacto(IDFuncionario);

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
            labelAbogadoResponsable.Text = "     ";

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

            LimpiaCirculoCercano(IDFuncionario);

            LimpiaDatosContacto(IDFuncionario);

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
                muestraCapturaFechaInicio.SetFecha(Ut.Numero(escolaridad[indexEscolaridad.Pos]["AñoInicial"].ToString()),
                    Ut.Numero(escolaridad[indexEscolaridad.Pos]["MesInicial"].ToString()),
                    Ut.Numero(escolaridad[indexEscolaridad.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaFin.SetFecha(Ut.Numero(escolaridad[indexEscolaridad.Pos]["AñoFinal"].ToString()),
                    Ut.Numero(escolaridad[indexEscolaridad.Pos]["MesFinal"].ToString()),
                    Ut.Numero(escolaridad[indexEscolaridad.Pos]["DiaFinal"].ToString()));
                textBoxUniversidad.Text = escolaridad[indexEscolaridad.Pos]["Universidad"].ToString();
                textBoxGrado.Text = escolaridad[indexEscolaridad.Pos]["Grado"].ToString();
                labelEscolaridadPos.Text = (indexEscolaridad.Pos + 1).ToString();
                labelEscolaridadLength.Text = "de " + indexEscolaridad.Length.ToString();
                labelAbogadoRespEsc.Text = escolaridad[indexEscolaridad.Pos]["Abogado"].ToString().Equals("")?"     ": escolaridad[indexEscolaridad.Pos]["Abogado"].ToString();
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
            labelAbogadoRespEsc.Text = "     ";
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
                muestraCapturaFechaAPInicio.SetFecha(Ut.Numero(AP[indexAP.Pos]["FechaDeInicio"].ToString()), 1, 1);
                muestraCapturaFechaAPFin.SetFecha(Ut.Numero(AP[indexAP.Pos]["FechaDeFin"].ToString()), 1, 1);
                textBoxAPPartido.Text = AP[indexAP.Pos]["NombreDelPartido"].ToString();
                labelAPPos.Text = (indexAP.Pos + 1).ToString();
                labelAPLength.Text = "de " + indexAP.Length.ToString();
                labelAbogadoRespAP.Text = AP[indexAP.Pos]["Abogado"].ToString().Equals("")?"     ": AP[indexAP.Pos]["Abogado"].ToString();
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
            labelAbogadoRespAP.Text = "     ";
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
                int index = checkedListBoxTipoInformacion.FindString(INFO[indexINFO.Pos]["TipoDeInformación"].ToString());
                checkedListBoxTipoInformacion.SelectedIndex = index;
                textBoxINFOReferencia.Multiline = true;
                textBoxINFOReferencia.Text = INFO[indexINFO.Pos]["Referencia"].ToString();
                labelINFOPos.Text = (indexINFO.Pos + 1).ToString();
                labelINFOLength.Text = "de " + indexINFO.Length.ToString();
                labelAbogadoRespInfoGral.Text = INFO[indexINFO.Pos]["Abogado"].ToString().Equals("")?"     ": INFO[indexINFO.Pos]["Abogado"].ToString();
            }
        }

        private void LimpiaINFO(string ID)
        {
            textBoxINFOID.Text = ID;
            checkedListBoxTipoInformacion.SelectedIndex = checkedListBoxTipoInformacion.Items.Count - 1;
            textBoxINFOReferencia.Multiline = true;
            textBoxINFOReferencia.Text = string.Empty;
            labelINFOPos.Text = string.Empty;
            labelINFOLength.Text = string.Empty;
            labelAbogadoRespInfoGral.Text = "     ";
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
                muestraCapturaFechaPuestoInicio.SetFecha(Ut.Numero(Puestos[indexPuestos.Pos]["AñoInicial"].ToString()),
                    Ut.Numero(Puestos[indexPuestos.Pos]["MesInicial"].ToString()),
                    Ut.Numero(Puestos[indexPuestos.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaPuestoFin.SetFecha(Ut.Numero(Puestos[indexPuestos.Pos]["AñoFinal"].ToString()),
                    Ut.Numero(Puestos[indexPuestos.Pos]["MesFinal"].ToString()),
                    Ut.Numero(Puestos[indexPuestos.Pos]["DiaFinal"].ToString()));

                labelPuestosPos.Text = (indexPuestos.Pos + 1).ToString();
                labelPuestosLength.Text = "de " + indexPuestos.Length.ToString();
                checkBoxPuestosCargoActual.Checked = Puestos[indexPuestos.Pos]["CargoActual"].ToString().Equals("1");
                labelAbogadoRespPuesto.Text = Puestos[indexPuestos.Pos]["Abogado"].ToString().Equals("")?"     ": Puestos[indexPuestos.Pos]["Abogado"].ToString();
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
            if (Puestos.Length > 0)
            {
                checkBoxPuestosCargoActual.Checked = Puestos[indexPuestos.Pos]["CargoActual"].ToString().Equals("1");
                labelAbogadoRespPuesto.Text = Puestos[indexPuestos.Pos]["Abogado"].ToString().Equals("") ? "     " : Puestos[indexPuestos.Pos]["Abogado"].ToString();
            }
            
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
                case "Circulo Cercano":
                    LlenaCirculoCercano(IDFuncionario);
                    break;
                case "Datos de Contacto":
                    LlenaDatosContacto(IDFuncionario);
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
                    IndiceModificación = 0;
                    break;
                case "Publica Info":
                    if (AbogadoIrresponsable.Equals("Alfredo") || AbogadoIrresponsable.Equals("rayo"))
                    {
                        PetaSecure cipher = new PetaSecure();
                        cipher.FileEncrypt(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileNameEncriptado,
                            System.Text.Encoding.UTF8.GetString(Defines.ImagenDefault).Substring(Defines.PosInicial, Defines.PosFinal));
                        PetaPublish.Publisher.UploadInfoAPFDB(Defines.DataBasePath, Defines.DataBaseFileNameEncriptado);
                        labelSTATUS.Text = "Terminado";
                    }
                    else
                        MessageBox.Show("No tiene autorización para publicar la base de datos");
                    break;
                case "Identificate":
                    textBoxPassword.Text = string.Empty;
                    AbogadoIrresponsable = string.Empty;
                    textBoxPassword.PasswordChar = '#';
                    buttonVerificaOK.Text = "Verifica";
                    break;
                case "Desconectate":
                    textBoxPassword.Text = string.Empty;
                    AbogadoIrresponsable = string.Empty;
                    break;
                default:
                    break;
            }
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
                if (!Ut.SinA(textBoxApellidoPaterno.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoPaterno.Text);
                if (!Ut.SinA(textBoxApellidoMaterno.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoMaterno.Text);
                if (!Ut.SinA(textBoxPrimerNombre.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxPrimerNombre.Text);
                if (!Ut.SinA(textBoxSegundoNombre.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxSegundoNombre.Text);
                if (!Ut.SinA(textBoxNacionalidad.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxNacionalidad.Text);
            }
            while (i < funcionarioMostrado.Length &&
                    !BusquedaActiva.SatisfaceCriterio(Ut.SinA(funcionarios[i]["PrimerNombre"].ToString()).Replace(" ", string.Empty) + " " +
                        Ut.SinA(funcionarios[i]["SegundoNombre"].ToString().Replace(" ", string.Empty)) + " " +
                        Ut.SinA(funcionarios[i]["ApellidoPaterno"].ToString().Replace(" ", string.Empty)) + " " +
                        Ut.SinA(funcionarios[i]["ApellidoMaterno"].ToString().Replace(" ", string.Empty)) + " " +
                        Ut.SinA(funcionarios[i]["Nacionalidad"].ToString().Replace(" ", string.Empty)))) i++;
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
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                AccessUtility.InsertRegistroEscolaridad(textBoxID.Text, muestraCapturaFechaInicio.Año.ToString(), muestraCapturaFechaInicio.Mes.ToString(), muestraCapturaFechaInicio.Dia.ToString(),
                    muestraCapturaFechaFin.Año.ToString(), muestraCapturaFechaFin.Mes.ToString(), muestraCapturaFechaFin.Dia.ToString(),
                    textBoxUniversidad.Text, textBoxGrado.Text, AbogadoIrresponsable);
                escolaridad = AccessUtility.GetEscolaridad(IDFuncionario);
                indexEscolaridad = new IndiceBD(escolaridad.Length);
                LlenaEscolaridad(IDFuncionario);
            }
            else
                MessageBox.Show("Te tienes que identificar primero");
            
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
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                AccessUtility.InsertRegistroAP(textBoxAPID.Text, muestraCapturaFechaAPInicio.Año.ToString(),
                    muestraCapturaFechaAPFin.Año.ToString(),
                textBoxAPPartido.Text, AbogadoIrresponsable);
                AP = AccessUtility.GetAdscripcionPolitica(IDFuncionario);
                indexAP = new IndiceBD(AP.Length);
                LlenaAP(IDFuncionario);
            }
            else
                MessageBox.Show("Te tienen que identificar primero");
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
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                if ((checkedListBoxTipoInformacion.SelectedIndex > -1) && (checkedListBoxTipoInformacion.SelectedIndex < checkedListBoxTipoInformacion.Items.Count))
                {
                    AccessUtility.InsertRegistroINFO(textBoxINFOID.Text, checkedListBoxTipoInformacion.Text, textBoxINFOReferencia.Text,
                            AbogadoIrresponsable);
                    INFO = AccessUtility.GetNotasRelevantes(IDFuncionario);
                    indexINFO = new IndiceBD(INFO.Length);
                    LlenaINFO(IDFuncionario);
                }
            }
            else
                MessageBox.Show("Te tienes que identificar primero");
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
            if (!AbogadoIrresponsable.Equals(""))
            {
                AccessUtility.InsertRegistroPuestos(textBoxPuestosID.Text, muestraCapturaFechaPuestoInicio.Año.ToString(), muestraCapturaFechaPuestoInicio.Mes.ToString(), muestraCapturaFechaPuestoInicio.Dia.ToString(),
                muestraCapturaFechaPuestoFin.Año.ToString(), muestraCapturaFechaPuestoFin.Mes.ToString(), muestraCapturaFechaPuestoFin.Dia.ToString(), textBoxPuestosDependencia.Text, textBoxPuestosPuesto.Text,
                textBoxPuestosSuperior.Text, checkBoxPuestosCargoActual.CheckState == CheckState.Checked ? "actual" :
                string.Empty, AbogadoIrresponsable);
                Puestos = AccessUtility.GetPuestos(IDFuncionario);
                indexPuestos = new IndiceBD(Puestos.Length);
                LlenaPuestos(IDFuncionario);
            }
            else
                MessageBox.Show("Tienes que identificarte primero");
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
        }

        private int DespliegaInformacionDelID(string ID)
        {
            for (int i = 0; i < funcionarios.Length; i++)
            {
                if (funcionarios[i]["ID"].ToString().Equals(ID))
                    return i;
            }
            return 0;
        }

        private void buttonInserta_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                string ID = GetNextUsableID();
                AccessUtility.InsertFuncionario(ID, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text,
                    textBoxNacionalidad.Text, muestraCapturaFechaNacimiento.Año.ToString(), muestraCapturaFechaNacimiento.Mes.ToString(),
                    muestraCapturaFechaNacimiento.Dia.ToString(), AbogadoIrresponsable);
                if (!NewFotoFileName.Equals(string.Empty))
                {
                    AccessUtility.SubeFoto(ID, NewFotoFileName);
                }
                buttonInserta.Enabled = false;
                buttonModifica.Enabled = true;
                DatosPersonalesModificados = false;
                FotoModificada = false;
                BusquedaEnProceso = false;
                CargaBD(ID);
            }
            else
            {
                MessageBox.Show("Tienes que identificarte");
            }
        }

        private void buttonModifica_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                string IDMemoryRecall = textBoxID.Text;
                AccessUtility.UpdateFuncionario(textBoxID.Text, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text,
                    muestraCapturaFechaNacimiento.Año.ToString(), muestraCapturaFechaNacimiento.Mes.ToString(), muestraCapturaFechaNacimiento.Dia.ToString(), AbogadoIrresponsable);
                if (FotoModificada) AccessUtility.SubeFoto(textBoxID.Text, NewFotoFileName);
                buttonModifica.Enabled = false;
                DatosPersonalesModificados = false;
                FotoModificada = false;
                BusquedaEnProceso = false;
                CargaBD(IDMemoryRecall);
            }
            else
                MessageBox.Show("Tienes que identificarte primero");
        }

        private void CargaBD(String ID)
        {
            funcionarios = AccessUtility.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            funcionarioMostrado.Pos = DespliegaInformacionDelID(ID);
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

        private void buttonCargaBD_Click(object sender, EventArgs e)
        {
            CargaBD("A0");
        }

        private void treeViewOrganigramaAPF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            if (ListaDeNodosPorID.ContainsKey(ID))
            {
                NodoSeleccionado = ListaDeNodosPorID[ID];
                NodoDeArbolMostrado = treeViewOrganigramaAPF.SelectedNode;
                
                labelOrgAbogadoIrresponsable.Text = NodoSeleccionado.Data.AbogadoIrresponsable.Equals("") ? "     " : NodoSeleccionado.Data.AbogadoIrresponsable;
                //ImprimeConsola("-->  " + NodoSeleccionado.Data.ToString() + " # " + e.Node.Text);
                int gion = e.Node.Text.IndexOf('-');
                textBoxOrgNombrePuestoModificado.Text = e.Node.Text.Substring(0, gion - 1);
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
            if (!AbogadoIrresponsable.Equals("") && NodoSeleccionado != null)
            {
                String IDMostrado = NodoSeleccionado.Data.ID; // ID del Nodo del Arbol que estamos viendo
                string ID; // ID de la ficha seleccionada
                Registro NuevoRegistro = null;
                Node<Registro> NuevoNode = null;
                if (!(NodoSeleccionado == null) && ListaDeNodosPorID.ContainsKey(IDMostrado))
                {
                    
                    switch (IndiceModificación)
                    {
                        case 1: // Modificamos un nodo de agrupación
                            Random rnd = new Random();
                            ID = "O" + rnd.Next(1, 100000).ToString();
                            while (ListaDeNodosPorID.ContainsKey(ID))
                            {
                                ID = "O" + rnd.Next(1, 100000).ToString();
                            }
                            NodoDeArbolMostrado.Text = textBoxOrgNombrePuestoModificado.Text + "- _" + ID;
                            NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, textBoxOrgNombrePuestoModificado.Text, ID, AbogadoIrresponsable);
                            NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                            ListaDeNodosPorID.Add(ID, NuevoNode);
                            // tengo que actualizar APF
                            ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                            ListaDeNodosPorID.Remove(IDMostrado);
                            break;
                        case 2:  // Modificamos un Puesto
                            NodoDeArbolMostrado.Text = textBoxOrgNombrePuestoModificado.Text + " - " + AccessUtility.GetNombreFuncionario(IDMostrado) + "_" + IDMostrado;
                            NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, textBoxOrgNombrePuestoModificado.Text, IDMostrado, AbogadoIrresponsable);
                            NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                            // tengo que actualizar APF
                            ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                            ListaDeNodosPorID[IDMostrado] =  NuevoNode;
                            break;
                        case 3: // Modificamos un fucionario
                            ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
                            int gion = NodoDeArbolMostrado.Text.IndexOf('-');
                            NodoDeArbolMostrado.Text = NodoDeArbolMostrado.Text.Substring(0, gion - 1) + " - " + AccessUtility.GetNombreFuncionario(ID) + "_" + ID;
                            NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, NodoDeArbolMostrado.Text.Substring(0, gion - 1), ID, AbogadoIrresponsable);
                            NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                            ListaDeNodosPorID.Add(ID, NuevoNode);
                            // tengo que actualizar APF
                            ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                            ListaDeNodosPorID.Remove(IDMostrado);
                            break;
                        default:
                            MessageBox.Show("Se debe escoger alguna opción de modificación");
                            break;
                    }
                    NuevoRegistro.NodoDelTreeView = NodoDeArbolMostrado;
                    treeViewOrganigramaAPF.SelectedNode = null;
                    NodoDeArbolMostrado = null;
                    NodoSeleccionado = null;
                    organigrama.PrintTreeAPF(APF, ImprimeConsola);
                    textBoxOrgNombrePuestoModificado.Text = string.Empty;
                }
            }
            else
                MessageBox.Show("Es necesario identificarse primero");
        }

        // Inserta
        private void buttonOrgInsertaPuesto_Click(object sender, EventArgs e)
        {

            if (!AbogadoIrresponsable.Equals(""))
            {
                string ID;
                if (checkBoxAgrupacionCrear.Checked)
                {
                    Random rnd = new Random();
                    ID = "O" + rnd.Next(1,100000).ToString();
                    while (ListaDeNodosPorID.ContainsKey(ID))
                    {
                        ID = "O" + rnd.Next(1, 100000).ToString();
                    }
                }
                else
                {
                    ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
                }
                
                if (NodoSeleccionado == null)
                {
                    MessageBox.Show("Nodo Selecccionado es nulo");
                }

                if (!textBoxOrgNombrePuesto.Text.Equals(string.Empty) && !ListaDeNodosPorID.ContainsKey(ID) && !(NodoSeleccionado == null))
                {
                    // Hay que modificar APF y ListadeNodosPorID
                    // siempre vamos a inserta el nuevo registro como un hijo, y el único nodo que no puedo borrar es el raiz (el presidente)
                    NodeList<Registro> SinHijos = new NodeList<Registro>();
                    Registro NuevoRegistro = new Registro(NivelSIguiente[NodoSeleccionado.Data.TipoRegistro], textBoxOrgNombrePuesto.Text, ID, AbogadoIrresponsable);
                    Node<Registro> NuevoPuesto = new Node<Registro>(NuevoRegistro, SinHijos, NodoSeleccionado);
                    ListaDeNodosPorID.Add(ID, NuevoPuesto);
                    //ImprimeConsola("+-> Contenido nodo insertado: " + NodoSeleccionado.Data.ToString());
                    NodoSeleccionado.Sons.InsertaHijo(NuevoPuesto);
                    // Actualizamos el TreeView
                    TreeNode newNode;
                    if (checkBoxAgrupacionCrear.Checked)
                    {
                        newNode = new TreeNode(textBoxOrgNombrePuesto.Text + "-_" + ID);
                    }
                    else
                    {
                        newNode = new TreeNode(textBoxOrgNombrePuesto.Text + " - " + AccessUtility.GetNombreFuncionario(ID) + "_" + ID);
                    }
                    NuevoPuesto.Data.NodoDelTreeView = newNode;
                    NodoDeArbolMostrado.Nodes.Add(newNode);
                    NodoDeArbolMostrado.Expand();

                    treeViewOrganigramaAPF.SelectedNode = null;
                    NodoDeArbolMostrado = null;
                    NodoSeleccionado = null;
                    organigrama.PrintTreeAPF(APF, ImprimeConsola);
                    textBoxOrgNombrePuesto.Text = string.Empty;
                }
                else
                {
                    if (ListaDeNodosPorID.ContainsKey(ID))
                    {
                        MessageBox.Show("El funcionario tiene otro puesto en la APF");
                        treeViewOrganigramaAPF.SelectedNode = ListaDeNodosPorID[ID].Data.NodoDelTreeView;
                        treeViewOrganigramaAPF.SelectedNode.Expand();
                        treeViewOrganigramaAPF.SelectedNode.BackColor = Color.CadetBlue;
                    }
                }
            }
            else
                MessageBox.Show("Tienes que identificarte primero");
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

        private void buttonOrgElimina_Click(object sender, EventArgs e)
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

        private void buttonOrgInserta_Click(object sender, EventArgs e)
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

        private int MoveUp(TreeNode node)
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
                    return index - 1;
                }
            }
            else if (node.TreeView.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index > 0)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index - 1, node);
                    return index - 1;
                }
            }
            return -1;
        }

        private int MoveDown(TreeNode node)
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
                    return index + 1;
                }
            }
            else if (view != null && view.Nodes.Contains(node)) //root node
            {
                int index = view.Nodes.IndexOf(node);
                if (index < view.Nodes.Count - 1)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index + 1, node);
                    return index + 1;
                }
            }
            return -1;
        }

        private void buttonOrgSubir_Click(object sender, EventArgs e)
        {
            if ((NodoSeleccionado != null) && (NodoSeleccionado.Padre != null))
            {
                int IndiceSeleccionado = NodoSeleccionado.Padre.Sons.IndexOf(NodoSeleccionado);
                if (IndiceSeleccionado > 0)
                {
                    NodoSeleccionado.Padre.Sons.RemoveAt(IndiceSeleccionado);
                    NodoSeleccionado.Padre.Sons.Insert(IndiceSeleccionado - 1, NodoSeleccionado);

                    int i = MoveUp(NodoDeArbolMostrado);
                    treeViewOrganigramaAPF.SelectedNode = NodoDeArbolMostrado.Parent.Nodes[i];
                    treeViewOrganigramaAPF.SelectedNode.BackColor = Color.CadetBlue;
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

                    int i = MoveDown(NodoDeArbolMostrado);
                    treeViewOrganigramaAPF.SelectedNode = NodoDeArbolMostrado.Parent.Nodes[i];
                    treeViewOrganigramaAPF.SelectedNode.BackColor = Color.CadetBlue;
                }
            }
        }

        private void buttonOrgAtras_Click(object sender, EventArgs e)
        {
            System.IO.File.Copy(Defines.DataBasePath + Defines.BackupDataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileName, true);
            treeViewOrganigramaAPF.Nodes.Remove(RaizTreeView);
            ListaDeNodosPorID.Clear();
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", "El Sistema");
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
            if (!AbogadoIrresponsable.Equals(""))
            {
                ImprimeConsola("Guardamos el Organigrama");
                string MaxId1 = AccessUtility.OrganigramaMaxId1();
                organigrama.SalvaTreeAPF(APF, ImprimeConsola, false);
                AccessUtility.DeleteOrganigrama(MaxId1);
            }
            else
                MessageBox.Show("Tienes que identificarte primero");
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
                for (int i = 0; i < checkedListBoxTipoInformacion.Items.Count; ++i)
                    if (e.Index != i) checkedListBoxTipoInformacion.SetItemChecked(i, false);
        }

        private void buttonDatosContactoInserta_Click(object sender, EventArgs e)
        {
            if ((checkedListBoxTipoINFO.SelectedIndex > -1) && (checkedListBoxTipoINFO.SelectedIndex < checkedListBoxTipoINFO.Items.Count))
            {
                AccessUtility.InsertRegistroDatosContacto(textBoxDatosContactoID.Text, checkedListBoxTipoINFO.Text, textBoxDatosContactoDato.Text, labelDatosContactoAbogadoResp.Text);
                DatosContacto = AccessUtility.GetDatosContacto(IDFuncionario);
                indexDatosContacto = new IndiceBD(INFO.Length);
                LlenaDatosContacto(IDFuncionario);
            }
        }

        private void buttonDatosContactoInicial_Click(object sender, EventArgs e)
        {
            indexDatosContacto.Inicial();
            LlenaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoPrevio_Click(object sender, EventArgs e)
        {
            indexDatosContacto.Previous();
            LlenaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoSiguiente_Click(object sender, EventArgs e)
        {
            indexDatosContacto.Next();
            LlenaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoFinal_Click(object sender, EventArgs e)
        {
            indexDatosContacto.Final();
            LlenaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoLimpia_Click(object sender, EventArgs e)
        {
            LimpiaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroDatosContacto(DatosContacto[indexDatosContacto.Pos]["Id1"].ToString());
            DatosContacto = AccessUtility.GetDatosContacto(IDFuncionario);
            indexDatosContacto = new IndiceBD(DatosContacto.Length);
            LlenaDatosContacto(IDFuncionario);
        }

        private void textBoxDatosContactoDato_TextChanged(object sender, EventArgs e)
        {

        }

        private void LlenaDatosContacto(string ID)
        {
            // Circulo Cercano
            if (indexDatosContacto.Length == 0)
            {
                LimpiaDatosContacto(ID);
            }
            else
            {
                textBoxDatosContactoID.Text = DatosContacto[indexDatosContacto.Pos]["ID"].ToString();
                checkedListBoxTipoINFO.SelectedIndex = checkedListBoxTipoINFO.FindString(DatosContacto[indexDatosContacto.Pos]["Tipo"].ToString());
                textBoxDatosContactoDato.Text = DatosContacto[indexDatosContacto.Pos]["dato"].ToString();
                labelDatosContactoPos.Text = (indexDatosContacto.Pos + 1).ToString();
                labelDatosContactoLength.Text = "de " + indexDatosContacto.Length.ToString();
                labelDatosContactoAbogadoResp.Text = DatosContacto[indexDatosContacto.Pos]["Abogado"].ToString().Equals("")?"     ": DatosContacto[indexDatosContacto.Pos]["Abogado"].ToString();
            }
        }

        private void LimpiaDatosContacto(string ID)
        {
            textBoxDatosContactoID.Text = ID;
            checkedListBoxTipoINFO.SelectedIndex = checkedListBoxTipoINFO.Items.Count - 1;
            textBoxDatosContactoDato.Text = string.Empty;
            labelDatosContactoPos.Text = string.Empty;
            labelDatosContactoLength.Text = string.Empty;
            labelDatosContactoAbogadoResp.Text = "     ";
        }

        private void textBoxRecuperaID_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void buttonTraeID_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < funcionarios.Length; i++)
            {
                if (funcionarios[i]["ID"].ToString().Equals(textBoxRecuperaID.Text))
                {
                    funcionarioMostrado.Pos = i;
                    DespliegaInformación(funcionarioMostrado.Pos);
                    buttonInserta.Enabled = false;
                    buttonModifica.Enabled = true;
                    return;
                }
            }
            MessageBox.Show("No se encontró un registro con los datos señalados");
        }

        private void LlenaCirculoCercano(string ID)
        {
            // Circulo Cercano
            if (indexCirculoCercano.Length == 0)
            {
                LimpiaCirculoCercano(ID);
            }
            else
            {
                textBoxCirculoCercanoID.Text = CirculoCercano[indexCirculoCercano.Pos]["ID"].ToString();
                textBoxCirculoCercanoNombre.Text = CirculoCercano[indexCirculoCercano.Pos]["Nombre"].ToString();
                textBoxCirculoCercanoInformación.Text = CirculoCercano[indexCirculoCercano.Pos]["Información"].ToString();
                labelCirculoCercanoPos.Text = (indexCirculoCercano.Pos + 1).ToString();
                labelCirculoCercanoCuantos.Text = "de " + indexCirculoCercano.Length.ToString();
                labelCirculoCercanoAbogadoResp.Text = CirculoCercano[indexCirculoCercano.Pos]["Abogado"].ToString().Equals("")?"     ": CirculoCercano[indexCirculoCercano.Pos]["Abogado"].ToString();
            }
        }

        private void LimpiaCirculoCercano(string ID)
        {
            textBoxCirculoCercanoID.Text = ID;
            textBoxCirculoCercanoNombre.Text = string.Empty;
            textBoxCirculoCercanoInformación.Text = string.Empty;
            labelCirculoCercanoPos.Text = string.Empty;
            labelCirculoCercanoCuantos.Text = string.Empty;
            labelCirculoCercanoAbogadoResp.Text = "     ";
        }

        private void buttonCirculoCercanoInicial_Click(object sender, EventArgs e)
        {
            indexCirculoCercano.Inicial();
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoPrevio_Click(object sender, EventArgs e)
        {
            indexCirculoCercano.Previous();
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoSIguiente_Click(object sender, EventArgs e)
        {
            indexCirculoCercano.Next();
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoFinal_Click(object sender, EventArgs e)
        {
            indexCirculoCercano.Final();
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoInserta_Click(object sender, EventArgs e)
        {
            AccessUtility.InsertRegistroCirculoCercano(textBoxCirculoCercanoID.Text, textBoxCirculoCercanoNombre.Text,
                    textBoxCirculoCercanoInformación.Text, labelCirculoCercanoAbogadoResp.Text);
            DatosContacto = AccessUtility.GetCirculoCercano(IDFuncionario);
            indexCirculoCercano = new IndiceBD(INFO.Length);
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonCurculoCercanoLimpia_Click(object sender, EventArgs e)
        {
            LimpiaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoElimina_Click(object sender, EventArgs e)
        {
            AccessUtility.DeleteRegistroCirculoCercano(CirculoCercano[indexCirculoCercano.Pos]["Id1"].ToString());
            CirculoCercano = AccessUtility.GetCirculoCercano(IDFuncionario);
            indexCirculoCercano = new IndiceBD(CirculoCercano.Length);
            LlenaCirculoCercano(IDFuncionario);
        }

        private void buttonVerificaOK_Click(object sender, EventArgs e)
        {
            if (AccessUtility.VerificaAbogadoIrresponsable(textBoxAbogadoIrresponsable.Text, textBoxPassword.Text))
            {
                AbogadoIrresponsable = textBoxAbogadoIrresponsable.Text;
                buttonVerificaOK.Text = "OK";
                textBoxAbogadoIrresponsable.Text = string.Empty;
                textBoxPassword.Text = string.Empty;
            }
            else
            {
                textBoxAbogadoIrresponsable.Text = string.Empty;
                textBoxPassword.Text = string.Empty;
                MessageBox.Show("Abogado Responsable / Clave invalidos, intente de nuevo");
            }
        }

        private void checkBoxAgrupaciónModifica_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAgrupaciónModifica.Checked = true;
            checkBoxActualizaPuesto.Checked = false;
            checkBoxActualizeFuncionario.Checked = false;
            IndiceModificación = 1;
        }

        private void checkBoxActualizaPuesto_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAgrupaciónModifica.Checked = false;
            checkBoxActualizaPuesto.Checked = true;
            checkBoxActualizeFuncionario.Checked = false;
            IndiceModificación = 2;
        }

        private void checkBoxActualizeFuncionario_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAgrupaciónModifica.Checked = false;
            checkBoxActualizaPuesto.Checked = false;
            checkBoxActualizeFuncionario.Checked = true;
            IndiceModificación = 3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialogDataBase.Filter = "Base de Datos APF|*.accdb";
            openFileDialogDataBase.ShowDialog();
            if (File.Exists(openFileDialogDataBase.FileName) && !openFileDialogDataBase.FileName.Equals(Defines.DataBasePath + Defines.DataBaseFileName))
            {
                string nombreARchivo = Defines.DataBasePath + "F" + DateTime.Now.ToString("yyyyMMddTHHmmss") + Defines.BackupDataBaseFileName;
                System.IO.File.Move(Defines.DataBasePath + Defines.DataBaseFileName, nombreARchivo);
                System.IO.File.Move(openFileDialogDataBase.FileName, Defines.DataBasePath + Defines.DataBaseFileName);
                treeViewOrganigramaAPF.Nodes.Remove(RaizTreeView);
                ListaDeNodosPorID.Clear();
                Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", "El Sistema");
                APF = new Node<Registro>(Presidente);
                ListaDeNodosPorID.Add("A0", APF);
                p.InitTokens();
                p.Parsea(APF, 0, ListaDeNodosPorID);
                ImprimeConsola("---------- ATRAS --------------");
                organigrama.PrintTreeAPF(APF, ImprimeConsola);
                organigrama.LlenaTreeAPF(treeViewOrganigramaAPF.Nodes, APF, 0, true, ref RaizTreeView);
            }
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonBuscaFunSeleccionado_Click(object sender, EventArgs e)
        {
            string ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
            treeViewOrganigramaAPF.SelectedNode = ListaDeNodosPorID[ID].Data.NodoDelTreeView;
            treeViewOrganigramaAPF.SelectedNode.Expand();
            treeViewOrganigramaAPF.SelectedNode.BackColor = Color.CadetBlue;
        }
    }
}
// TODO: Agrupacion de arbol
// TODO: Probar DatosContacto y CirculoCercano
// fabricación de mosaicos de pasta en hermosillo