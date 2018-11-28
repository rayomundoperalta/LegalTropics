using Microsoft.Office.Interop.Word;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Globales;
using System.Windows.Forms;

namespace MSAccess
{
    public static class AccessUtility
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

        static public string GetNombreFuncionario(string ID)
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
            DataRow[] funcionarios = tablaDeDatos.Select();
            String name = string.Empty;
            for (int i = 0; i < funcionarios.Length; i++)
            {
                name = funcionarios[i]["PrimerNombre"] + " " + funcionarios[i]["ApellidoPaterno"];
                return name;
            }
            return name;
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
                var sql = "SELECT * FROM InformaciónGeneral where ID = @ID;";
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

        static public void InsertRegistroEscolaridad(string ID, string FechaDeInicio, string FechaDeFin, string Universidad, string Grado)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Escolaridad (ID,FechaDeInicio,FechaDeFin,Universidad,Grado)  VALUES (@ID, @FechaDeInicio, @FechaDeFin, @Universidad, @Grado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@FechaDeInicio", OleDbType.Numeric, 80).Value = FechaDeInicio;
                    cmd.Parameters.Add("@FechaDeFin", OleDbType.Numeric, 80).Value = FechaDeFin;
                    cmd.Parameters.Add("@Universidad", OleDbType.VarChar, 80).Value = Universidad;
                    cmd.Parameters.Add("@Grado", OleDbType.VarChar, 80).Value = Grado;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void DeleteRegistroEscolaridad(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Escolaridad WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.Numeric, 80).Value = Id1;
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void InsertRegistroAP(string ID, string FechaDeInicio, string FechaDeFin, string NombreDelPartido)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO AdscripciónPolítica (ID,FechaDeInicio,FechaDeFin,NombreDelPartido)  VALUES (@ID, @FechaDeInicio, @FechaDeFin, @NombreDelPartido);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@FechaDeInicio", OleDbType.Numeric, 80).Value = FechaDeInicio;
                    cmd.Parameters.Add("@FechaDeFin", OleDbType.VarChar, 80).Value = FechaDeFin;
                    cmd.Parameters.Add("@NombreDelPartido", OleDbType.VarChar, 80).Value = NombreDelPartido;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void DeleteRegistroAP(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM AdscripciónPolítica WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void InsertRegistroINFO(string ID, string TipoDeInformación, string Referencia)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO InformaciónGeneral (ID,TipoDeInformación,Referencia)  VALUES (@ID, @TipoDeInformación, @Referencia);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@TipoDeInformación", OleDbType.VarChar, 80).Value = TipoDeInformación;
                    cmd.Parameters.Add("@Referencia", OleDbType.VarChar, 100000).Value = Referencia;
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void DeleteRegistroINFO(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM InformaciónGeneral WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void InsertRegistroPuestos(string ID, string FechaDeInicio, string FechaDeFin, string DependenciaEntidad, string Puesto, string JefeInmediantoSuperior)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Puestos (ID,FechaDeInicio,FechaDeFin,DependenciaEntidad,Puesto,JefeInmediantoSuperior,CurrículumVitae)  VALUES (@ID, @FechaDeInicio, @FechaDeFin, @DependenciaEntidad, @Puesto, @JefeInmediantoSuperior, @CurrículumVitae);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@FechaDeInicio", OleDbType.Numeric, 80).Value = FechaDeInicio;
                    cmd.Parameters.Add("@FechaDeFin", OleDbType.Numeric, 80).Value = FechaDeFin;
                    cmd.Parameters.Add("@DependenciaEntidad", OleDbType.VarChar, 80).Value = DependenciaEntidad;
                    cmd.Parameters.Add("@Puesto", OleDbType.VarChar, 80).Value = Puesto;
                    cmd.Parameters.Add("@JefeInmediantoSuperior", OleDbType.VarChar, 80).Value = JefeInmediantoSuperior;
                    cmd.Parameters.Add("@CurrículumVitae", OleDbType.VarChar, 80).Value = "No";
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void DeleteRegistroPuestos(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Puestos WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }
    }
}
