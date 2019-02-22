using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using AccesoBaseDatos;
using Globales;
using System.Text.RegularExpressions;

using OrganigramaAdmin;
using Peta;
using APFInfo;
using System.Collections.Generic;
using System.Drawing;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using System.Text;

// In case of installation problems:  Solo indicar que borrando todas las claves del registro 
// HKEY_CURRENT_USER\Software\Classes\Software\Microsoft\Windows\CurrentVersion\PackageMetadata

namespace ActualizaBaseDatos
{
    public partial class AdminForm : Form
    {
        Organigrama organigrama = new Organigrama();

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
        DataRow[] PDFPresupuesto;
        const int NumeroDeBotones = 3;

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
                indiceModificación = (value % (NumeroDeBotones + 1)); // 1 posición para que no haya nada seleccionado

                switch( indiceModificación)
                {
                    case 1:
                        labelFuncionario.BackColor = Color.White;
                        labelPuesto.BackColor = Color.White;
                        labelNodoAgrupación.BackColor = Color.Aquamarine;
                        break;
                    case 2:
                        labelNodoAgrupación.BackColor = Color.White;
                        labelFuncionario.BackColor = Color.White;
                        labelPuesto.BackColor = Color.Aquamarine;
                        break;
                    case 3:
                        labelPuesto.BackColor = Color.White;
                        labelNodoAgrupación.BackColor = Color.White;
                        labelFuncionario.BackColor = Color.Aquamarine;
                        break;
                    default:
                        labelFuncionario.BackColor = Color.White;
                        labelPuesto.BackColor = Color.White;
                        labelNodoAgrupación.BackColor = Color.White;
                        break;
                }
            }
        }

        //System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();

        public Node<Registro> NodoSeleccionado
        {
            get
            {
                //if (nodoSeleccionado == null)
                //{
                //    t.ToString();
                //}
                return nodoSeleccionado;
            }
            set
            {
                //t.ToString();
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

        TreeNode RaizTreeView = null;

        private void InicializaDS()
        {
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", 0, "El sistema", 0);
            APF = new Node<Registro>(Presidente);
            ListaDeNodosPorID.Add("A0", APF);
            p.Parsea(APF, 0, ListaDeNodosPorID);

            funcionarios = Datos.Instance.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            if (funcionarioMostrado.Inicializado())
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
            LlenaComboBoxPresupuesto();
        }

        void LlenaComboBoxPresupuesto()
        {
            if (comboBoxPDFPresupuesto.Items.Count > 0)
            {
                comboBoxPDFPresupuesto.Items.Clear();
            }
            PDFPresupuesto = Datos.Instance.GetPresupuesto();
            foreach (DataRow row in PDFPresupuesto)
            {
                if (!Convert.ToBoolean(row["Asignado"]))
                    comboBoxPDFPresupuesto.Items.Add(row["PDFFileName"]);
            }
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
            muestraCapturaFechaNacimiento.SetFecha(U.Numero(funcionarios[index]["AñoNacimiento"].ToString()),
                U.Numero(funcionarios[index]["MesNacimiento"].ToString()),
                U.Numero(funcionarios[index]["DiaNacimiento"].ToString()));

            IDFuncionario = funcionarios[index]["ID"].ToString();
            labelAbogadoResponsable.Text = funcionarios[index]["Abogado"].ToString().Equals("")?"     ": funcionarios[index]["Abogado"].ToString();

            LoadPhoto(Datos.Instance.GetFoto(IDFuncionario));

            escolaridad = Datos.Instance.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad(IDFuncionario);

            AP = Datos.Instance.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP(IDFuncionario);

            INFO = Datos.Instance.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO(IDFuncionario);

            Puestos = Datos.Instance.GetPuestos(IDFuncionario);
            indexPuestos = new IndiceBD(Puestos.Length);
            LlenaPuestos(IDFuncionario);

            CirculoCercano = Datos.Instance.GetCirculoCercano(IDFuncionario);
            indexCirculoCercano = new IndiceBD(CirculoCercano.Length);
            LlenaCirculoCercano(IDFuncionario);

            DatosContacto = Datos.Instance.GetDatosContacto(IDFuncionario);
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
                pictureBox1.Image = System.Drawing.Image.FromStream(ms);
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
                muestraCapturaFechaInicio.SetFecha(U.Numero(escolaridad[indexEscolaridad.Pos]["AñoInicial"].ToString()),
                    U.Numero(escolaridad[indexEscolaridad.Pos]["MesInicial"].ToString()),
                    U.Numero(escolaridad[indexEscolaridad.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaFin.SetFecha(U.Numero(escolaridad[indexEscolaridad.Pos]["AñoFinal"].ToString()),
                    U.Numero(escolaridad[indexEscolaridad.Pos]["MesFinal"].ToString()),
                    U.Numero(escolaridad[indexEscolaridad.Pos]["DiaFinal"].ToString()));
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
                muestraCapturaFechaAPInicio.SetFecha(U.Numero(AP[indexAP.Pos]["FechaDeInicio"].ToString()), 1, 1);
                muestraCapturaFechaAPFin.SetFecha(U.Numero(AP[indexAP.Pos]["FechaDeFin"].ToString()), 1, 1);
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
                textBoxPuestosSuperior.Text = Puestos[indexPuestos.Pos]["JefeInmediatoSuperior"].ToString();
                muestraCapturaFechaPuestoInicio.SetFecha(U.Numero(Puestos[indexPuestos.Pos]["AñoInicial"].ToString()),
                    U.Numero(Puestos[indexPuestos.Pos]["MesInicial"].ToString()),
                    U.Numero(Puestos[indexPuestos.Pos]["DiaInicial"].ToString()));
                muestraCapturaFechaPuestoFin.SetFecha(U.Numero(Puestos[indexPuestos.Pos]["AñoFinal"].ToString()),
                    U.Numero(Puestos[indexPuestos.Pos]["MesFinal"].ToString()),
                    U.Numero(Puestos[indexPuestos.Pos]["DiaFinal"].ToString()));

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
                    IndiceModificación = 0;
                    break;
                case "Publica Info":
                    if (AbogadoIrresponsable.Equals("Alfredo") || AbogadoIrresponsable.Equals("rayo"))
                    {
                        PetaSecure cipher = new PetaSecure();
                        cipher.FileEncrypt(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + Defines.DataBaseFileNameEncriptado,
                            System.Text.Encoding.UTF8.GetString(Defines.ImagenDefault).Substring(Defines.PosInicial, Defines.PosFinal));
                        Peta.Publisher.UploadInfoAPFDB(Defines.DataBasePath, Defines.DataBaseFileNameEncriptado);
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
                if (!U.SinA(textBoxApellidoPaterno.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoPaterno.Text);
                if (!U.SinA(textBoxApellidoMaterno.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxApellidoMaterno.Text);
                if (!U.SinA(textBoxPrimerNombre.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxPrimerNombre.Text);
                if (!U.SinA(textBoxSegundoNombre.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxSegundoNombre.Text);
                if (!U.SinA(textBoxNacionalidad.Text).Replace(" ", string.Empty).Equals(string.Empty)) BusquedaActiva.Add(textBoxNacionalidad.Text);
            }
            while (i < funcionarioMostrado.Length &&
                    !BusquedaActiva.SatisfaceCriterio(U.SinA(funcionarios[i]["PrimerNombre"].ToString()).Replace(" ", string.Empty) + " " +
                        U.SinA(funcionarios[i]["SegundoNombre"].ToString().Replace(" ", string.Empty)) + " " +
                        U.SinA(funcionarios[i]["ApellidoPaterno"].ToString().Replace(" ", string.Empty)) + " " +
                        U.SinA(funcionarios[i]["ApellidoMaterno"].ToString().Replace(" ", string.Empty)) + " " +
                        U.SinA(funcionarios[i]["Nacionalidad"].ToString().Replace(" ", string.Empty)))) i++;
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
                    if (funcionarioMostrado.Inicializado())
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
                if (funcionarioMostrado.Inicializado())
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
                    if (funcionarioMostrado.Inicializado())
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
                if (funcionarioMostrado.Inicializado())
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
            if (indexEscolaridad.Inicializado())
            {
                indexEscolaridad.Inicial();
                LlenaEscolaridad(IDFuncionario);
            }
        }

        private void buttonEscolaridadSiguiente_Click(object sender, EventArgs e)
        {
            if (indexEscolaridad.Inicializado())
            {
                indexEscolaridad.Next();
                LlenaEscolaridad(IDFuncionario);
            }
        }

        private void buttonEscolaridadPrevio_Click(object sender, EventArgs e)
        {
            if (indexEscolaridad.Inicializado())
            {
                indexEscolaridad.Previous();
                LlenaEscolaridad(IDFuncionario);
            }
        }

        private void buttonEscolaridadFinal_Click(object sender, EventArgs e)
        {
            if (indexEscolaridad.Inicializado())
            {
                indexEscolaridad.Final();
                LlenaEscolaridad(IDFuncionario);
            }
        }

        private void buttonEscolaridadLimpia_Click(object sender, EventArgs e)
        {
            LimpiaEscolaridad(IDFuncionario);
        }

        private void buttonEscolaridadInserta_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                Datos.Instance.InsertRegistroEscolaridad(textBoxID.Text, muestraCapturaFechaInicio.Año.ToString(), muestraCapturaFechaInicio.Mes.ToString(), muestraCapturaFechaInicio.Dia.ToString(),
                    muestraCapturaFechaFin.Año.ToString(), muestraCapturaFechaFin.Mes.ToString(), muestraCapturaFechaFin.Dia.ToString(),
                    textBoxUniversidad.Text, textBoxGrado.Text, AbogadoIrresponsable);
                escolaridad = Datos.Instance.GetEscolaridad(IDFuncionario);
                indexEscolaridad = new IndiceBD(escolaridad.Length);
                LlenaEscolaridad(IDFuncionario);
            }
            else
                MessageBox.Show("Te tienes que identificar primero");
            
        }

        private void buttonEscolaridadElimina_Click(object sender, EventArgs e)
        {
            Datos.Instance.DeleteRegistroEscolaridad(escolaridad[indexEscolaridad.Pos]["Id1"].ToString());
            escolaridad = Datos.Instance.GetEscolaridad(IDFuncionario);
            indexEscolaridad = new IndiceBD(escolaridad.Length);
            LlenaEscolaridad(IDFuncionario);
        }

        private void buttonAPInicial_Click(object sender, EventArgs e)
        {
            if (indexAP.Inicializado())
            {
                indexAP.Inicial();
                LlenaAP(IDFuncionario);
            }
        }

        private void buttonAPPrevious_Click(object sender, EventArgs e)
        {
            if (indexAP.Inicializado())
            {
                indexAP.Previous();
                LlenaAP(IDFuncionario);
            }
        }

        private void buttonAPSiguiente_Click(object sender, EventArgs e)
        {
            if (indexAP.Inicializado())
            {
                indexAP.Next();
                LlenaAP(IDFuncionario);
            }
        }

        private void buttonAPFinal_Click(object sender, EventArgs e)
        {
            if (indexAP.Inicializado())
            {
                indexAP.Final();
                LlenaAP(IDFuncionario);
            }
        }

        private void buttonAPInserta_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                Datos.Instance.InsertRegistroAP(textBoxAPID.Text, muestraCapturaFechaAPInicio.Año.ToString(),
                    muestraCapturaFechaAPFin.Año.ToString(),
                textBoxAPPartido.Text, AbogadoIrresponsable);
                AP = Datos.Instance.GetAdscripcionPolitica(IDFuncionario);
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
            Datos.Instance.DeleteRegistroAP(AP[indexAP.Pos]["Id1"].ToString());
            AP = Datos.Instance.GetAdscripcionPolitica(IDFuncionario);
            indexAP = new IndiceBD(AP.Length);
            LlenaAP(IDFuncionario);
        }

        private void buttonINFOInicial_Click(object sender, EventArgs e)
        {
            if (indexINFO.Inicializado())
            {
                indexINFO.Inicial();
                LlenaINFO(IDFuncionario);
            }
        }

        private void buttonINFOPrevious_Click(object sender, EventArgs e)
        {
            if (indexINFO.Inicializado())
            {
                indexINFO.Previous();
                LlenaINFO(IDFuncionario);
            }
        }

        private void buttonINFOSiguiente_Click(object sender, EventArgs e)
        {
            if (indexINFO.Inicializado())
            {
                indexINFO.Next();
                LlenaINFO(IDFuncionario);
            }
        }

        private void buttonINFOFinal_Click(object sender, EventArgs e)
        {
            if (indexINFO.Inicializado())
            {
                indexINFO.Final();
                LlenaINFO(IDFuncionario);
            }
        }

        private void buttonINFOInserta_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(string.Empty))
            {
                if ((checkedListBoxTipoInformacion.SelectedIndex > -1) && (checkedListBoxTipoInformacion.SelectedIndex < checkedListBoxTipoInformacion.Items.Count))
                {
                    Datos.Instance.InsertRegistroINFO(textBoxINFOID.Text, checkedListBoxTipoInformacion.Text, textBoxINFOReferencia.Text,
                            AbogadoIrresponsable);
                    INFO = Datos.Instance.GetNotasRelevantes(IDFuncionario);
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
            Datos.Instance.DeleteRegistroINFO(INFO[indexINFO.Pos]["Id1"].ToString());
            INFO = Datos.Instance.GetNotasRelevantes(IDFuncionario);
            indexINFO = new IndiceBD(INFO.Length);
            LlenaINFO(IDFuncionario);
        }

        private void buttonPuestosInicial_Click(object sender, EventArgs e)
        {
            if (indexPuestos.Inicializado())
            {
                indexPuestos.Inicial();
                LlenaPuestos(IDFuncionario);
            }
        }

        private void buttonPuestosPrevious_Click(object sender, EventArgs e)
        {
            if (indexPuestos.Inicializado())
            {
                indexPuestos.Previous();
                LlenaPuestos(IDFuncionario);
            }
        }

        private void buttonPuestosSiguiente_Click(object sender, EventArgs e)
        {
            if (indexPuestos.Inicializado())
            {
                indexPuestos.Next();
                LlenaPuestos(IDFuncionario);
            }
        }

        private void buttonPuestosFinal_Click(object sender, EventArgs e)
        {
            if (indexPuestos.Inicializado())
            {
                indexPuestos.Final();
                LlenaPuestos(IDFuncionario);
            }
        }

        private void buttonPuestosInserta_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                Datos.Instance.InsertRegistroPuestos(textBoxPuestosID.Text, muestraCapturaFechaPuestoInicio.Año.ToString(), muestraCapturaFechaPuestoInicio.Mes.ToString(), muestraCapturaFechaPuestoInicio.Dia.ToString(),
                muestraCapturaFechaPuestoFin.Año.ToString(), muestraCapturaFechaPuestoFin.Mes.ToString(), muestraCapturaFechaPuestoFin.Dia.ToString(), textBoxPuestosDependencia.Text, textBoxPuestosPuesto.Text,
                textBoxPuestosSuperior.Text, checkBoxPuestosCargoActual.CheckState == CheckState.Checked ? "actual" :
                string.Empty, AbogadoIrresponsable);
                Puestos = Datos.Instance.GetPuestos(IDFuncionario);
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
            Datos.Instance.DeleteRegistroPuestos(Puestos[indexPuestos.Pos]["Id1"].ToString());
            Puestos = Datos.Instance.GetPuestos(IDFuncionario);
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
                Datos.Instance.InsertFuncionario(ID, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text,
                    textBoxNacionalidad.Text, muestraCapturaFechaNacimiento.Año.ToString(), muestraCapturaFechaNacimiento.Mes.ToString(),
                    muestraCapturaFechaNacimiento.Dia.ToString(), AbogadoIrresponsable);
                if (!NewFotoFileName.Equals(string.Empty))
                {
                    Datos.Instance.SubeFoto(ID, NewFotoFileName);
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
                Datos.Instance.UpdateFuncionario(textBoxID.Text, textBoxPrimerNombre.Text, textBoxSegundoNombre.Text, textBoxApellidoPaterno.Text, textBoxApellidoMaterno.Text, textBoxNacionalidad.Text,
                    muestraCapturaFechaNacimiento.Año.ToString(), muestraCapturaFechaNacimiento.Mes.ToString(), muestraCapturaFechaNacimiento.Dia.ToString(), AbogadoIrresponsable);
                if (FotoModificada) Datos.Instance.SubeFoto(textBoxID.Text, NewFotoFileName);
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
            funcionarios = Datos.Instance.GetFuncionarios();
            funcionarioMostrado = new IndiceBD(funcionarios.Length);
            funcionarioMostrado.Pos = DespliegaInformacionDelID(ID);
            if (funcionarioMostrado.Inicializado())
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
                int posBlanco = 0;
                if (e.Node.Text.Substring(0,1).Equals("#"))
                {
                    posBlanco = e.Node.Text.IndexOf(' ') + 1;
                    gion -= posBlanco;
                }
                textBoxOrgNombrePuestoModificado.Text = e.Node.Text.Substring(posBlanco, gion - 1);
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
            bool NodoDeAgrupación = false;
            if (NodoSeleccionado == null)
            {
                MessageBox.Show("Error NodoSeleccionado null " + AbogadoIrresponsable);
                return;
            }
            int PosiciónTipoID = NodoDeArbolMostrado.Text.IndexOf('_');
            if (NodoDeArbolMostrado.Text[PosiciónTipoID + 1] == 'O') NodoDeAgrupación = true;
            if (!AbogadoIrresponsable.Equals(""))
            {
                String IDMostrado = NodoSeleccionado.Data.ID; // ID del Nodo del Arbol que estamos viendo
                string ID; // ID de la ficha seleccionada
                Registro NuevoRegistro = null;
                Node<Registro> NuevoNode = null;
                if (ListaDeNodosPorID.ContainsKey(IDMostrado))
                {
                    switch (IndiceModificación)
                    {
                        case 1: // Modificamos un nodo de agrupación
                            if (NodoDeAgrupación)
                            {
                                Random rnd = new Random();
                                ID = "O" + rnd.Next(1, 100000).ToString();
                                while (ListaDeNodosPorID.ContainsKey(ID))
                                {
                                    ID = "O" + rnd.Next(1, 100000).ToString();
                                }
                                NodoDeArbolMostrado.Text = textBoxOrgNombrePuestoModificado.Text + "- _" + ID;
                                NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, textBoxOrgNombrePuestoModificado.Text, ID, -1, AbogadoIrresponsable, 0);
                                NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                                ListaDeNodosPorID.Add(ID, NuevoNode);
                                // tengo que actualizar APF
                                ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                                ListaDeNodosPorID.Remove(IDMostrado);
                            }
                            else
                            {
                                MessageBox.Show("Esta opción solo se aplica a los nodos de agrupación");
                                return;
                            }  
                            break;
                        case 2:  // Modificamos un Puesto
                            if (!NodoDeAgrupación)
                            {
                                string prefijo = string.Empty;
                                if (NodoSeleccionado.Data.Id1Presupuesto > 0)
                                {
                                    prefijo = "#" + NodoSeleccionado.Data.Id1Presupuesto + " ";
                                }
                                if (textBoxOrgNombrePuestoModificado.Text.Substring(0,1).Equals("#"))
                                {
                                    MessageBox.Show("No es valido introducir # en el nombre del puesto");
                                    return;
                                }
                                NodoDeArbolMostrado.Text = prefijo + textBoxOrgNombrePuestoModificado.Text + " - " + Datos.Instance.GetNombreFuncionario(IDMostrado) + "_" + IDMostrado;
                                NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, textBoxOrgNombrePuestoModificado.Text, IDMostrado, -1, AbogadoIrresponsable, NodoSeleccionado.Data.Id1Presupuesto);
                                NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                                // tengo que actualizar APF
                                ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                                ListaDeNodosPorID[IDMostrado] = NuevoNode;
                            }
                            else
                            {
                                MessageBox.Show("Esta opción no se puede aplicar a un nodo de agrupación");
                                return;
                            }
                            break;
                        case 3: // Modificamos un fucionario
                            if (!NodoDeAgrupación)
                            {
                                ID = funcionarios[funcionarioMostrado.Pos]["ID"].ToString();
                                int gion = NodoDeArbolMostrado.Text.IndexOf('-');
                                NodoDeArbolMostrado.Text = NodoDeArbolMostrado.Text.Substring(0, gion - 1) + " - " + Datos.Instance.GetNombreFuncionario(ID) + "_" + ID;
                                NuevoRegistro = new Registro(NodoSeleccionado.Data.TipoRegistro, NodoDeArbolMostrado.Text.Substring(0, gion - 1), ID, -1, AbogadoIrresponsable, NodoSeleccionado.Data.Id1Presupuesto);
                                NuevoNode = new Node<Registro>(NuevoRegistro, NodoSeleccionado.Sons, NodoSeleccionado.Padre);
                                ListaDeNodosPorID.Add(ID, NuevoNode);
                                // tengo que actualizar APF
                                ListaDeNodosPorID[IDMostrado].Data = NuevoRegistro;
                                ListaDeNodosPorID.Remove(IDMostrado);
                            }
                            else
                            {
                                MessageBox.Show("Esta opción no se puede aplicar a un nodo de agrupación");
                                return;
                            }  
                            break;
                        default:
                            MessageBox.Show("Se debe escoger alguna opción de modificación");
                            return;
                    }
                    NuevoRegistro.NodoDelTreeView = NodoDeArbolMostrado;
                    treeViewOrganigramaAPF.SelectedNode = null;
                    NodoDeArbolMostrado = null;
                    NodoSeleccionado = null;
                    organigrama.PrintTreeAPF(APF, ImprimeConsola);
                    textBoxOrgNombrePuestoModificado.Text = string.Empty;
                    IndiceModificación = 0;
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
                    MessageBox.Show("Nodo Seleccionado es nulo");
                    return;
                }

                if (!textBoxOrgNombrePuesto.Text.Equals(string.Empty) && !ListaDeNodosPorID.ContainsKey(ID) && !(NodoSeleccionado == null))
                {
                    if (textBoxOrgNombrePuesto.Text.Substring(0,1).Equals("#"))
                    {
                        MessageBox.Show("No es valido comenzar el nombre de puesto con #");
                        return;
                    }
                    // Hay que modificar APF y ListadeNodosPorID
                    // siempre vamos a inserta el nuevo registro como un hijo, y el único nodo que no puedo borrar es el raiz (el presidente)
                    NodeList<Registro> SinHijos = new NodeList<Registro>();
                    Registro NuevoRegistro = new Registro(NivelSIguiente[NodoSeleccionado.Data.TipoRegistro], textBoxOrgNombrePuesto.Text, ID, -1, AbogadoIrresponsable, 0);
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
                        newNode = new TreeNode(textBoxOrgNombrePuesto.Text + " - " + Datos.Instance.GetNombreFuncionario(ID) + "_" + ID);
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
            saveFileDialogTreeAPF.DefaultExt = "pdf";
            saveFileDialogTreeAPF.ShowDialog();
            if (!saveFileDialogTreeAPF.FileName.Equals(string.Empty))
            {
                var writer = new PdfWriter(saveFileDialogTreeAPF.FileName);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                int ImprimePDF(string line)
                {
                    document.Add(new Paragraph(line));
                    return 0;
                }
                organigrama.PrintTreeAPF(APF, ImprimePDF);
                document.Close();
                pdf.Close();
                writer.Close();
                System.Diagnostics.Process.Start(saveFileDialogTreeAPF.FileName);
            }
        }

        private void buttonListaDeNodosPorID_Click(object sender, EventArgs e)
        {
            saveFileDialogTreeAPF.DefaultExt = "pdf";
            saveFileDialogTreeAPF.ShowDialog();
            if (!saveFileDialogTreeAPF.FileName.Equals(string.Empty))
            {
                var writer = new PdfWriter(saveFileDialogTreeAPF.FileName);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                int ImprimePDF(string line)
                {
                    document.Add(new Paragraph(line));
                    return 0;
                }
                ImprimePDF("Tamaño de la lista: " + ListaDeNodosPorID.Count);
                foreach (var entry in ListaDeNodosPorID)
                {
                    ImprimePDF(entry.Key + " -- " + entry.Value.Data.ToString());
                }
                document.Close();
                pdf.Close();
                writer.Close();
                System.Diagnostics.Process.Start(saveFileDialogTreeAPF.FileName);
            }
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
            Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", -1, "El Sistema", 0);
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
            System.IO.File.Copy(Defines.DataBasePath + Defines.DataBaseFileName, Defines.DataBasePath + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_" + Defines.DataBaseFileName, true);
        }

        private void buttonOrgGuardar_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                ImprimeConsola("Guardamos el Organigrama");
                string MaxId1 = Datos.Instance.OrganigramaMaxId1();
                organigrama.SalvaTreeAPF(APF, ImprimeConsola, false);
                Datos.Instance.DeleteOrganigrama(MaxId1);
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
                Datos.Instance.InsertRegistroDatosContacto(textBoxDatosContactoID.Text, checkedListBoxTipoINFO.Text, textBoxDatosContactoDato.Text, labelDatosContactoAbogadoResp.Text);
                DatosContacto = Datos.Instance.GetDatosContacto(textBoxDatosContactoID.Text);
                indexDatosContacto = new IndiceBD(DatosContacto.Length);
                LlenaDatosContacto(textBoxDatosContactoID.Text);
            }
        }

        private void buttonDatosContactoInicial_Click(object sender, EventArgs e)
        {
            if (indexDatosContacto.Inicializado())
            {
                indexDatosContacto.Inicial();
                LlenaDatosContacto(IDFuncionario);
            }
        }

        private void buttonDatosContactoPrevio_Click(object sender, EventArgs e)
        {
            if (indexDatosContacto.Inicializado())
            {
                indexDatosContacto.Previous();
                LlenaDatosContacto(IDFuncionario);
            }
        }

        private void buttonDatosContactoSiguiente_Click(object sender, EventArgs e)
        {
            if (indexDatosContacto.Inicializado())
            {
                indexDatosContacto.Next();
                LlenaDatosContacto(IDFuncionario);
            }
        }

        private void buttonDatosContactoFinal_Click(object sender, EventArgs e)
        {
            if (indexDatosContacto.Inicializado())
            {
                indexDatosContacto.Final();
                LlenaDatosContacto(IDFuncionario);
            } 
        }

        private void buttonDatosContactoLimpia_Click(object sender, EventArgs e)
        {
            LimpiaDatosContacto(IDFuncionario);
        }

        private void buttonDatosContactoElimina_Click(object sender, EventArgs e)
        {
            Datos.Instance.DeleteRegistroDatosContacto(DatosContacto[indexDatosContacto.Pos]["Id1"].ToString());
            DatosContacto = Datos.Instance.GetDatosContacto(IDFuncionario);
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
                if (funcionarios[i]["ID"].ToString().Equals(textBoxRecuperaID.Text.ToUpper()))
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
            if (indexCirculoCercano.Inicializado())
            {
                indexCirculoCercano.Inicial();
                LlenaCirculoCercano(IDFuncionario);
            }
        }

        private void buttonCirculoCercanoPrevio_Click(object sender, EventArgs e)
        {
            if (indexCirculoCercano.Inicializado())
            {
                indexCirculoCercano.Previous();
                LlenaCirculoCercano(IDFuncionario);
            }
        }

        private void buttonCirculoCercanoSIguiente_Click(object sender, EventArgs e)
        {
            if (indexCirculoCercano.Inicializado())
            {
                indexCirculoCercano.Next();
                LlenaCirculoCercano(IDFuncionario);
            }
        }

        private void buttonCirculoCercanoFinal_Click(object sender, EventArgs e)
        {
            if (indexCirculoCercano.Inicializado())
            {
                indexCirculoCercano.Final();
                LlenaCirculoCercano(IDFuncionario);
            }
        }

        private void buttonCirculoCercanoInserta_Click(object sender, EventArgs e)
        {
            Datos.Instance.InsertRegistroCirculoCercano(textBoxCirculoCercanoID.Text, textBoxCirculoCercanoNombre.Text,
                    textBoxCirculoCercanoInformación.Text, labelCirculoCercanoAbogadoResp.Text);
            CirculoCercano = Datos.Instance.GetCirculoCercano(textBoxCirculoCercanoID.Text);
            indexCirculoCercano = new IndiceBD(CirculoCercano.Length);
            LlenaCirculoCercano(textBoxCirculoCercanoID.Text);
        }

        private void buttonCurculoCercanoLimpia_Click(object sender, EventArgs e)
        {
            LimpiaCirculoCercano(IDFuncionario);
        }

        private void buttonCirculoCercanoElimina_Click(object sender, EventArgs e)
        {
            Datos.Instance.DeleteRegistroCirculoCercano(CirculoCercano[indexCirculoCercano.Pos]["Id1"].ToString());
            CirculoCercano = Datos.Instance.GetCirculoCercano(IDFuncionario);
            indexCirculoCercano = new IndiceBD(CirculoCercano.Length);
            LlenaCirculoCercano(IDFuncionario);
        }

        private void LlenaPDFPresupuesto()
        {
            // PDF Presupuesto
            
        }

        string PrintHexaConsole(string cadena)
        {
            for (int j = 0; j < cadena.Length; j++)
            {
                Console.Write(cadena[j]);
                Console.Write(" ");
            }
            Console.WriteLine();
            byte[] ba = Encoding.Default.GetBytes(cadena);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        private void buttonVerificaOK_Click(object sender, EventArgs e)
        {
            if (Datos.Instance.VerificaAbogadoIrresponsable(textBoxAbogadoIrresponsable.Text, textBoxPassword.Text))
            {
                MessageBox.Show("-" + textBoxAbogadoIrresponsable.Text + "-" + 
                    PrintHexaConsole(textBoxAbogadoIrresponsable.Text) + "-" +
                    textBoxPassword.Text + "-" +
                    PrintHexaConsole(textBoxPassword.Text) + "-");
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
                Registro Presidente = new Registro("Presidencia", "Presidencia", "A0", -1, "El Sistema", 0);
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

        private void saveFileDialogTreeAPF_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            IndiceModificación++;
        }

        private void checkedListBoxTipoModificacion_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void buttonAsignaPresupuesto_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                if (NodoSeleccionado == null) //  Este nodo corresponde al árbol de funcionarios
                    MessageBox.Show("Error NodoSeleccionado null " + AbogadoIrresponsable);
                int PosiciónTipoID = NodoDeArbolMostrado.Text.IndexOf('_');   // este es el árbol que se muestra en windowsform
                if (NodoDeArbolMostrado.Text[PosiciónTipoID + 1] == 'O')
                {
                    MessageBox.Show("No se puede asignar un presupuesto a un nodo de agrupación");
                    return;
                }
                String IDMostrado = NodoSeleccionado.Data.ID; // ID del Nodo del Árbol que estamos viendo
                NodoSeleccionado.Data.Id1Presupuesto = Datos.Instance.GetID1Presupuesto(comboBoxPDFPresupuesto.Text);
                NodoDeArbolMostrado.Text = "#" + NodoSeleccionado.Data.Id1Presupuesto + " " + NodoDeArbolMostrado.Text;
                Datos.Instance.SetAsignadoID1Presupuesto(NodoSeleccionado.Data.Id1Presupuesto);
                LlenaComboBoxPresupuesto();
            }
            else
                MessageBox.Show("Tienes que identificarte Primero");
        }

       

        private void comboBoxPDFPresupuesto_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private bool PDFType(string FileName)
        {
            if (FileName != string.Empty)
            {
                switch (Path.GetExtension(FileName).ToLower())
                {
                    case ".pdf":
                        if (File.Exists(FileName))
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

        private void buttonInsertPDFFile_Click(object sender, EventArgs e)
        {
            if (!AbogadoIrresponsable.Equals(""))
            {
                openFileDialogPDFPresupuesto.ShowDialog();
                if (File.Exists(openFileDialogPDFPresupuesto.FileName) && PDFType(openFileDialogPDFPresupuesto.FileName))
                {
                    Datos.Instance.SubePDF(openFileDialogPDFPresupuesto.FileName, labelOrgAbogadoIrresponsable.Text);
                }
            }
            else
                MessageBox.Show("Tienes que identificarte primero");
                
        }

        
    }
}
