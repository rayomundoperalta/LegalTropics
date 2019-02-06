using System;
using Globales;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;

namespace UpLoadPresupuestoToMsAccess
{
    public static class UpLoadPDF
    {
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
        static private DataTable DataTable { get; set; }

        static public void SubePDF(string filename)
        {
            string PDFType = Path.GetExtension(filename);
            string PDFFileName = string.Empty;

            PDFFileName = Regex.Replace(Regex.Replace(Path.GetFileName(filename), @"CALENDARIO de presupuesto ", string.Empty,
                RegexOptions.IgnoreCase, TimeSpan.FromSeconds(0.25)), " ", "_", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(0.25));

            Console.WriteLine(PDFFileName);
            Console.ReadKey();
            
            FileInfo fi = new FileInfo(filename);

            if (fi.Exists)
            {
                Builder.Provider = Defines.StringAccessProvider;
                Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
                DataTable = new DataTable();
                byte[] bData = null;
                // Read file data into buffer
                using (FileStream fs = fi.OpenRead())
                {
                    bData = new byte[fi.Length];

                    int nReadLength = fs.Read(bData, 0, (int)(fi.Length));

                    using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
                    {
                        cn.Open();
                        // Add file info into DB
                        string sql = "INSERT INTO PDFPresupuesto "
                              + " ( PDFFileName, PDF, Abogado, Asignado ) "
                              + " VALUES "
                              + " ( @PDFFileName, @PDF, 'Luis', " +  false + " ) ";

                        using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                        {
                            cmd.Parameters.Add("@PDFFileName", OleDbType.VarChar, 4000).Value = PDFFileName;
                            cmd.Parameters.Add("@PDF", OleDbType.LongVarBinary, (int)fi.Length).Value = bData;
                            cmd.ExecuteReader();
                        }
                    }
                }
            }
        }
    }

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
                    UpLoadPDF.SubePDF(PDFFiles[i]);
                }
            }
            Console.WriteLine("\n\n                       F i n");
            Console.ReadKey();
        }
    }
}
