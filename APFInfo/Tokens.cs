using System.Data;
using System.Data.OleDb;
using System.IO;
using Globales;

namespace APFInfo
{
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
                foreach (DataRow row in Posiciones)
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

        public Registro LastToken { get { return lastToken; } }

        public int Cursor { get { return cursor; } }
    }
}
