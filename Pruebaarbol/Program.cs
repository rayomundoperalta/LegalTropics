using System;
using System.Linq;
using APFInfo;
using Arboles;

namespace Pruebaarbol
{
    class Program
    {
        static void Main(string[] args)
        {
            Registro Presidente = new Registro("P", "Presidencia", "A0");
            Node<Registro> APF = new Node<Registro>(Presidente);
            Console.WriteLine("Parseamos");
            Parser.Parsea(APF, 0);
            Console.WriteLine("Imprimimos arbol");
            APF.Print();
            Console.WriteLine("\nbuscamos ");
            NodeList<Registro> lista = Parser.GetNodeListOf(APF, "Presidencia");
            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine(lista[i].Value.ToString());
            }
            Console.ReadKey();
        }
    }
}
