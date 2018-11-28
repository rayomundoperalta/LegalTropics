using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Arboles
{
    public class NodeList<T> : Collection<Node<T>>
    {
        public NodeList() : base() { }

        public void InsertaHijo(Node<T> nodo)
        {
            base.Items.Add(nodo);
        }

        public Node<T> FindByValue(T value)
        {
            // Search the list for the value
            foreach (Node<T> node in Items)
                if (node.Value.Equals(value))
                    return node;
            // if we reached here, we didn't find a matching node
            return null;
        }

        public void Print()
        {
            foreach (Node<T> nodo in base.Items) nodo.Print();
        }

        public List<T> ListPuestosDeHijos()
        {
            List<T> Resultado = new List<T>();
            foreach (Node<T> nodo in base.Items) Resultado.AddRange(nodo.ListPuestos());
            return Resultado;
        }
    }
}
