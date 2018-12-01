using System;
using System.Collections.Generic;


namespace Arboles
{
    public class Node<T>
    {
        //Private member-variables
        private T data;
        private NodeList<T> sons = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> sons)
        {
            this.data = data;
            this.sons = sons;
        }

        public NodeList<T> Sons
        {
            get { return sons; }
            set { sons = value; }
        }

        public T Value
        {
            get { return data; }
            set { data = value; }
        }

        public void AddNodeList(NodeList<T> Sons)
        {
            this.sons = Sons;
        }

        public void Print()
        {
            if (sons == null || sons.Count == 0)
            {
                Console.Write("(" + data.ToString());
            }
            else
            {
                Console.Write("(" + data.ToString() + ",");
                sons.Print();
            }
            Console.Write(")");
        }

        public List<T> ListPuestos()
        {
            List<T> Resultado = new List<T>();
            Resultado.Add(data);
            if (sons == null || sons.Count == 0)
            {
                return Resultado;
            }
            else
            {
                Resultado.AddRange(sons.ListPuestosDeHijos());
                return Resultado;
            }
        }
    }
}
