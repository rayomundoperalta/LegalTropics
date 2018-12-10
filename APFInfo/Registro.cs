using Arboles;

namespace APFInfo
{
    public class Registro
    {
        string tipoRegistro;
        string nombrePuesto;
        string id;
        //Node<Registro> AutoApuntador;

        public Registro(string TipoRegistro, string NombrePuesto, string ID)
        {
            this.tipoRegistro = TipoRegistro;
            this.nombrePuesto = NombrePuesto;
            this.id = ID;
        }

        public override string ToString()
        {
            return nombrePuesto + "_" + id;
        }

        public string TipoRegistro { get { return tipoRegistro; } set { tipoRegistro = value; } }
        public string NombrePuesto { get { return nombrePuesto; } }
        public string ID { get { return id; } }
        //public Node<Registro> AlterEGO { get { return AutoApuntador; } set { AutoApuntador = value; } }
    }
}
