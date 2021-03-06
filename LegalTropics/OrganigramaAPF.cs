﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OrganigramaAdmin;
using Peta;
using OrgBusqueda;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AccesoBaseDatos;

namespace LegalTropics
{
    public partial class OrganigramaAPF : Form
    {
        Organigrama organigrama = new Organigrama();
        IndiceBD RevisandoFuncionario = null;
        IndiceBD RevisandoPuesto = null;
        ListaNombreNodo ListaFuncionarios = null;
        bool BusquedaEnProceso = false;
        bool BusquedaPuestoEnProceso = false;
        Busqueda BusquedaActiva = null;
        Busqueda BusquedaPuestoActiva = null;
        TreeNode FuncionarioEncontrado = null;

        public OrganigramaAPF()
        {
            InitializeComponent();
            this.Resize += NavegaciónAPF_Resize;
            TreeNode NodeAuxiliar = null;
            organigrama.LlenaTreeAPF(treeViewAPF.Nodes, Globals.Ribbons.Tropicalizador.APF, 0, false, ref NodeAuxiliar);
            ListaFuncionarios = new ListaNombreNodo();
            organigrama.ListaDeNodosAPF(Globals.Ribbons.Tropicalizador.APF, ListaFuncionarios);
            RevisandoFuncionario = new IndiceBD(ListaFuncionarios.Count);
            RevisandoPuesto = new IndiceBD(ListaFuncionarios.Count);
            BusquedaEnProceso = false;
            BusquedaActiva = null;
            treeViewAPF.BeforeSelect += TreeViewAPF_BeforeSelect;
            textBoxOrgCadenaBusqueda.Click += TextBoxOrgCadenaBusqueda_Click;
            textBoxOrgBuscaID.Click += TextBoxOrgBuscaID_Click;
            buttonVerFicha.Enabled = false;
            buttonVerPresupuesto.Enabled = false;
            buttonVerAmbos.Enabled = false;
        }

        private void TextBoxOrgBuscaID_Click(object sender, EventArgs e)
        {
            textBoxOrgBuscaID.Text = string.Empty;
        }

        private void TextBoxOrgCadenaBusqueda_Click(object sender, EventArgs e)
        {
            textBoxOrgCadenaBusqueda.Text = string.Empty;
        }

        private void TreeViewAPF_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeViewAPF.SelectedNode != null) treeViewAPF.SelectedNode.BackColor = Color.White;
            if (FuncionarioEncontrado != null) FuncionarioEncontrado.BackColor = Color.White;
        }

        private void NavegaciónAPF_Resize(object sender, EventArgs e)
        {
            // treeViewAPF.Size = new Size(this.Width - 42, this.Height - 71);
        }

        private string ID;
        private long Id1Presupuesto;

        private void treeViewAPF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            int posBlanco = e.Node.Text.IndexOf(' ');
            ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            if (e.Node.Text.Substring(0, 1).Equals("#"))
            {
                //Ficha_Presupuesto VentanaDesición = new Ficha_Presupuesto();
                //VentanaDesición.ID = ID;
                Id1Presupuesto = Convert.ToInt64(e.Node.Text.Substring(1, posBlanco - 1));
                //VentanaDesición.Id1Presupuesto = Id1Presupuesto;
                //VentanaDesición.Show();
                buttonVerFicha.Enabled = true;
                buttonVerPresupuesto.Enabled = true;
                buttonVerAmbos.Enabled = true;
            }
            else
            {
                Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
            }
        }

        private void buttonOrgTraeID_Click(object sender, EventArgs e)
        {
            int i = 0;
            for (i = 0; i < ListaFuncionarios.Count; i++)
            {
                if (ListaFuncionarios[i].Data.ID.Equals(textBoxOrgBuscaID.Text))
                {
                    break;
                }
            }

            if (i < ListaFuncionarios.Count)
            {
                ListaFuncionarios[i].Data.NodoDelTreeView.BackColor = Color.Aquamarine;
                ListaFuncionarios[i].Data.NodoDelTreeView.Expand();
                ListaFuncionarios[i].Data.NodoDelTreeView.EnsureVisible();
                FuncionarioEncontrado = ListaFuncionarios[i].Data.NodoDelTreeView;
            }
            else
            {
                MessageBox.Show("No se encontró un registro con los datos señalados");
            }
        }

        private void buttonOrgBuscarFuncionario_Click(object sender, EventArgs e)
        {
            int j = 0;
            if (FuncionarioEncontrado != null) FuncionarioEncontrado.BackColor = Color.White;
            if (BusquedaEnProceso)
            {
                j = RevisandoFuncionario.Pos + 1;
            }
            else // nueva busqueda
            {
                BusquedaActiva = new Busqueda();
                if (textBoxOrgCadenaBusqueda.Text.Equals(string.Empty))
                {
                    MessageBox.Show("Es necesario especificar lo que se va a buscar");
                }
                else
                {
                    BusquedaActiva.AddLineConFaltasDeOrtografia(textBoxOrgCadenaBusqueda.Text);
                }
            }
            int i = 0;
            for (i = j; i < RevisandoFuncionario.Length; i++)
            {
                if (BusquedaActiva.SatisfaceCriterio(ListaFuncionarios.NC(i)))
                {
                    RevisandoFuncionario.Pos = i;
                    BusquedaEnProceso = true;
                    break;
                }
            }
            
            if (i < RevisandoFuncionario.Length)
            {
                ListaFuncionarios[RevisandoFuncionario.Pos].Data.NodoDelTreeView.BackColor = Color.Aquamarine;
                ListaFuncionarios[RevisandoFuncionario.Pos].Data.NodoDelTreeView.Expand();
                ListaFuncionarios[RevisandoFuncionario.Pos].Data.NodoDelTreeView.EnsureVisible();
                FuncionarioEncontrado = ListaFuncionarios[RevisandoFuncionario.Pos].Data.NodoDelTreeView;
            }
            else
            {
                BusquedaEnProceso = false;
                RevisandoFuncionario.Pos = 0;
                MessageBox.Show("No se encontró un registro con los datos señalados");
            }
        }

        private void buttonOrgBuscaPuesto_Click(object sender, EventArgs e)
        {
            int j = 0;
            if (FuncionarioEncontrado != null) FuncionarioEncontrado.BackColor = Color.White;
            if (BusquedaPuestoEnProceso)
            {
                j = RevisandoPuesto.Pos + 1;
            }
            else // nueva busqueda
            {
                BusquedaPuestoActiva = new Busqueda();
                if (textBoxOrgCadenaBusqueda.Text.Equals(string.Empty))
                {
                    MessageBox.Show("Es necesario especificar lo que se va a buscar");
                }
                else
                {
                    BusquedaPuestoActiva.AddLineConFaltasDeOrtografia(textBoxOrgCadenaBusqueda.Text);
                }
            }
            int i = 0;
            for (i = j; i < RevisandoPuesto.Length; i++)
            {
                if (BusquedaPuestoActiva.SatisfaceCriterio(ListaFuncionarios[i].Data.NombrePuesto))
                {
                    RevisandoPuesto.Pos = i;
                    BusquedaPuestoEnProceso = true;
                    break;
                }
            }

            if (i < RevisandoPuesto.Length)
            {
                ListaFuncionarios[RevisandoPuesto.Pos].Data.NodoDelTreeView.BackColor = Color.Aquamarine;
                ListaFuncionarios[RevisandoPuesto.Pos].Data.NodoDelTreeView.Expand();
                ListaFuncionarios[RevisandoPuesto.Pos].Data.NodoDelTreeView.EnsureVisible();
                FuncionarioEncontrado = ListaFuncionarios[RevisandoPuesto.Pos].Data.NodoDelTreeView;
            }
            else
            {
                BusquedaPuestoEnProceso = false;
                RevisandoPuesto.Pos = 0;
                MessageBox.Show("No se encontró un registro con los datos señalados");
            }
        }

        private void textBoxOrgCadenaBusqueda_TextChanged(object sender, EventArgs e)
        {
            if (FuncionarioEncontrado != null) FuncionarioEncontrado.BackColor = Color.White;
            BusquedaPuestoEnProceso = false;
            BusquedaPuestoActiva = null;
        }

        private void textBoxOrgBuscaID_TextChanged(object sender, EventArgs e)
        {
            if (FuncionarioEncontrado != null) FuncionarioEncontrado.BackColor = Color.White;
        }

        private void buttonVerFicha_Click(object sender, EventArgs e)
        {
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
            buttonVerFicha.Enabled = false;
            buttonVerPresupuesto.Enabled = false;
            buttonVerAmbos.Enabled = false;
        }

        private void buttonVerPresupuesto_Click(object sender, EventArgs e)
        {
            Process.Start(Datos.Instance.GetFilePDFPresupuesto(Id1Presupuesto));
            buttonVerFicha.Enabled = false;
            buttonVerPresupuesto.Enabled = false;
            buttonVerAmbos.Enabled = false;
        }

        private void buttonVerAmbos_Click(object sender, EventArgs e)
        {
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
            Process.Start(Datos.Instance.GetFilePDFPresupuesto(Id1Presupuesto));
            buttonVerFicha.Enabled = false;
            buttonVerPresupuesto.Enabled = false;
            buttonVerAmbos.Enabled = false;
        }
    }
}
