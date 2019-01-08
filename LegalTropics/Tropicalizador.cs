using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Traductor;
using System.Data;
using System.IO;
using Arboles;
using Globales;
using APFInfo;
using MSAccess;
using CifradoPeta;
using PetaPublish;
using FuncionesAuxiliares;
using OrganigramaAdmin;
using System.Threading;

namespace LegalTropics
{
    public partial class Tropicalizador
    {
        // Create a TCP/IP  socket.  
        Socket sender1;
        Dictionary<string, string> dicFuncionarios = new Dictionary<string, string>();
        public Node<Registro> APF;
        public NavegaciónAPF VentanaNavegación;
        public ImageScroll AparadorDeFotografias;
        public PuestosAPF VentanaPuestos;
        public FuncionariosAPF VentanaFuncionarios;
        
        enum Formato
        {
            Justificado = 0,
            Centrado = 1
        }

        enum Italic
        {
            Italicas = 1,
            NoItalicas = 0
        }

        enum Bold
        {
            Bold = 1,
            NoBold = 0
        }

        enum ALaLinea
        {
            NewLine = 1,
            NoNewLine = 0
        }

        private int ImprimeConsola(string line)
        {
            return 0;
        }

        Thread InicialicaBaseDeDatosThread;
        bool DataBaseInicializada = false;

        private void Tropicalizador_Load(object sender, RibbonUIEventArgs e)
        {
            VerificaDirectorios();
            if (!File.Exists(Defines.DataBasePath + Defines.DataBaseFileName))
            {
                DescargaBaseDeDatos();
            }
            InicialicaBaseDeDatosThread = new Thread(new ThreadStart(LoadDataBaseAPF));
            InicialicaBaseDeDatosThread.Start();
        }

        Dictionary<string, Node<Registro>> ListaDeNodosPorID = new Dictionary<string, Node<Registro>>();

        private void LoadDataBaseAPF()
        {
            Parser parser = new Parser(ImprimeConsola);
            Registro Presidente = new Registro("P", "Presidencia", "A0", "El Sistema");
            APF = new Node<Registro>(Presidente);
            parser.Parsea(APF, 0, ListaDeNodosPorID);
            // Console.WriteLine("Imprimimos arbol");
            // APF.Print();
            // Console.WriteLine("\nbuscamos ");
            // NodeList<Registro> lista = Parser.GetNodeListOf(APF, "DG 2");
            // for (int i = 0; i < lista.Count; i++)
            // {
            //     Console.WriteLine(lista[i].Value.ToString());
            // }

            RibbonDropDownItem item;
            DataRow[] funcionarios = AccessUtility.GetFuncionarios();
            for (int i = 0; i < funcionarios.Length; i++)
            {
                item = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                item.Label = funcionarios[i]["PrimerNombre"] + " " + funcionarios[i]["SegundoNombre"] + " " + funcionarios[i]["ApellidoPaterno"] + " " + funcionarios[i]["ApellidoMaterno"] + "_" + funcionarios[i]["ID"];
                //dicFuncionarios.Add(item.Label, funcionarios[i]["ID"].ToString());
                comboBoxFuncionarios.Items.Add(item);
            }

            //List<Registro> Puestos = APF.ListPuestos();
            DataRow[] Puestos = AccessUtility.GetDistinctPuestos();
            List<string> StringPuestos = new List<string>();
            for (int i = 0; i < Puestos.Length; i++)
            {
                StringPuestos.Add(Puestos[i]["Puesto"].ToString());
            }

            StringPuestos.Sort((x, y) => x.CompareTo(y));

            StringPuestos.Sort();

            for (int i = 0; i < StringPuestos.Count; i++)
            {
                item = Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem();
                item.Label = StringPuestos[i];
                comboBoxPuestos.Items.Add(item);
            }

            VentanaNavegación = new NavegaciónAPF();
            AparadorDeFotografias = new ImageScroll();
            VentanaPuestos = new PuestosAPF();
            VentanaFuncionarios = new FuncionariosAPF();

            DataBaseInicializada = true;
        }

        private IPAddress parse(string ipAddress)
        {
            try
            {
                // Create an instance of IPAddress for the specified address string (in 
                // dotted-quad, or colon-hexadecimal notation).
                IPAddress address = IPAddress.Parse(ipAddress);
                return address;
            }

            catch (ArgumentNullException)
            {
                return IPAddress.None;
            }

            catch (FormatException)
            {
                return IPAddress.None;
            }

            catch (Exception)
            {
                return IPAddress.None;
            }
        }

        private string CallAnalyzer(string TextToAnalyze)
        {
            if (!buttonAnalyzer.Label.Equals("Analyzer Abierto")) return ("Server not running");
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1000000];
            /*
            IPEndPoint remoteEP;
            if (ipaddress.Text.Equals(""))
            {
                remoteEP = new IPEndPoint(Globals.ThisAddIn.ipAddress, 5000);
            }
            else
            {
                IPAddress ipdir = parse(ipaddress.Text);
                if (ipdir.Equals(IPAddress.None))
                {
                    MessageBox.Show("Error en la forma de la dirección IP");
                }
                remoteEP = new IPEndPoint(ipdir, 5000);
            }

            // Create a TCP/IP  socket.  
            Socket sender1 = new Socket(Globals.ThisAddIn.ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            */
            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                //sender1.Connect(remoteEP);

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.UTF8.GetBytes("RESET_STATS" + char.MinValue);

                // Send the data through the socket.  
                int bytesSent = sender1.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender1.Receive(bytes);
                string response = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                
                if (response.Equals("FL-SERVER-READY" + char.MinValue))
                {
                    buttonAnalyzer.Label = "Analyzer Abierto";
                }
                else
                {
                    buttonAnalyzer.Label = "Analyzer Cerrado";
                }

                msg = Encoding.UTF8.GetBytes(TextToAnalyze + char.MinValue);
                bytesSent = sender1.Send(msg);

                bytesRec = sender1.Receive(bytes);
                // Release the socket.
                //sender1.Shutdown(SocketShutdown.Both);
                //sender1.Close();

                Compilador lc = new Compilador(Encoding.UTF8.GetString(bytes, 0, bytesRec));
                return /* "\n" + lc.result + */ "\n" + lc.result1; // + "\n" + lc.result2;
            }
            catch (ArgumentNullException ane)
            {
                return("ArgumentNullException : " + ane.ToString());
            }
            catch (SocketException se)
            {
                return("SocketException : " + se.ToString());
            }
            catch (Exception ex)
            {
                return("Unexpected exception : " + ex.ToString());
            }
        }

        private void buttonAnalyzer_Click(object sender, RibbonControlEventArgs e)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            IPEndPoint remoteEP;
            if (ipaddress.Text.Equals(""))
            {
                remoteEP = new IPEndPoint(Globals.ThisAddIn.ipAddress, 5000);
            }
            else
            {
                IPAddress ipdir = parse(ipaddress.Text);
                if (ipdir.Equals(IPAddress.None))
                {
                    MessageBox.Show("Error en la forma de la dirección IP");
                }
                remoteEP = new IPEndPoint(ipdir, 5000);
            }
           
            // Create a TCP/IP  socket.  
            sender1 = new Socket(Globals.ThisAddIn.ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender1.Connect(remoteEP);

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.UTF8.GetBytes("RESET_STATS" + char.MinValue);

                // Send the data through the socket.  
                int bytesSent = sender1.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender1.Receive(bytes);
                string response = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                //MessageBox.Show("Echoed test = >" + response + "<");
                if (response.Equals("FL-SERVER-READY" + char.MinValue))
                {
                    buttonAnalyzer.Label = "Analyzer Abierto";
                }
                else
                {
                    buttonAnalyzer.Label = "Analyzer Cerrado";
                }
               
                msg = Encoding.UTF8.GetBytes("esperé a que volvieras." + char.MinValue);
                bytesSent = sender1.Send(msg);

                bytesRec = sender1.Receive(bytes);
                //MessageBox.Show(
                //    Encoding.UTF8.GetString(bytes, 0, bytesRec));

                // Release the socket.  
                //sender1.Shutdown(SocketShutdown.Both);
                //sender1.Close();

            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show("ArgumentNullException : " + ane.ToString());
            }
            catch (SocketException se)
            {
                MessageBox.Show("SocketException : " + se.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected exception : " + ex.ToString());
            }
        }

        private void StartLocalServer_Click(object sender, RibbonControlEventArgs e)
        {
            if (StartLocalServer.Label.Equals("Start Local Server"))
            {
                ipaddress.Text = "127.0.0.1";
                Globals.ThisAddIn.AnalyzerProcess.Exited += AnalyzerProcess_Exited;
                
                Globals.ThisAddIn.AnalyzerProcess.Start();
                System.Threading.Thread.Sleep(8000);
                StartLocalServer.Label = "Stop " + Globals.ThisAddIn.AnalyzerProcess.ProcessName;
                buttonAnalyzer.Enabled = true;
                buttonParrafos.Enabled = true;
            }
            else
            {
                try
                {
                    if (!Globals.ThisAddIn.AnalyzerProcess.HasExited)
                        Globals.ThisAddIn.AnalyzerProcess.Kill();
                }
                catch (InvalidOperationException) { };
                StartLocalServer.Label = "Start Local Server";
                buttonAnalyzer.Enabled = false;
                buttonParrafos.Enabled = false;
            }
        }

        private void AnalyzerProcess_Exited(object sender, EventArgs e)
        {
            //MessageBox.Show("Murio Analyzer");
            buttonAnalyzer.Enabled = false;
            buttonAnalyzer.Label = "Analyzer";
            buttonParrafos.Enabled = false;
            StartLocalServer.Label = "Start Local Server";
        }

        private void ipaddress_TextChanged(object sender, RibbonControlEventArgs e)
        {

        }

        private void buttonParrafos_Click(object sender, RibbonControlEventArgs e)
        {
            object start = Globals.ThisAddIn.Application.ActiveDocument.Content.Start;
            object end = Globals.ThisAddIn.Application.ActiveDocument.Content.End;
            Range rng = Globals.ThisAddIn.Application.ActiveDocument.Range(ref start, ref end);
            rng.Select();
            
            foreach(Range range in Globals.ThisAddIn.Application.ActiveDocument.Sentences)
            {
                object fin = range.End - 1;
                Range ran = Globals.ThisAddIn.Application.ActiveDocument.Range(ref fin, ref fin);
                ran.Select();
                Selection currentSelection = Globals.ThisAddIn.Application.Selection;

                string analisis = CallAnalyzer(range.Text.Replace("\r", ""));
                string textoAInsertar = analisis;
                Utility.InsertText(currentSelection, textoAInsertar);
            }
        }

        string linea;

        private void CRLF(Range rng, float size, Italic it, string FontFamily,  Formato fmt, Bold negritas, ALaLinea NewLine)
        {
            rng.Text = (NewLine == ALaLinea.NewLine) ? linea + "\n" : linea;
            rng.Font.Name = FontFamily;
            rng.Font.Bold = (negritas == Bold.Bold) ? 1 : 0;
            switch (fmt)
            {
                case Formato.Justificado:
                    rng.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                    break;
                case Formato.Centrado:
                    rng.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    break;
                default:
                    break;
            }
            rng.Font.Size = size;
            rng.Font.Italic = (it == Italic.Italicas) ? 1 : 0;
            rng.Collapse(WdCollapseDirection.wdCollapseEnd);
            rng.Select();
        }

        private void WriteLine(Range rango, string text, float size, Italic it, string FontFamily, Formato fmt, Bold negritas, ALaLinea NewLine)
        {
            linea = text;
            CRLF(rango, size, it, FontFamily, fmt, negritas, NewLine);
        }

        private string PuestoActual(DataRow[] puestos)
        {
            if (puestos.Length > 0)
            {
                for (int i = 0; i < puestos.Length; i++)
                {
                    if (puestos[i]["CargoActual"].ToString().Equals("1")) return puestos[i]["Puesto"].ToString();
                }
            }
            return string.Empty;
        }

        private void ImprimeDatosFuncionarios(Range rng, DataRow[] funcionarios)
        {
            Object oMissing = System.Reflection.Missing.Value;
            int inicio = rng.Start;
            for (int i = 0; i < funcionarios.Length; i++)
            {
                float PageWidthPoints = Globals.ThisAddIn.Application.ActiveDocument.PageSetup.PageWidth;
                string nombre = funcionarios[i]["PrimerNombre"] + " " + funcionarios[i]["SegundoNombre"] + " " + funcionarios[i]["ApellidoPaterno"] + " " + funcionarios[i]["ApellidoMaterno"];
                WriteLine(rng, nombre, (float)16, Italic.NoItalicas, "Century Gothic", Formato.Centrado, Bold.Bold, ALaLinea.NewLine);
                DataRow[] puestos = AccessUtility.GetPuestos(funcionarios[i]["ID"].ToString());
                if (puestos.Length > 0)
                {
                    //WriteLine(rng, puestos[puestos.Length - 1]["Puesto"].ToString(), (float)10.5, Italic.NoItalicas, "Century Gothic", Formato.Centrado, Bold.NoBold, ALaLinea.NewLine);
                    WriteLine(rng, puestos[puestos.Length - 1]["DependenciaEntidad"].ToString(), (float)10.5, Italic.NoItalicas, "Century Gothic", Formato.Centrado, Bold.NoBold, ALaLinea.NewLine);
                }

                /* Poner la foto aquí */
                WriteLine(rng, "\n", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                string FullName = AccessUtility.GetFoto(funcionarios[i]["ID"].ToString());
                if (FullName != String.Empty)
                {
                    object PhotoPos = rng;

                    ((Range) PhotoPos).Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    object tr = true;
                    object fa = true;
                    InlineShape shape = Globals.ThisAddIn.Application.ActiveDocument.InlineShapes.AddPicture(FullName, ref tr, ref fa, ref PhotoPos);
                    
                    float alto = shape.Height;
                    float ancho = shape.Width;
                    float anchoDoc = Globals.ThisAddIn.Application.ActiveDocument.PageSetup.PageWidth;
                    float AnchoUtilizable = anchoDoc - Globals.ThisAddIn.Application.ActiveDocument.PageSetup.LeftMargin - Globals.ThisAddIn.Application.ActiveDocument.PageSetup.RightMargin;
                    shape.Width = (float)0.25 * AnchoUtilizable;
                    shape.Height = (alto / ancho) * (float)0.25 * AnchoUtilizable;
                    Shape ShapeFoto = shape.ConvertToShape();
                    InlineShape shape1 = Globals.ThisAddIn.Application.ActiveDocument.InlineShapes.AddPicture(FullName, ref tr, ref fa, ref PhotoPos);
                    shape1.Height = (alto / ancho) * (float)0.25 * AnchoUtilizable;
                    shape1.Width = 0;
                    ShapeFoto.Left = (float)(0.75 / 2.0) * (anchoDoc - Globals.ThisAddIn.Application.ActiveDocument.PageSetup.LeftMargin - Globals.ThisAddIn.Application.ActiveDocument.PageSetup.RightMargin);
                    Range rangoFoto = shape.Range;
                    rangoFoto.Collapse(WdCollapseDirection.wdCollapseEnd);
                    rangoFoto.Select();
                    rng = rangoFoto;
                    rng.Collapse(WdCollapseDirection.wdCollapseEnd);
                    rng.Select();
                    FileInfo fi = new FileInfo(FullName);
                    if (fi.Exists) File.Delete(FullName);
                    //WriteLine(rng, "\n" + alto.ToString() + " " + ancho.ToString() + " " + anchoDoc.ToString(), (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NewLine);
                }

                WriteLine(rng, "\n", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NewLine);

                Table tabla1 = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(rng, 5, 2);

                tabla1.AllowPageBreaks = true;
                tabla1.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                tabla1.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                tabla1.PreferredWidth = PageWidthPoints;
                tabla1.Columns[1].SetWidth(PageWidthPoints * (float) (0.15), WdRulerStyle.wdAdjustSameWidth);

                tabla1.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                //WriteLine(tabla1.Cell(1, 1).Range, "Nombre Completo", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                //string nombreCompleto = funcionarios[i]["PrimerNombre"] + " " + funcionarios[i]["SegundoNombre"] + " " + funcionarios[i]["ApellidoPaterno"] + " " + funcionarios[i]["ApellidoMaterno"];
                //WriteLine(tabla1.Cell(1, 2).Range, nombreCompleto, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                tabla1.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                WriteLine(tabla1.Cell(1, 1).Range, "Fecha de nacimiento", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                WriteLine(tabla1.Cell(1, 2).Range,
                    FormatoFecha.FechaString(funcionarios[i]["AñoNacimiento"].ToString(),
                        funcionarios[i]["MesNacimiento"].ToString(),
                        funcionarios[i]["DiaNacimiento"].ToString(), "Fecha: no disponible"),
                    (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                //         Adscripción Política
                tabla1.Cell(2, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                WriteLine(tabla1.Cell(2, 1).Range, "Partido", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                DataRow[] adscripcionPolitica = AccessUtility.GetAdscripcionPolitica(funcionarios[i]["ID"].ToString());
                string HistoriaPartidista = "";
                for (int j = adscripcionPolitica.Length - 1; j >= 0; j--)
                {
                    HistoriaPartidista += adscripcionPolitica[j]["NombreDelPartido"].ToString().Replace("\n", string.Empty) + " " +
                        adscripcionPolitica[j]["FechaDeInicio"].ToString() + " " +
                        adscripcionPolitica[j]["FechaDeFin"].ToString();
                    HistoriaPartidista += (j == 0) ? string.Empty : "\n";
                }
                tabla1.Cell(2, 2).Range.ListFormat.ApplyNumberDefault(ref oMissing);
                ListTemplate lt1 = tabla1.Cell(2, 2).Range.ListFormat.ListTemplate;
                tabla1.Cell(2, 2).Range.ListFormat.ApplyListTemplate(lt1, false);

                WriteLine(tabla1.Cell(2, 2).Range, HistoriaPartidista, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                //      Cargo Actual
                tabla1.Cell(3, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                WriteLine(tabla1.Cell(3, 1).Range, "Cargo Actual", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                WriteLine(tabla1.Cell(3, 2).Range, PuestoActual(puestos), (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                //     Datos de Contacto
                tabla1.Cell(4, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                WriteLine(tabla1.Cell(4, 1).Range, "Datos de Contacto", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                DataRow[] DatosContacto = AccessUtility.GetDatosDeContacto(funcionarios[i]["ID"].ToString());
                string StringDatosContacto = "";
                for (int j = 0; j < DatosContacto.Length; j--)
                {
                    StringDatosContacto += DatosContacto[j]["Tipo"].ToString().Replace("\n", string.Empty) + ": " +
                        DatosContacto[j]["dato"].ToString();
                    StringDatosContacto += (j == 0) ? string.Empty : "\n";
                }
                if (DatosContacto.Length > 0 )
                {
                    tabla1.Cell(4, 2).Range.ListFormat.ApplyNumberDefault(ref oMissing);
                    ListTemplate lt2 = tabla1.Cell(4, 2).Range.ListFormat.ListTemplate;
                    tabla1.Cell(4, 2).Range.ListFormat.ApplyListTemplate(lt2, false);
                    WriteLine(tabla1.Cell(4, 2).Range, StringDatosContacto, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                }
                   

                //tabla1.Cell(4, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                //WriteLine(tabla1.Cell(4, 1).Range, "Datos de Contacto", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                //WriteLine(tabla1.Cell(4, 2).Range, "", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                //     Circulo Cercano
                tabla1.Cell(5, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                WriteLine(tabla1.Cell(5, 1).Range, "Circulo Cercano", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                DataRow[] CirculoCercano = AccessUtility.GetCirculoCercano(funcionarios[i]["ID"].ToString());
                string StringCirculoCercano = "";
                for (int j = 0; j < CirculoCercano.Length; j--)
                {
                    StringCirculoCercano += CirculoCercano [j]["Nombre"].ToString().Replace("\n", string.Empty) + ": " +
                        CirculoCercano[j]["Información"].ToString();
                    StringCirculoCercano += (j == 0) ? string.Empty : "\n";
                }
                if (CirculoCercano.Length > 0)
                {
                    tabla1.Cell(5, 2).Range.ListFormat.ApplyNumberDefault(ref oMissing);
                    ListTemplate lt3 = tabla1.Cell(5, 2).Range.ListFormat.ListTemplate;
                    tabla1.Cell(5, 2).Range.ListFormat.ApplyListTemplate(lt3, false);
                    WriteLine(tabla1.Cell(5, 2).Range, StringCirculoCercano, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                }
                    

                //tabla1.Cell(5, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                //WriteLine(tabla1.Cell(5, 1).Range, "Circulo Cercano", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                //WriteLine(tabla1.Cell(5, 2).Range, "", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                rng = Globals.ThisAddIn.Application.ActiveDocument.Range(Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1, Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1);

                DataRow[] escolaridad = AccessUtility.GetEscolaridad(funcionarios[i]["ID"].ToString());
                if (escolaridad.Length > 0)
                {
                    rng.Select();

                    WriteLine(rng, " ", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    Table tabla2 = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(rng, 1, 2);

                    tabla2.AllowPageBreaks = true;
                    tabla2.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    tabla2.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    tabla2.PreferredWidth = PageWidthPoints;
                    tabla2.Columns[1].SetWidth(PageWidthPoints * (float)(0.15), WdRulerStyle.wdAdjustSameWidth);

                    tabla2.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                    WriteLine(tabla2.Cell(1, 1).Range, "Estudios", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    string datosEscolares = "";
                    for (int j = 0; j < escolaridad.Length; j++)
                    {
                        datosEscolares += escolaridad[j]["Universidad"] + "  " + escolaridad[j]["Grado"] + " - " + 
                            FormatoFecha.FechaString(escolaridad[j]["AñoFinal"].ToString(),
                                escolaridad[j]["MesFinal"].ToString(),
                                escolaridad[j]["DiaFinal"].ToString(), "Fecha: no disponible");
                        datosEscolares += (j == escolaridad.Length - 1) ? string.Empty : "\n";
                    }
                    tabla2.Cell(1, 2).Range.ListFormat.ApplyBulletDefault(ref oMissing);
                    WriteLine(tabla2.Cell(1, 2).Range, datosEscolares, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                    rng = Globals.ThisAddIn.Application.ActiveDocument.Range(Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1, Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1);
                }

                if (puestos.Length > 0)
                {
                    rng.Select();

                    WriteLine(rng, " ", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    Table tabla3 = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(rng, 1, 2);

                    tabla3.AllowPageBreaks = true;
                    tabla3.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    tabla3.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    tabla3.PreferredWidth = PageWidthPoints;
                    tabla3.Columns[1].SetWidth(PageWidthPoints * (float)(0.20), WdRulerStyle.wdAdjustSameWidth);

                    tabla3.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                    WriteLine(tabla3.Cell(1, 1).Range, "Trayectoria", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                    string trayectoria = "";
                    for (int j = 0; j < puestos.Length; j++)
                    {
                        trayectoria += puestos[j]["Puesto"] + " - " + puestos[j]["DependenciaEntidad"] + " - " +
                            FormatoFecha.FechaString(puestos[j]["AñoInicial"].ToString(),
                                puestos[j]["MesInicial"].ToString(),
                                puestos[j]["DiaInicial"].ToString(), string.Empty) + "   " +
                            FormatoFecha.FechaString(puestos[j]["AñoFinal"].ToString(),
                                puestos[j]["MesFinal"].ToString(),
                                puestos[j]["DiaFinal"].ToString(), "Fecha: no disponible");
                        trayectoria += (j == puestos.Length - 1) ? string.Empty : "\n";
                    }
                    tabla3.Cell(1, 2).Range.ListFormat.ApplyBulletDefault(ref oMissing);
                    WriteLine(tabla3.Cell(1, 2).Range, trayectoria, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                    rng = Globals.ThisAddIn.Application.ActiveDocument.Range(Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1, Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1);
                }

                DataRow[] notas = AccessUtility.GetNotasRelevantes(funcionarios[i]["ID"].ToString());
                if (notas.Length > 0)
                {
                    rng.Select();

                    WriteLine(rng, " ", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    Table tabla4 = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(rng, 1, 2);

                    tabla4.AllowPageBreaks = true;
                    tabla4.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    tabla4.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    tabla4.PreferredWidth = PageWidthPoints;
                    tabla4.Columns[1].SetWidth(PageWidthPoints * (float)(0.20), WdRulerStyle.wdAdjustSameWidth);

                    tabla4.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                    WriteLine(tabla4.Cell(1, 1).Range, "Notas relevantes", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    string notasRelevantes = "";
                    for (int j = 0; j < notas.Length; j++)
                    {
                        notasRelevantes += notas[j]["Referencia"].ToString().Replace("\n\n", "\n");
                        notasRelevantes += (j == notas.Length - 1) ? string.Empty : "\n";
                    }
                    tabla4.Cell(1, 2).Range.ListFormat.ApplyNumberDefault(ref oMissing);
                    ListTemplate lt = tabla4.Cell(1, 2).Range.ListFormat.ListTemplate;
                    tabla4.Cell(1, 2).Range.ListFormat.ApplyListTemplate(lt , false);
                    WriteLine(tabla4.Cell(1, 2).Range, notasRelevantes, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    rng = Globals.ThisAddIn.Application.ActiveDocument.Range(Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1, Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1);
                }

                DataRow[] comentarios = AccessUtility.GetComentarios(funcionarios[i]["ID"].ToString());
                if (comentarios.Length > 0)
                {
                    rng.Select();

                    WriteLine(rng, " ", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    Table tabla5 = Globals.ThisAddIn.Application.ActiveDocument.Tables.Add(rng, 1, 2);

                    tabla5.AllowPageBreaks = true;
                    tabla5.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                    tabla5.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

                    tabla5.PreferredWidth = PageWidthPoints;
                    tabla5.Columns[1].SetWidth(PageWidthPoints * (float)(0.20), WdRulerStyle.wdAdjustSameWidth);

                    tabla5.Cell(1, 1).Shading.BackgroundPatternColor = (Microsoft.Office.Interop.Word.WdColor)Defines.ColorInstitucional;
                    WriteLine(tabla5.Cell(1, 1).Range, "Comentarios", (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);

                    string comments = "";
                    for (int j = 0; j < comentarios.Length; j++)
                    {
                        comments += comentarios[j]["Referencia"].ToString().Replace("\n\n", "\n");
                        comments += (j == comentarios.Length - 1) ? string.Empty : "\n";
                    }
                    tabla5.Cell(1, 2).Range.ListFormat.ApplyBulletDefault(ref oMissing);
                    WriteLine(tabla5.Cell(1, 2).Range, comments, (float)10, Italic.NoItalicas, "Century Gothic", Formato.Justificado, Bold.NoBold, ALaLinea.NoNewLine);
                    rng = Globals.ThisAddIn.Application.ActiveDocument.Range(Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1, Globals.ThisAddIn.Application.ActiveDocument.Content.End - 1);
                }

                // insertamos un salto de pagina al final de los datos de cada funcionario
                //Utility.InsertText(currentSelection, "==========================================================");
                object type = 7;
                rng.InsertBreak(ref type);
                int fin = rng.End;
                Range rango = Globals.ThisAddIn.Application.ActiveDocument.Range(inicio, fin);
                rango.Paragraphs.Space1();
                rango.Paragraphs.SpaceBefore = 0;
                rango.Paragraphs.SpaceAfter = 0;

                rng.Collapse(WdCollapseDirection.wdCollapseEnd);
                rng.Select();
                
            }
        }

        public void DelteBlankPages()
        {

            // Get application object
            //Microsoft.Office.Interop.Word.Application WordApplication = new Microsoft.Office.Interop.Word.Application();

            // Get document object
            object Miss = System.Reflection.Missing.Value;
            object ReadOnly = false;
            object Visible = false;
            Document Doc = Globals.ThisAddIn.Application.ActiveDocument; 
            // WordApplication.Documents.Open(ref Path, ref Miss, ref ReadOnly, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Visible, ref Miss, ref Miss, ref Miss, ref Miss);

            // Get pages count
            Microsoft.Office.Interop.Word.WdStatistic PagesCountStat = Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages;
            int PagesCount = Doc.ComputeStatistics(PagesCountStat, ref Miss);

            //Get pages
            object What = Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage;
            object Which = Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToAbsolute;
            object Start;
            object End;
            object CurrentPageNumber;
            object NextPageNumber;

            for (int Index = 1; Index < PagesCount + 1; Index++)
            {
                CurrentPageNumber = (Convert.ToInt32(Index.ToString()));
                NextPageNumber = (Convert.ToInt32((Index + 1).ToString()));

                // Get start position of current page
                Start = Globals.ThisAddIn.Application.Selection.GoTo(ref What, ref Which, ref CurrentPageNumber, ref Miss).Start;

                // Get end position of current page                                
                End = Globals.ThisAddIn.Application.Selection.GoTo(ref What, ref Which, ref NextPageNumber, ref Miss).End;

                // Get text
                Int32 inicio = Convert.ToInt32(Start.ToString());
                Int32 fin = Convert.ToInt32(End.ToString());
                if (System.Math.Abs(inicio - fin)  < 4) {
                    Doc.Range(ref Start, ref End).Delete();
                }
            }
        }

        private void buttonGeneraReporte_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                DataRow[] funcionarios = AccessUtility.GetFuncionarios();
                //Range rng = Globals.ThisAddIn.Application.ActiveDocument.Range(0, 0);
                Range rng = Globals.ThisAddIn.Application.Selection.Range;
                ImprimeDatosFuncionarios(rng, funcionarios);
                DelteBlankPages();
            }
        }

        public void GeneraReporte(string ID)
        {
            DataRow[] funcionarios = AccessUtility.GetFuncionario(ID);
            //Range rng = Globals.ThisAddIn.Application.ActiveDocument.Range(0, 0);
            Range rng = Globals.ThisAddIn.Application.Selection.Range;
            if (funcionarios.Length > 1)
            {
                MessageBox.Show("Error grave en la base de datos.\nComunicarse con la DNCP");
            } 
            else
            {
                ImprimeDatosFuncionarios(rng, funcionarios);
            }
        }

        private void comboBoxFuncionarios_TextChanged(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                int pos = comboBoxFuncionarios.Text.IndexOf('_');
                string ID = comboBoxFuncionarios.Text.Substring(pos + 1, comboBoxFuncionarios.Text.Length - pos - 1);
                GeneraReporte(ID);
            }
        }

        private void comboBoxPuestos_TextChanged(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                DataRow[] IDs = AccessUtility.GetIDPuestos(comboBoxPuestos.Text);
                for (int i = 0; i < IDs.Length; i++)
                {
                    GeneraReporte(IDs[i]["ID"].ToString());
                }
            }
        }

        

        private void VentanaNavegación_Deactivate(object sender, System.EventArgs e)
        {
            VentanaNavegación.Hide();
        }

        private void VentanaPuestos_Deactivate(object sender, System.EventArgs e)
        {
            VentanaPuestos.Hide();
        }

        private void VentanaFuncionarios_Deactivate(object sender, System.EventArgs e)
        {
            VentanaFuncionarios.Hide();
        }

        private void buttonOrganigrama_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                VentanaNavegación.Deactivate += VentanaNavegación_Deactivate;
                VentanaNavegación.treeViewAPF.Enabled = false;
                VentanaNavegación.Show();
                VentanaNavegación.treeViewAPF.Enabled = true;
            }
        }


        private void AparadorDeFotografias_Deactivate(object sender, EventArgs e)
        {
            AparadorDeFotografias.Hide();
        }

        private void buttonCatalogoFotos_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                AparadorDeFotografias.Deactivate += AparadorDeFotografias_Deactivate;
                AparadorDeFotografias.Show();
            }
        }

        // TODO: Checar que existan los directorios de fotos y de la base de datos, sino existen crearlos
        // TODO: Checar que exista la base de datos si no existe descargarla

        private void VerificaDirectorios()
        {
            if (!Directory.Exists(Defines.FotoTempBasePath))
            {
                Directory.CreateDirectory(Defines.FotoTempBasePath);
            }
            if (!Directory.Exists(Defines.DataBasePath))
            {
                Directory.CreateDirectory(Defines.DataBasePath);
            }
        }

        private void DescargaBaseDeDatos()
        {
            PetaSecure cipher = new PetaSecure();
            if (File.Exists(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado))
            {
                System.IO.File.Delete(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado);
            }
            if (Publisher.DownloadInfoAPFDB(Defines.DataBasePath, Defines.DataBaseFileNameEncriptado) != 0)
            {
                if (File.Exists(Defines.DataBasePath + Defines.DataBaseFileName))
                {
                    System.IO.File.Delete(Defines.DataBasePath + Defines.DataBaseFileName);
                }
                cipher.FileDecrypt(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado,
                Defines.DataBasePath + Defines.DataBaseFileName,
                System.Text.Encoding.UTF8.GetString(Defines.ImagenDefault).Substring(Defines.PosInicial, Defines.PosFinal));
                if (File.Exists(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado))
                {
                    System.IO.File.Delete(Defines.DataBasePath + Defines.DataBaseFileNameEncriptado);
                }
            }
        }

        private void buttonActualizaBaseDeDatos_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                DataBaseInicializada = false;
                DescargaBaseDeDatos();
                InicialicaBaseDeDatosThread = new Thread(new ThreadStart(LoadDataBaseAPF));
                InicialicaBaseDeDatosThread.Start();
                MessageBox.Show("Base de Datos descargada");
            }
            
        }

        private void buttonPuestos_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                VentanaPuestos.Deactivate += VentanaPuestos_Deactivate;
                VentanaPuestos.treeViewPuestos.Enabled = false;
                VentanaPuestos.Show();
                VentanaPuestos.treeViewPuestos.Enabled = true;
            }
        }

        private void buttonFuncionarios_Click(object sender, RibbonControlEventArgs e)
        {
            if (DataBaseInicializada)
            {
                VentanaFuncionarios.Deactivate += VentanaFuncionarios_Deactivate;
                VentanaFuncionarios.treeViewFuncionarios.Enabled = false;
                VentanaFuncionarios.Show();
                VentanaFuncionarios.treeViewFuncionarios.Enabled = true;
            }
        }
    }
}

