using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace LegalTropics
{
    public partial class Tropicalizador
    {
        // Create a TCP/IP  socket.  
        Socket sender1;

        private void Tropicalizador_Load(object sender, RibbonUIEventArgs e)
        {
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
                Regex regex = new Regex(@"- - [a-zA-Z0-9 ():-]+ - - - -");
                MatchCollection AllMatches = regex.Matches(Encoding.UTF8.GetString(bytes, 0, bytesRec));
                string result = string.Empty;
                if (AllMatches.Count > 0)
                {
                    foreach (Match someMatch in AllMatches)
                    {
                        result += someMatch.Groups[0].Value.Replace(" -","").Replace("- ","").Replace(" ","") + " ";
                    }
                }
                return "\n" + result;
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
    }

    public static class Utility
    {
        public static void InsertText(Selection currentSelection, string text)
        {
            if (currentSelection.Type == WdSelectionType.wdSelectionIP)
            {
                currentSelection.TypeText(text);
                currentSelection.TypeParagraph();
            }
            else
            {
                if (currentSelection.Type == WdSelectionType.wdSelectionNormal)
                {
                    if (Globals.ThisAddIn.Application.Options.ReplaceSelection)
                    {
                        object direction = WdCollapseDirection.wdCollapseStart;
                        currentSelection.Collapse(ref direction);
                    }
                    currentSelection.TypeText(text);
                    currentSelection.TypeParagraph();
                }
            }
        }
    }
}
