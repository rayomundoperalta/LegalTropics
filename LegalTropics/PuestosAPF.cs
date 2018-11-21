﻿using Arboles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using APFInfo;
using System.Globalization;

namespace LegalTropics
{
    public partial class PuestosAPF : Form
    {
        List<string> ListaPuestos;

        public PuestosAPF()
        {
            InitializeComponent();
            this.Resize += Puestos_Resize;
            ListaPuestos = Globals.Ribbons.Tropicalizador.APF.ListPuestos();
            ListaPuestos.Sort(delegate (string x, string y)
            {
                return x.CompareTo(y);
            });

            ListaPuestos.Sort();

            List<Rangos> rangos = new List<Rangos>();
            rangos.Add(new Rangos("a", "A","c","C"));
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
                treeViewPuestos.Nodes.Add(new TreeNode(rangos[i].IniMayuscula + " - " + rangos[i].FinMayuscula));
                /* O J O   CON EL ORDEN DE LOS OPERANDOS */
                while ((j < ListaPuestos.Count) && EnRango(rangos[i], ListaPuestos[j])) // Cuidado el orden de las condiciones es importante
                {
                    treeViewPuestos.Nodes[i].Nodes.Add(new TreeNode(ListaPuestos[j]));
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
            treeViewPuestos.Size = new Size(this.Width - 42, this.Height - 71);
        }

        private void treeViewPuestos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
        }
    }
}
