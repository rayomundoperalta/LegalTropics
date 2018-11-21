using MSAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalTropics
{
    public partial class FuncionariosAPF : Form
    {
        List<string> ListaFuncionarios;

        public FuncionariosAPF()
        {
            InitializeComponent();
            this.Resize += Puestos_Resize;
            ListaFuncionarios = Globals.Ribbons.Tropicalizador.APF.ListPuestos();
            ListaFuncionarios.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });

            ListaFuncionarios.Sort();

            List<Rangos> rangos = new List<Rangos>();
            rangos.Add(new Rangos("a", "A", "c", "C"));
            rangos.Add(new Rangos("d", "D", "f", "F"));
            rangos.Add(new Rangos("g", "G", "i", "I"));
            rangos.Add(new Rangos("j", "J", "l", "L"));
            rangos.Add(new Rangos("m", "M", "o", "O"));
            rangos.Add(new Rangos("p", "P", "r", "R"));
            rangos.Add(new Rangos("s", "S", "u", "U"));
            rangos.Add(new Rangos("v", "V", "x", "X"));
            rangos.Add(new Rangos("y", "Y", "z", "Z"));

            int j = 0;
            for (int i = 0; i < rangos.Count; i++)
            {
                treeViewFuncionarios.Nodes.Add(new TreeNode(rangos[i].IniMayuscula + " - " + rangos[i].FinMayuscula));
                /* O J O   CON EL ORDEN DE LOS OPERANDOS */
                while ((j < ListaFuncionarios.Count) && EnRango(rangos[i], ListaFuncionarios[j])) // Cuidado el orden de las condiciones es importante
                {
                    int pos = ListaFuncionarios[j].IndexOf('_');
                    string ID = ListaFuncionarios[j].Substring(pos + 1, ListaFuncionarios[j].Length - pos - 1);

                    treeViewFuncionarios.Nodes[i].Nodes.Add(new TreeNode(AccessUtility.GetNombreFuncionario(ID) + "_" + ID));
                    j++;
                }
            }
        }

        struct Rangos
        {
            public char IniMinuscula;
            public char IniMayuscula;
            public char FinMinuscula;
            public char FinMayuscula;

            public Rangos(string iniMin, string iniMay, string finMin, string finMay)
            {
                IniMinuscula = iniMin[0];
                IniMayuscula = iniMay[0];
                FinMinuscula = finMin[0];
                FinMayuscula = finMay[0];
            }
        }

        private bool EnRango(Rangos rangos, string cuerda)
        {
            char IniChar = cuerda[0];
            bool c1, c2, c3, c4, r1, r2;
            c1 = rangos.IniMinuscula <= IniChar;
            c2 = IniChar <= rangos.FinMinuscula;
            c3 = rangos.IniMayuscula <= IniChar;
            c4 = IniChar <= rangos.FinMayuscula;

            r1 = c1 && c2;
            r2 = c3 && c4;
            if (r1 || r2) return true;
            return false;
        }

        private void Puestos_Resize(object sender, EventArgs e)
        {
            treeViewFuncionarios.Size = new Size(this.Width - 42, this.Height - 71);
        }

        private void treeViewFuncionarios_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
        }
    }
}
