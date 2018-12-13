using Microsoft.Office.Interop.Word;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Globales;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MSAccess
{
    public static class AccessUtility
    {
        static private OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
        static private System.Data.DataTable tablaDeDatos { get; set; }
        
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

        static public DataRow[] GetDatosDeContacto(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM DatosContacto where ID = @ID;";
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

        static public DataRow[] GetCirculoCercano(string ID)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            tablaDeDatos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM CirculoCercano where ID = @ID;";
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
                var sql = "SELECT * FROM AdscripciónPolítica where ID = @ID order by Id1;";
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
                {
                    return Defines.DefaultPhoto();
                }
            }
            else
            {
                return Defines.DefaultPhoto();
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

        static public void UpdateFuncionario(string ID, string PrimerNombre, string SegundoNombre, string ApellidoPaterno, string ApellidoMaterno, string Nacionalidad, string FechaDeNacimiento)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "UPDATE Funcionarios set ApellidoPaterno = @ApellidoPaterno, ApellidoMaterno = @ApellidoMaterno, PrimerNombre = @PrimerNombre, SegundoNombre = @SegundoNombre, Nacionalidad = @Nacionalidad, FechaDeNacimiento = @FechaDeNacimiento WHERE ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@ApellidoPaterno", OleDbType.VarChar, 80).Value = ApellidoPaterno;
                    cmd.Parameters.Add("@ApellidoMaterno", OleDbType.VarChar, 80).Value = ApellidoMaterno;
                    cmd.Parameters.Add("@PrimerNombre", OleDbType.VarChar, 80).Value = PrimerNombre;
                    cmd.Parameters.Add("@SegundoNombre", OleDbType.VarChar, 80).Value = SegundoNombre;
                    cmd.Parameters.Add("@Nacionalidad", OleDbType.VarChar, 80).Value = Nacionalidad;
                    cmd.Parameters.Add("@FechaDeNacimiento", OleDbType.DBDate, 80).Value = FechaDeNacimiento;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void InsertFuncionario(string ID, string PrimerNombre, string SegundoNombre, string ApellidoPaterno, string ApellidoMaterno, string Nacionalidad, string FechaDeNacimiento)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Funcionarios (ID, ApellidoPaterno, ApellidoMaterno, PrimerNombre, SegundoNombre, Nacionalidad, FechaDeNacimiento) VALUES (@ID, @ApellidoPaterno, @ApellidoMaterno, @PrimerNombre, @SegundoNombre, @Nacionalidad, @FechaDeNacimiento);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@ApellidoPaterno", OleDbType.VarChar, 80).Value = ApellidoPaterno;
                    cmd.Parameters.Add("@ApellidoMaterno", OleDbType.VarChar, 80).Value = ApellidoMaterno;
                    cmd.Parameters.Add("@PrimerNombre", OleDbType.VarChar, 80).Value = PrimerNombre;
                    cmd.Parameters.Add("@SegundoNombre", OleDbType.VarChar, 80).Value = SegundoNombre;
                    cmd.Parameters.Add("@Nacionalidad", OleDbType.VarChar, 80).Value = Nacionalidad;
                    cmd.Parameters.Add("@FechaDeNacimiento", OleDbType.DBDate, 80).Value = FechaDeNacimiento;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static private System.Data.DataTable DataTable { get; set; }

        static public void SubeFoto(string DBfilename, string DiskFileName)
        {
            string PhotoType = Path.GetExtension(DiskFileName);
            string ID = string.Empty;

            var myRegex = new Regex(@"[a-zA-Z]+[ ]*[0-9]+");

            MatchCollection AllMatches = myRegex.Matches(Path.GetFileName(DBfilename));
            if (AllMatches.Count > 0)
            {
                foreach (Match someMatch in AllMatches)
                {
                    ID = someMatch.Groups[0].Value.Replace(" ", string.Empty);
                    break;
                }
                MessageBox.Show(DiskFileName);
                FileInfo fi = new FileInfo(DiskFileName);

                if (fi.Exists)
                {
                    Builder.Provider = Defines.StringAccessProvider;
                    Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
                    byte[] bData = null;
                    DataTable = new System.Data.DataTable();

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
                            string sql;

                            using (OleDbCommand cmd = new OleDbCommand { CommandText = sqlQuery, Connection = cn })
                            {
                                cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                                DataTable.Load(cmd.ExecuteReader());
                                fotos = DataTable.Select();
                            }
                            // Si no existe la foto en la bd se inserta, sino se actualiza
                            if ((fotos.Length > 0))
                            {
                                sql = @"DELETE FROM Fotos where ID = '" + ID + @"'";

                                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                                {
                                    int AffectedRows = cmd.ExecuteNonQuery();
                                }
                            }
                            // Add file info into DB
                            sql = "INSERT INTO Fotos "
                                  + " ( ID, Foto, PhotoType ) "
                                  + " VALUES "
                                  + " ( @ID, @FotoData, @tipoFoto ) ";

                            using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                            {
                                cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                                cmd.Parameters.Add("@FotoData", OleDbType.LongVarBinary, (int)fi.Length).Value = bData;
                                cmd.Parameters.Add("@tipoFoto", OleDbType.VarChar, 80).Value = PhotoType;
                                int AffectedRows = cmd.ExecuteNonQuery();
                            }
                            cn.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("El archivo con la foto no existe");
                }
            }
            else
            {
                MessageBox.Show("El nombre del archivo esta mal formado: " + DBfilename);
            }
        }

        static public string OrganigramaMaxId1()
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTable = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT Id1 FROM OrganigramaFederal order by Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    tablaDeDatos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
            DataRow[] Rows = tablaDeDatos.Select();
            return Rows[Rows.Length - 1]["Id1"].ToString();
        }

        static public void DeleteOrganigrama(string MaxId1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM OrganigramaFederal where Id1 <= " + MaxId1 + ";";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
        }

        static public void InsertRegistroOrganigrama(string TipoRegistro, string NombrePuesto, string ID, int Sec, Func<string, int> Print)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO OrganigramaFederal (TipoRegistro, NombrePuesto, ID, Sec) VALUES ('" + TipoRegistro + "', '" + NombrePuesto + "', '" + ID + "', '" + Sec.ToString() + "');";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    Print(cmd.CommandText);
                    cn.Open();
                    if (cmd.ExecuteNonQuery() == 0) MessageBox.Show("Error al insertar datos");
                    cn.Close();
                }
            }
        }
    }
}
