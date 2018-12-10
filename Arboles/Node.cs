using System;
using System.Collections.Generic;


namespace Arboles
{
    public class Node<T>
    {
        //Private member-variables
        private T data;
        private NodeList<T> sons = null;
        private Node<T> padre = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> sons) : this(data, sons, null) { }
        public Node(T data, NodeList<T> sons, Node<T> padre)
        {
            this.data = data;
            this.sons = sons;
            this.padre = padre;
        }

        public NodeList<T> Sons
        {
            get { return sons; }
            set { sons = value; }
        }

        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        public Node<T> Padre
        {
            get { return padre; }
            set { padre = value; }
        }

        public void AddNodeList(NodeList<T> Sons)
        {
            this.sons = Sons;
        }

        public void RemoveHijo(Node<T> hijo)
        {
            sons.Remove(hijo);
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

        public string PrintS()
        {
            if (sons == null || sons.Count == 0)
            {
                return data.ToString();
            }
            return "-";
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
