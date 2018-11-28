using Arboles;
using System;
using System.Runtime.CompilerServices;

namespace APFInfo
{
    public class Parser
    {
        Func<string, int> Print;
        public Tokens RT;
        private string[] pila = new string[] { "Presidencia", "S", "SS", "DG", "Dir" };

        public Parser(Func<string, int> print)
        {
            Print = print;
            RT = new Tokens(Print);
        }

        public void Parsea(Node<Registro> nodo, int Nivel)
        {
            Nivel++;
            if (Nivel < pila.Length)
            {
#if Debug
                if (RT.CurrentToken != null)
                    Print("Nivel " + Nivel + " busco " + pila[Nivel] + " tengo " + RT.CurrentToken.ToString());
#endif
                NodeList<Registro> hijos = new NodeList<Registro>();
                while (RT.CheckToken(pila[Nivel]))
                {
#if Debug
                    Print("Insertamos -> " + RT.LastToken.ToString());
#endif
                    Node<Registro> NivelJerarquia = new Node<Registro>(RT.LastToken);
                    Parsea(NivelJerarquia, Nivel);
                    hijos.InsertaHijo(NivelJerarquia);
#if Debug
                    if (RT.CurrentToken != null) Print("CurrentToken -> " + RT.CurrentToken.ToString());
#endif
                }
                nodo.AddNodeList(hijos);
                return;
            }
            return;
        }

        public NodeList<Registro> GetNodeListOf(Node<Registro> nodo, string NombrePuesto)
        {
            if (nodo.Value.NombrePuesto.Equals(NombrePuesto))
            {
                return nodo.Sons;
            }
            else
            {
                if (nodo.Sons != null)
                {
                    foreach (Node<Registro> node in nodo.Sons)
                    {
                        NodeList<Registro> lista = GetNodeListOf(node, NombrePuesto);
                        if (lista != null) return lista;
                    }
                }
            }
            return null;
        }
    }
}
