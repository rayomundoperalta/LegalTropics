using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arboles;

namespace LegalTropics
{
    public partial class NavegaciónAPF : Form
    {
        public NavegaciónAPF()
        {
            InitializeComponent();
            LlenaTreeAPF(treeViewAPF.Nodes, Globals.Ribbons.Tropicalizador.APF, 0);
        }

        public void LlenaTreeAPF(TreeNodeCollection APFtreeNodes, Node<Registro> APF, int i)
        {
            TreeNode newNode = new TreeNode(APF.Value.ToString());
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
            DataRow[] IDs = MSAccess.GetIDPuestoAPF(e.Node.Text);
            for (int i = 0; i < IDs.Length; i++)
            {
                Globals.Ribbons.Tropicalizador.GeneraReporte(IDs[i]["ID"].ToString());
            }
        }


    }
}
