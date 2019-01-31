using System;
using System.Collections.Generic;
using APFInfo;
using Peta;

namespace Pruebaarbol
{
    class Program
    {
        static int ImprimeConsola(string line)
        {
            Console.WriteLine(line);
            return 0;
        }

        static Dictionary<string, Node<Registro>> ListaDeNodosPorID = new Dictionary<string, Node<Registro>>();

        static void Main(string[] args)
        {
            Registro Presidente = new Registro("P", "Presidencia", "A0", "El Sistema");
            Node<Registro> APF = new Node<Registro>(Presidente);
            Console.WriteLine("Parseamos");
            Parser parser = new Parser(ImprimeConsola);
            parser.Parsea(APF, 0, ListaDeNodosPorID);
            Console.WriteLine("Imprimimos arbol");
            APF.Print();
            Console.WriteLine("\nbuscamos ");
            NodeList<Registro> lista = parser.GetNodeListOf(APF, "Presidencia");
            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine(lista[i].Data.ToString());
            }
            Console.WriteLine("Fin");
            Console.ReadKey();
        }
    }
}
