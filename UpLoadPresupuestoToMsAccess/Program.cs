using System;
using Globales;
using System.Data.OleDb;
using System.IO;
using AccesoBaseDatos;

namespace UpLoadPresupuestoToMsAccess
{
    

    class Program
    {
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();

        static void limpiaDataBase(string DB)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);

            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                cn.Open();
                string sql = "Delete from " + DB;
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        static bool AcceptedFileType(string PDFFileName)
        {
            if (PDFFileName != string.Empty)
            {
                switch (Path.GetExtension(PDFFileName).ToLower())
                {
                    case ".pdf":
                        return true;
                    default:
                        return false;
                }
            }
            else
                return false;
        }

        static void Main(string[] args)
        {
            limpiaDataBase("PDFPresupuesto");
            Console.WriteLine("Base de datos PDFPresupuesto limpia");
            Console.WriteLine("Vamos a subir los archivos PDF de presupuesto");

            string[] PDFFiles = Directory.GetFiles(Defines.PDFBasePath, "*.*");
            Console.WriteLine("Numero de PDF " + PDFFiles.Length);

            for (int i = 0; i < PDFFiles.Length; i++)
            {
                Console.Write((i + 1) + " ");
                if (AcceptedFileType(PDFFiles[i]))
                {
                    Console.Write("- ");
                    Datos.Instance.SubePDF(PDFFiles[i], "Luis");
                }
            }
            Console.WriteLine("\n\n                       F i n");
            Console.ReadKey();
        }
    }
}
