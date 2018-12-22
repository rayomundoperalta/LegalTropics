using System.Windows.Forms;

namespace APFInfo
{
    public class Registro
    {
        string tipoRegistro;
        string nombrePuesto;
        string id;
        string abogadoIrresponsable;
        TreeNode nodoDelTreeView;

        //Node<Registro> AutoApuntador;

        public Registro(string TipoRegistro, string NombrePuesto, string ID, string AbogadoIrresponsable)
        {
            this.tipoRegistro = TipoRegistro;
            this.nombrePuesto = NombrePuesto;
            this.id = ID;
            this.abogadoIrresponsable = AbogadoIrresponsable;
        }

        public override string ToString()
        {
            return nombrePuesto + "_" + id;
        }

        public string TipoRegistro { get { return tipoRegistro; } set { tipoRegistro = value; } }
        public string NombrePuesto { get { return nombrePuesto; } }
        public string ID { get { return id; } }
        public string AbogadoIrresponsable { get { return abogadoIrresponsable; } }
        public TreeNode NodoDelTreeView { get { return nodoDelTreeView; } set { nodoDelTreeView = value; } }
        //public Node<Registro> AlterEGO { get { return AutoApuntador; } set { AutoApuntador = value; } }
    }
}
