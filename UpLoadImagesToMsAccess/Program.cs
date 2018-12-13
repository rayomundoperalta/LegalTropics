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
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
        static private DataTable DataTable { get; set; }

        static public void SubeFoto(string filename)
        {
            string PhotoType = Path.GetExtension(filename);
            string ID = string.Empty;

            var myRegex = new Regex(@"[a-zA-Z]+[ ]*[0-9]+");

            MatchCollection AllMatches = myRegex.Matches(Path.GetFileName(filename));
            if (AllMatches.Count > 0)
            {
                foreach (Match someMatch in AllMatches)
                {
                    ID = someMatch.Groups[0].Value.Replace(" ", string.Empty);
                    break;
                }
                Console.WriteLine(ID + " " + filename + " " + PhotoType);
            }
            else
                Console.WriteLine("El nombre del archivo esta mal formado: " + filename);

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
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();

        static void limpiaDataBase(string DB)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                cn.Open();
                string sql = "Delete from " + DB + " where ID = ID";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.ExecuteNonQuery();
                }
                cn.Close();
            }   
        }

        static bool AcceptedFileType(string PhotoFileName)
        {
            Console.Write("-" + PhotoFileName + "-");
            if (PhotoFileName != string.Empty)
            {
                switch (Path.GetExtension(PhotoFileName).ToLower())
                {
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                    case ".bmp":
                    case ".wmf":
                    case ".png":
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
            //limpiaDataBase("AdscripciónPolítica");
            //limpiaDataBase("Escolaridad");
            limpiaDataBase("Fotos");
            //limpiaDataBase("Funcionarios");
            //limpiaDataBase("InformaciónGeneral");
            //limpiaDataBase("OrganigramaFederal");
            //limpiaDataBase("Puestos");


            string[] PhotoFiles = Directory.GetFiles(Defines.FotoBasePath, "*.*");
            Console.WriteLine("Numero de Photos " + PhotoFiles.Length);

            for (int i = 0; i < PhotoFiles.Length; i++)
            {
                Console.Write((i + 1) + " ");
                if (AcceptedFileType(PhotoFiles[i]))
                {
                    Console.Write("- ");
                    UpLoadPhotos.SubeFoto(PhotoFiles[i]);
                }
            }
            Console.WriteLine("F i n");
            Console.ReadKey();
        }
    }
}
