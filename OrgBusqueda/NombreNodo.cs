using Arboles;
using APFInfo;

namespace OrgBusqueda
{
    public class NombreNodo
    {
        public Node<Registro> nodo;
        public string NombreCompleto;

        public NombreNodo(Node<Registro> nodo, string NombreCompleto)
        {
            this.nodo = nodo;
            this.NombreCompleto = NombreCompleto;
        }
    }
}
