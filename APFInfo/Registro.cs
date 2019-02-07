using System.Windows.Forms;

namespace APFInfo
{
    public class Registro
    {
        string tipoRegistro;
        string nombrePuesto;
        string id;
        long sec;
        string abogadoIrresponsable;
        long id1Presupuesto;
        TreeNode nodoDelTreeView;

        //Node<Registro> AutoApuntador;

        public Registro(string TipoRegistro, string NombrePuesto, string ID, long Sec, string AbogadoIrresponsable, long Id1Presupuesto)
        {
            this.tipoRegistro = TipoRegistro;
            this.nombrePuesto = NombrePuesto;
            this.id = ID;
            if (Sec != -1)
            {
                this.sec = Sec;
            }
            this.abogadoIrresponsable = AbogadoIrresponsable;
            this.id1Presupuesto = Id1Presupuesto;
        }

        public override string ToString()
        {
            if (id1Presupuesto > 0)
            {
                return "# " + nombrePuesto + "_" + id;
            }
            return nombrePuesto + "_" + id;
        }

        public string TipoRegistro { get { return tipoRegistro; } set { tipoRegistro = value; } }
        public string NombrePuesto { get { return nombrePuesto; } }
        public string ID { get { return id; } }
        public long Sec { get { return sec; } }
        public string AbogadoIrresponsable { get { return abogadoIrresponsable; } }
        public long Id1Presupuesto { get { return id1Presupuesto; } set { id1Presupuesto = value; } }
        public TreeNode NodoDelTreeView { get { return nodoDelTreeView; } set { nodoDelTreeView = value; } }
        //public Node<Registro> AlterEGO { get { return AutoApuntador; } set { AutoApuntador = value; } }
    }
}
