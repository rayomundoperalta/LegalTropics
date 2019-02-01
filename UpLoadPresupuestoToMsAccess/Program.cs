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
            string Entidad = string.Empty;

            Entidad = Regex.Replace(Regex.Replace(Path.GetFileName(filename), @"CALENDARIO de presupuesto ", string.Empty,
                RegexOptions.IgnoreCase, TimeSpan.FromSeconds(0.25)), " ", string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(0.25));

            Console.WriteLine(Entidad);
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
                              + " ( Entidad, PDF, Abogado ) "
                              + " VALUES "
                              + " ( @Entidad, @PDF, 'Luis' ) ";

                        using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                        {
                            cmd.Parameters.Add("@Entidad", OleDbType.VarChar, 80).Value = Entidad;
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
