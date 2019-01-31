using System;
using System.Collections.Generic;
using Peta;

namespace APFInfo
{
    public class Parser
    {
        Func<string, int> Print;
        public Tokens RT;
        public string[] pila = new string[] { "Presidencia", "S", "SS", "DG", "Dir", "D1", "D2", "D3", "D4", "D5", "D6" };

        public Parser(Func<string, int> print)
        {
            Print = print;
            RT = new Tokens(Print);
        }

        public void InitTokens()
        {
            RT = new Tokens(Print);
        }

        public void Parsea(Node<Registro> nodo, int Nivel, Dictionary<string, Node<Registro>> ListaDeNodosPorID)
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
                    Node<Registro> NivelJerarquia = new Node<Registro>(RT.LastToken, null, nodo);
                    //Registro bufferToken = NivelJerarquia.Value;
                    ListaDeNodosPorID.Add(NivelJerarquia.Data.ID, NivelJerarquia);
#if Debug
                    Print("Diccionario: " + ListaDeNodosPorID[NivelJerarquia.Data.ID].Data.ToString() + " orig " + NivelJerarquia.Data.ToString());
#endif
                    //bufferToken.AlterEGO = NivelJerarquia;
                    //NivelJerarquia.Value = bufferToken; // cada nodo se autoapunta, no es ortodoxo, pero me va a ayudar a editar el organigrama
                    Parsea(NivelJerarquia, Nivel, ListaDeNodosPorID);
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
            if (nodo.Data.NombrePuesto.Equals(NombrePuesto))
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
