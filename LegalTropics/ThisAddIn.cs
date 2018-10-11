using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Linq.Expressions;

namespace LegalTropics
{
    public partial class ThisAddIn
    {
        public Process AnalyzerProcess = new Process();
        public string LocalHostName = string.Empty; // para tener el nombre de la máquina
        public IPAddress ipAddress;                 // local ip address

        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    if (IPAddress.Parse(hostIP.ToString()).AddressFamily == AddressFamily.InterNetwork)
                    {
                        // is localhost
                        if (IPAddress.IsLoopback(hostIP)) return true;
                        // is local address
                        foreach (IPAddress localIP in localIPs)
                        {
                            if (IPAddress.Parse(localIP.ToString()).AddressFamily == AddressFamily.InterNetwork)
                                if (hostIP.Equals(localIP)) return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private void InicializaDatosIP()
        {
            // Get the local Hostname and IPAddress
            try
            {
                // Establish the remote endpoint for the socket.  
                // Analyzer Server uses port 5000 on the local computer.
                LocalHostName = Dns.GetHostName();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(LocalHostName); // Gets the host name of the local computer
                int i = 0;
                for (i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    if (IsLocalIpAddress(ipHostInfo.AddressList[i].ToString())) break;
                }
                ipAddress = ipHostInfo.AddressList[i];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Arrancamos el servicio de Freeling como servidor para poder usarlo desde Word
            // TODO: Llamar a analyzer a través de sockets y recuperar la salida 
            // TODO: Almacenar la salida de Analyzer en una estructura de datos apropiada para procesar

            AnalyzerProcess.StartInfo.UseShellExecute = false;
            AnalyzerProcess.StartInfo.FileName = @"D:\freeling-4.0-win64\freeling\bin\analyzer.exe";
            AnalyzerProcess.StartInfo.Arguments = @" -f D:\mx.cfg --outlv parsed --output conll --workers 5 --server --port 5000 "; //argument
            AnalyzerProcess.StartInfo.CreateNoWindow = true;
            

            Application.WindowDeactivate += Application_WindowDeactivate;

            InicializaDatosIP();
            Globals.Ribbons.Tropicalizador.buttonParrafos.Enabled = false;
            Globals.Ribbons.Tropicalizador.buttonAnalyzer.Enabled = false;
        }

        private void Application_WindowDeactivate(Word.Document Doc, Word.Window Wn)
        {
            //MessageBox.Show("Cerramos intenpestivamente word");

            try
            {
                if (!AnalyzerProcess.HasExited)
                    AnalyzerProcess.Kill();
            }
            catch (InvalidOperationException) { }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Matamos el proceso de analisis linguístico al terminar el addin
            try
            {
                if (!AnalyzerProcess.HasExited)
                    AnalyzerProcess.Kill();
            }
            catch (InvalidOperationException) { }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
