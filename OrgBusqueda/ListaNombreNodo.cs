using System.Collections.Generic;
using Arboles;
using APFInfo;
using MSAccess;

namespace OrgBusqueda
{
    public class ListaNombreNodo
    {
        List<NombreNodo> LaLista = new List<NombreNodo>();
        public void Add(Node<Registro> nodo)
        {
            string NombreCompleto = AccessUtility.GetNombreFuncionario(nodo.Data.ID);
            LaLista.Add(new NombreNodo(nodo, NombreCompleto));
        }

        public Node<Registro> this[int index]
        {
            get { return LaLista[index].nodo; }

        }

        public string NC(int index)
        {
            return LaLista[index].NombreCompleto;
        }

        public int Count { get { return LaLista.Count; } }
    }
}
