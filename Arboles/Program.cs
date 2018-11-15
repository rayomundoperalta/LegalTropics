using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Globales;

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

        public List<string> ListPuestosDeHijos()
        {
            List<string> Resultado = new List<string>();
            foreach (Node<T> nodo in base.Items) Resultado.AddRange(nodo.ListPuestos());
            return Resultado;
        }
    }

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

        public List<string> ListPuestos()
        {
            List<string> Resultado = new List<string>();
            Resultado.Add(data.ToString());
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
            return nombrePuesto;
        }

        public string ToString(int caso)
        {
            switch (caso)
            {
                case 1:
                    return "{" + tipoRegistro + " " + nombrePuesto + " " + id + "}";
                    
                default:
                    return "{" + tipoRegistro + " " + nombrePuesto + " " + id + "}";
            }
        }

        public string TipoRegistro { get { return tipoRegistro; } }
        public string NombrePuesto { get { return nombrePuesto; } }
        public string ID { get { return id; } }
    }

    public class Tokens
    {
        private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
        private DataTable DataTable { get; set; }

        private int cursor = 0;
        private Registro currentToken = null;
        private Registro lastToken = null;
        private Registro[] Registros = null;

        public Tokens()
        {
            DataRow[] Posiciones = null;
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTable = new DataTable();

            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM OrganigramaFederal order by sec;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    DataTable.Load(cmd.ExecuteReader());
                    cn.Close();
                }
                Posiciones = DataTable.Select();
                Registros = new Registro[Posiciones.Length];
                int i = 0;
                foreach(DataRow row in Posiciones)
                {
                    Registros[i] = new Registro(row["TipoRegistro"].ToString(), row["NombrePuesto"].ToString(), row["ID"].ToString());
#if Debug
                    Console.WriteLine(Registros[i].ToString());
#endif
                    i++;
                }
                cursor = 0;
                lastToken = null;
                currentToken = Registros[cursor];
            }
        }

        private bool AdvanceToken()
        {
#if Debug
            Console.WriteLine("Cursor " + cursor + "Registros.Length " + Registros.Length);
#endif
            if (++cursor < Registros.Length)
            {
                this.lastToken = currentToken;
                currentToken = Registros[cursor];
                return true;
            }
            else
            {
                this.lastToken = currentToken;
                currentToken = null;
                return true;
            }
        }

        public bool CheckToken(string TipoRegistro)
        {
            // SOLO AVANZA EL PARSER SI ENCONTRÓ EL TOKEN
#if Debug
            if (CurrentToken != null) Console.WriteLine("CT-> " + CurrentToken.TipoRegistro + " =? " + TipoRegistro);
#endif
            if (CurrentToken != null && CurrentToken.TipoRegistro.Equals(TipoRegistro))
            {
                return AdvanceToken();
            }
            return false;
        }

        public Registro CurrentToken { get { return currentToken; } }

        public Registro LastToken {  get { return lastToken; } }

        public int Cursor {  get { return cursor; } }
    }
    
    static public class Parser
    {
        public static Tokens RT = new Tokens();
        static private string[] pila = new string[] { "Presidencia", "S", "SS", "DG", "Dir" };

        static public void Parsea(Node<Registro> nodo, int Nivel)
        {
            Nivel++;
            if (Nivel < pila.Length)
            {
#if Debug
                Console.WriteLine("Nivel " + Nivel + " busco " + pila[Nivel] + " tengo " + RT.CurrentToken.ToString());
#endif
                NodeList<Registro> hijos = new NodeList<Registro>();
                while (RT.CheckToken(pila[Nivel]))
                {
#if Debug
                    Console.WriteLine("Insertamos -> " + RT.LastToken.ToString());
#endif
                    Node<Registro> NivelJerarquia = new Node<Registro>(RT.LastToken);
                    Parsea(NivelJerarquia, Nivel);
                    hijos.InsertaHijo(NivelJerarquia);
#if Debug
                    if (RT.CurrentToken != null) Console.WriteLine("CurrentToken -> " + RT.CurrentToken.ToString());
#endif
                }
                nodo.AddNodeList(hijos);
                return;
            }
            return;
        }

        static public NodeList<Registro> GetNodeListOf(Node<Registro> nodo, string NombrePuesto)
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
