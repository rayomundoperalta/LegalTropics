using System;
using System.Drawing;
using System.Windows.Forms;
using Arboles;
using APFInfo;
using MSAccess;

namespace LegalTropics
{
    public partial class NavegaciónAPF : Form
    {
        public NavegaciónAPF()
        {
            InitializeComponent();
            this.Resize += NavegaciónAPF_Resize;
            LlenaTreeAPF(treeViewAPF.Nodes, Globals.Ribbons.Tropicalizador.APF, 0);
        }

        private void NavegaciónAPF_Resize(object sender, EventArgs e)
        {
            treeViewAPF.Size = new Size(this.Width - 42, this.Height - 71);
        }

        public void LlenaTreeAPF(TreeNodeCollection APFtreeNodes, Node<Registro> APF, int i)
        {
            TreeNode newNode = new TreeNode(APF.Value.NombrePuesto + " - " + AccessUtility.GetNombreFuncionario(APF.Value.ID) + "_" + APF.Value.ID);
            APFtreeNodes.Add(newNode);
            if (APF.Sons == null || APF.Sons.Count == 0)
            {
            }
            else
            {
                int j = 0;
                foreach (Node<Registro> nodo in APF.Sons)
                {
                    LlenaTreeAPF(APFtreeNodes[i].Nodes, nodo, j);
                    j++;
                }
            }
        }

        private void treeViewAPF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
        }
    }
}
