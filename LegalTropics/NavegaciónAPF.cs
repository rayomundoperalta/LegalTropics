using System;
using System.Drawing;
using System.Windows.Forms;
using OrganigramaAdmin;

namespace LegalTropics
{
    public partial class NavegaciónAPF : Form
    {
        Organigrama organigrama = new Organigrama();
        public NavegaciónAPF()
        {
            InitializeComponent();
            this.Resize += NavegaciónAPF_Resize;
            TreeNode NodeAuxiliar = null;
            organigrama.LlenaTreeAPF(treeViewAPF.Nodes, Globals.Ribbons.Tropicalizador.APF, 0, false, ref NodeAuxiliar);
        }

        private void NavegaciónAPF_Resize(object sender, EventArgs e)
        {
            treeViewAPF.Size = new Size(this.Width - 42, this.Height - 71);
        }

        private void treeViewAPF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int pos = e.Node.Text.IndexOf('_');
            string ID = e.Node.Text.Substring(pos + 1, e.Node.Text.Length - pos - 1);
            Globals.Ribbons.Tropicalizador.GeneraReporte(ID);
        }
    }
}
