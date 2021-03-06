﻿using AccesoBaseDatos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using APFInfo;
using System.Data;
using Peta;

namespace LegalTropics
{
    public partial class FuncionariosAPF : Form
    {
        List<Registro> ListaFuncionarios;

        public FuncionariosAPF()
        {
            InitializeComponent();
            this.Resize += Puestos_Resize;
            ListaFuncionarios = Globals.Ribbons.Tropicalizador.APF.ListPuestos();
            List<string> nombres = new List<string>();
            for (int i = 0; i < ListaFuncionarios.Count; i++)
            {
                DataRow[] fun = Datos.Instance.GetFuncionario(ListaFuncionarios[i].ID);
                // throw new System.IndexOutOfRangeException("No hay funcionario para el ID " + ListaFuncionarios[i].ID);
                if (fun.Length > 0)
                {
                    nombres.Add(fun[0]["ApellidoPaterno"].ToString().Substring(0, 1) + fun[0]["PrimerNombre"].ToString() + " " + fun[0]["SegundoNombre"].ToString() + " " + fun[0]["ApellidoPaterno"].ToString() + " " + fun[0]["ApellidoMaterno"].ToString() + "_" + ListaFuncionarios[i].ID);
                }                
            }
            nombres.Sort((x, y) => x[0].CompareTo(y[0]));
            
            nombres.Sort();

            //MessageBox.Show("# Nombres: " + nombres.Count);

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
                //MessageBox.Show(nombres[j]);
                while ((j < nombres.Count) && EnRango(rangos[i], U.SinA(nombres[j]).Replace(" ", string.Empty))) // Cuidado el orden de las condiciones es importante
                {
                    //MessageBox.Show(nombres[j]);
                    treeViewFuncionarios.Nodes[i].Nodes.Add(new TreeNode(nombres[j].Substring(1, nombres[j].Length - 1)));
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

        private bool EnRango(Rangos rangos, string nombre)
        {
            ;
            char IniChar = nombre[0];
            bool c1, c2, c3, c4, r1, r2;
            c1 = rangos.IniMinuscula <= IniChar;
            c2 = IniChar <= rangos.FinMinuscula;
            c3 = rangos.IniMayuscula <= IniChar;
            c4 = IniChar <= rangos.FinMayuscula;

            r1 = c1 && c2;
            r2 = c3 && c4;
            //MessageBox.Show("char = " + IniChar +
            //    "\nIniMinuscula = " + rangos.IniMinuscula + ", FinMinuscula = " + rangos.FinMinuscula +
            //    "\nIniMayuscula = " + rangos.IniMayuscula + ", FinMayuscula = " + rangos.FinMayuscula +
            //    "\nc1 = " + c1 + ", c2 = " + c2 + ", c3 = " + c3 + ", c4 = " + c4 +
            //    "\nEnRango: r1 = " + r1 + ", r2 = " + r2);
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
