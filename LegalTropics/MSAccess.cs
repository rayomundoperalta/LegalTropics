using Microsoft.Office.Interop.Word;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Globales;

namespace LegalTropics
{
    public static class MSAccess
    {
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
        static private System.Data.DataTable tablaDeDatos { get; set; }
        
        static public void ActualizaDataBase(string NewDataBaseFileName)
        {
            // leemos el nuevo archivo con la base de datos
            FileInfo fi = new FileInfo(NewDataBaseFileName);
            byte[] bData = null;
            if (fi.Exists)
            {
                
                // Read file data into buffer
                using (FileStream fs = fi.OpenRead())
                {
                    bData = new byte[fi.Length];
                    int nReadLength = fs.Read(bData, 0, (int)(fi.Length));
                }
            }
            // Vamos a escribir el nuevo archivo en el lugar del anterior
            // No vamos a generar copias del archivo
            FileInfo fo;
            string FullName = Defines.DataBasePath + Defines.DataBaseFileName;
            fo = new FileInfo(FullName);

            //Create the file.
            using (FileStream fs = fi.Create())
            {
                fs.Write(bData, 0, (int)fi.Length);
            }
        }

        static public DataRow[] GetFuncionarios()
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Funcionarios order by ApellidoPaterno;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetFuncionario(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Funcionarios where ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetEscolaridad(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Escolaridad where ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetAdscripcionPolitica(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM AdscripciónPolítica where ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetPuestos(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Puestos where ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetNotasRelevantes(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM InformaciónGeneral where ID = @ID and TipoDeInformación = @tipo;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@tipo", OleDbType.VarChar, 80).Value = "Información Relevante";
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetComentarios(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM InformaciónGeneral where ID = @ID and TipoDeInformación = @tipo;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@tipo", OleDbType.VarChar, 80).Value = "Comentario";
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetDistinctPuestos()
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT distinct(Puesto) FROM Puestos;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetDistinctDependencias()
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT distinct(DependenciaEntidad) FROM Puestos;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetIDPuestos(string Puesto)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT ID FROM Puestos where Puesto = @Puesto;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Puesto", OleDbType.VarChar, 80).Value = Puesto;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetIDPuestoAPF(string Puesto)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT ID FROM OrganigramaFederal where NombrePuesto = @Puesto;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Puesto", OleDbType.VarChar, 80).Value = Puesto;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public DataRow[] GetIDDependencias(string Dependencia)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT ID FROM Puestos where DependenciaEntidad = @Dependencia;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Dependencia", OleDbType.VarChar, 80).Value = Dependencia;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            return (tablaDeDatos.Select());
        }

        static public string GetFoto(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Fotos where ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            
            DataRow[] foto = tablaDeDatos.Select();
            
            if (foto.Length > 0)
            {
                int tamaño = ((byte[])foto[0]["Foto"]).Length;
                if (tamaño > 0)
                {
                    string FullName;
                    Guid guid;
                    FileInfo fi;
                    do
                    {
                        guid = Guid.NewGuid();
                        string UniqueFileName = guid.ToString();
                        FullName = Defines.FotoTempBasePath + UniqueFileName + foto[0]["PhotoType"].ToString();
                        fi = new FileInfo(FullName);
                    } while (fi.Exists);
                    
                    //Create the file.
                    using (FileStream fs = fi.Create())
                    {
                        fs.Write((byte[])foto[0]["Foto"], 0, tamaño);
                    }
                    return FullName;
                }
                else
                    return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        static public string[] GetFotoIDs()
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT ID FROM Fotos;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            DataRow[] IDRows = tablaDeDatos.Select();
            string[] IDs = new string[IDRows.Length];
            for (int j = 0; j < IDRows.Length; j++)
            {
                IDs[j] = IDRows[j]["ID"].ToString();
            }
            return IDs;
        }
    }
}
