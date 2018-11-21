using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APFInfo
{
    public class Registro
    {
        string tipoRegistro;
        string nombrePuesto;
        string id;

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

        public string TipoRegistro { get { return tipoRegistro; } }
        public string NombrePuesto { get { return nombrePuesto; } }
        public string ID { get { return id; } }

    }
}
