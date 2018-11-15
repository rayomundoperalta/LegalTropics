using System.Data;
using System.IO;
using System.Data.OleDb;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using Globales;

namespace UpLoadImagesToMsAccess
{
    public static class UpLoadPhotos
    {
        static public void SubeFoto(string filename)
        {
            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
            DataTable DataTable { get; set; }
            string PhotoType = Path.GetExtension(filename);
            string ID = string.Empty;

            var myRegex = new Regex(@"[a-zA-Z]+[0-9]+");

            MatchCollection AllMatches = myRegex.Matches(Path.GetFileName(filename));
            if (AllMatches.Count > 0)
            {
                foreach (Match someMatch in AllMatches)
                {
                    ID = someMatch.Groups[0].Value;
                    break;
                }
                Console.WriteLine(ID + " " + filename + " " + PhotoType);
            }
            else
                Console.WriteLine("El nombre del archivo esta mal formado: " + filename);

            FileInfo fi = new FileInfo(filename);

            if (fi.Exists)
            {
                Console.WriteLine("Existe");
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
                        string sqlQuery = "select * from Fotos where ID = @ID";
                        DataRow[] fotos;
                        using (OleDbCommand cmd = new OleDbCommand { CommandText = sqlQuery, Connection = cn })
                        {
                            cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                            DataTable.Load(cmd.ExecuteReader());
                            fotos = DataTable.Select();
                        }
                        if ((fotos.Length == 0))
                        {
                            // Add file info into DB
                            string sql = "INSERT INTO Fotos "
                                  + " ( ID, Foto, PhotoType ) "
                                  + " VALUES "
                                  + " ( @ID, @FotoData, @tipoFoto ) ";

                            using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                            {
                                cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                                cmd.Parameters.Add("@FotoData", OleDbType.LongVarBinary, (int)fi.Length).Value = bData;
                                cmd.Parameters.Add("@tipoFoto", OleDbType.VarChar, 80).Value = PhotoType;
                                Console.WriteLine(ID + " " + PhotoType);
                                cmd.ExecuteReader();
                            }
                        }
                    }
                }
            }
        }
    } 

    class Program
    {
        static void Main(string[] args)
        {
            string[] PhotoFiles = Directory.GetFiles(Defines.FotoBasePath, "*.*");

            for (int i = 0; i < PhotoFiles.Length; i++)
            {
                UpLoadPhotos.SubeFoto(PhotoFiles[i]);
            }
            Console.ReadKey();
        }

            
    }
}
