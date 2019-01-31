using Globales;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Peta;
using System;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Linq;

/*
bool	System.Boolean
byte	System.Byte
sbyte	System.SByte
char	System.Char
decimal	System.Decimal
double	System.Double
float	System.Single
int	System.Int32
uint	System.UInt32
long	System.Int64
ulong	System.UInt64
object	System.Object
short	System.Int16
ushort	System.UInt16
string	System.String
*/

namespace AccesoBaseDatos
{
    public sealed class Datos
    {
        static private Datos instance = null;
        static private readonly object padlock = new object();
        static private OleDbConnectionStringBuilder Builder;
        // static private System.Data.DataTable tablaDeDatos { get; set; }

        System.Data.DataTable DataTableFuncionarios;
        System.Data.DataTable DataTableDatosContacto;
        System.Data.DataTable DataTableCirculoCercano;
        System.Data.DataTable DataTableEscolaridad;
        System.Data.DataTable DataTableAdscripciónPolítica;
        System.Data.DataTable DataTablePuestos;
        System.Data.DataTable DataTableInformaciónGeneral;
        System.Data.DataTable DataTableOrganigramaFederal;
        System.Data.DataTable DataTableFotos;
        System.Data.DataTable DataTableAbogados;

        bool boolFuncionarios = false;
        bool boolDatosContacto = false;
        bool boolCirculoCercano = false;
        bool boolEscolaridad = false;
        bool boolAdscripciónPolítica = false;
        bool boolPuestos = false;
        bool boolInformaciónGeneral = false;
        bool boolOrganigramaFederal = false;
        bool boolFotos = false;
        //bool boolAbogados = false;

        Datos()
        {
            Console.WriteLine("Inicializacion de Datos");
            Builder = new OleDbConnectionStringBuilder();
            // 1 Funcionarios
            // 2 Datos Contacto
            // 3 CirculoCercano
            // 4 Escolaridad
            // 5 AdscripciónPolítica
            // 6 Puestos
            // 7 InformaciónGeneral
            // 8 OrganigramaFederal
            // 9 Fotos
            // 10 Abogados  -- Esta tabla se carga cada vez que se usa, ya que puede cambiar.
            
        }

        static public Datos Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Datos();
                    }
                    return instance;
                }
            }
        }

        private void UpLoadFuncionarios()
        {
            //var sql = "SELECT * FROM Funcionarios order by ApellidoPaterno;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableFuncionarios = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Funcionarios;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cn.Open();
                    DataTableFuncionarios.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadDatosContacto()
        {
            //var sql = "SELECT * FROM DatosContacto where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableDatosContacto = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM DatosContacto;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableDatosContacto.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadCirculoCercano()
        {
            //var sql = "SELECT * FROM CirculoCercano where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableCirculoCercano = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM CirculoCercano;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableCirculoCercano.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadEscolaridad()
        {
            //var sql = "SELECT * FROM Escolaridad where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableEscolaridad = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Escolaridad;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableEscolaridad.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadAdscripciónPolítica()
        {
            //var sql = "SELECT * FROM AdscripciónPolítica where ID = @ID order by Id1;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableAdscripciónPolítica = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM AdscripciónPolítica;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableAdscripciónPolítica.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadPuestos()
        {
            //var sql = "SELECT * FROM Puestos where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTablePuestos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Puestos;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTablePuestos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadInformaciónGeneral()
        {
            //var sql = "SELECT * FROM InformaciónGeneral where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableInformaciónGeneral = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM InformaciónGeneral;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableInformaciónGeneral.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadOrganigramaFederal()
        {
            //var sql = "SELECT ID FROM OrganigramaFederal where NombrePuesto = @Puesto;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableOrganigramaFederal = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection{ConnectionString = Builder.ConnectionString})
            {
                var sql = "SELECT ID FROM OrganigramaFederal;";
                using (OleDbCommand cmd = new OleDbCommand{CommandText = sql,Connection = cn})
                {
                    //cmd.Parameters.Add("@Puesto", OleDbType.VarChar, 80).Value = Puesto;
                    cn.Open();
                    DataTableOrganigramaFederal.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        private void UpLoadFotos()
        {
            //var sql = "SELECT * FROM Fotos where ID = @ID;";
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableFotos = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT * FROM Fotos;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    //cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    DataTableFotos.Load(cmd.ExecuteReader());
                    cn.Close();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        

        public DataRow[] GetFuncionarios()
        {
            if (!boolFuncionarios)
            {
                UpLoadFuncionarios();
                boolFuncionarios = true;
            }
            return DataTableFuncionarios.Select();
        }

        public DataRow[] GetFuncionario(string ID)
        {
            if (!boolFuncionarios)
            {
                UpLoadFuncionarios();
                boolFuncionarios = true;
            }
            var renglón = DataTableFuncionarios.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = new DataTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                renglones.Rows.InsertAt(ren, i++);
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetFuncionario(string ID)");
            }
            return renglones.Select();
        }

        public string GetNombreFuncionario(string ID)
        {
            if (!boolFuncionarios)
            {
                UpLoadFuncionarios();
                boolFuncionarios = true;
            }
            var funcionarios = DataTableFuncionarios.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            String name = string.Empty;
            int i = 0;
            foreach(DataRow renglón in funcionarios)
            {
                name = renglón.Field<string>("PrimerNombre") + " " + renglón.Field<string>("SegundoNombre") + " " + renglón.Field<string>("ApellidoPaterno") + " " + renglón.Field<string>("ApellidoMaterno");
                i++;
            }
            if (i >= 2) MessageBox.Show("Tenemos un error en GetNombreFuncionario(string ID) con linq");
            return name;
        }

        public DataRow[] GetDatosContacto(string ID)
        {
            if (!boolDatosContacto)
            {
                UpLoadDatosContacto();
                boolDatosContacto = true;
            }
            // return (DataTableDatosContacto.Select("ID = " + ID));
            var renglón = DataTableDatosContacto.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = new DataTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                renglones.Rows.InsertAt(ren, i++);
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetDatosContacto(string ID)");
            }
            return renglones.Select();
        }

        public DataRow[] GetCirculoCercano(string ID)
        {
            if (!boolCirculoCercano)
            {
                UpLoadCirculoCercano();
                boolCirculoCercano = true;
            }
            //return (DataTableCirculoCercano.Select("ID = " + ID));
            var renglón = DataTableCirculoCercano.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = new DataTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                renglones.Rows.InsertAt(ren, i++);
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetCirculoCercano(string ID)");
            }
            return renglones.Select();
        }

        private DataTable MakeEscolaridadTable()
        {
            DataTable Escolaridad = new DataTable("Escolaridad");

            DataColumn Id1 = new DataColumn();
            Id1.DataType = System.Type.GetType("System.Int64");
            Id1.ColumnName = "Id1";
            Id1.AutoIncrement = true;
            Escolaridad.Columns.Add(Id1);

            DataColumn ID = new DataColumn();
            ID.DataType = System.Type.GetType("System.String");
            ID.ColumnName = "ID";
            Escolaridad.Columns.Add(ID);

            DataColumn AñoInicial = new DataColumn();
            AñoInicial.DataType = System.Type.GetType("System.Int32");
            AñoInicial.ColumnName = "AñoInicial";
            Escolaridad.Columns.Add(AñoInicial);

            DataColumn MesInicial = new DataColumn();
            MesInicial.DataType = System.Type.GetType("System.Int32");
            MesInicial.ColumnName = "MesInicial";
            Escolaridad.Columns.Add(MesInicial);

            DataColumn DiaInicial = new DataColumn();
            DiaInicial.DataType = System.Type.GetType("System.Int32");
            DiaInicial.ColumnName = "DiaInicial";
            Escolaridad.Columns.Add(DiaInicial);

            DataColumn AñoFinal = new DataColumn();
            AñoFinal.DataType = System.Type.GetType("System.Int32");
            AñoFinal.ColumnName = "AñoFinal";
            Escolaridad.Columns.Add(AñoFinal);

            DataColumn MesFinal = new DataColumn();
            MesFinal.DataType = System.Type.GetType("System.Int32");
            MesFinal.ColumnName = "MesFinal";
            Escolaridad.Columns.Add(MesFinal);

            DataColumn DiaFinal = new DataColumn();
            DiaFinal.DataType = System.Type.GetType("System.Int32");
            DiaFinal.ColumnName = "DiaFinal";
            Escolaridad.Columns.Add(DiaFinal);

            DataColumn Universidad = new DataColumn();
            Universidad.DataType = System.Type.GetType("System.String");
            Universidad.ColumnName = "Universidad";
            Escolaridad.Columns.Add(Universidad);

            DataColumn Grado = new DataColumn();
            Grado.DataType = System.Type.GetType("System.String");
            Grado.ColumnName = "Grado";
            Escolaridad.Columns.Add(Grado);

            DataColumn Abogado = new DataColumn();
            Abogado.DataType = System.Type.GetType("System.String");
            Abogado.ColumnName = "Abogado";
            Escolaridad.Columns.Add(Abogado);

            return Escolaridad;
        }

        public DataRow[] GetEscolaridad(string ID)
        {
            if (!boolEscolaridad)
            {
                UpLoadEscolaridad();
                boolEscolaridad = true;
            }
            //return (DataTableEscolaridad.Select("ID = " + ID));
            var renglón = DataTableEscolaridad.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = MakeEscolaridadTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["AñoInicial"] = ren["AñoInicial"];
                row["MesInicial"] = ren["MesInicial"];
                row["DiaInicial"] = ren["DiaInicial"];
                row["AñoFInal"] = ren["AñoFInal"];
                row["MesFinal"] = ren["MesFinal"];
                row["DiaFinal"] = ren["DiaFinal"];
                row["Universidad"] = ren["Universidad"];
                row["Grado"] = ren["Grado"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetEscolaridad(string ID)");
            }
            return renglones.Select();
        }

        private DataTable MakeAdscripciónPolíticaTable()
        {
            DataTable AdscripciónPolítica = new DataTable("AdscripciónPolítica");

            DataColumn Id1 = new DataColumn();
            Id1.DataType = System.Type.GetType("System.Int64");
            Id1.ColumnName = "Id1";
            Id1.AutoIncrement = true;
            AdscripciónPolítica.Columns.Add(Id1);

            DataColumn ID = new DataColumn();
            ID.DataType = System.Type.GetType("System.String");
            ID.ColumnName = "ID";
            AdscripciónPolítica.Columns.Add(ID);

            DataColumn FechaDeInicio = new DataColumn();
            FechaDeInicio.DataType = System.Type.GetType("System.Int32");
            FechaDeInicio.ColumnName = "FechaDeInicio";
            AdscripciónPolítica.Columns.Add(FechaDeInicio);

            DataColumn FechaDeFin = new DataColumn();
            FechaDeFin.DataType = System.Type.GetType("System.Int32");
            FechaDeFin.ColumnName = "FechaDeFin";
            AdscripciónPolítica.Columns.Add(FechaDeFin);

            DataColumn NombreDelPartido = new DataColumn();
            NombreDelPartido.DataType = System.Type.GetType("System.String");
            NombreDelPartido.ColumnName = "NombreDelPartido";
            AdscripciónPolítica.Columns.Add(NombreDelPartido);

            DataColumn Abogado = new DataColumn();
            Abogado.DataType = System.Type.GetType("System.String");
            Abogado.ColumnName = "Abogado";
            AdscripciónPolítica.Columns.Add(Abogado);

            return AdscripciónPolítica;
        }

        public DataRow[] GetAdscripcionPolitica(string ID)
        {
            if (!boolAdscripciónPolítica)
            {
                UpLoadAdscripciónPolítica();
                boolAdscripciónPolítica = true;
            }
            //return (DataTableAdscripciónPolítica.Select("ID = " + ID )); // + " order by Id1"
            var renglón = DataTableAdscripciónPolítica.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID)).OrderBy(o => o);
            DataTable renglones = MakeAdscripciónPolíticaTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["FechaDeInicio"] = ren["FechaDeInicio"];
                row["FechaDeFin"] = ren["FechaDeFin"];
                row["NombreDelPartido"] = ren["NombreDelPartido"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetAdscripcionPolitica(string ID)");
            }
            return renglones.Select();
        }

        private DataTable MakePuestosTable()
        {
            DataTable Puestos = new DataTable("Puestos");

            DataColumn Id1 = new DataColumn();
            Id1.DataType = System.Type.GetType("System.Int64");
            Id1.ColumnName = "Id1";
            Id1.AutoIncrement = true;
            Puestos.Columns.Add(Id1);

            DataColumn ID = new DataColumn();
            ID.DataType = System.Type.GetType("System.String");
            ID.ColumnName = "ID";
            Puestos.Columns.Add(ID);

            DataColumn AñoInicial = new DataColumn();
            AñoInicial.DataType = System.Type.GetType("System.Int32");
            AñoInicial.ColumnName = "AñoInicial";
            Puestos.Columns.Add(AñoInicial);

            DataColumn MesInicial = new DataColumn();
            MesInicial.DataType = System.Type.GetType("System.Int32");
            MesInicial.ColumnName = "MesInicial";
            Puestos.Columns.Add(MesInicial);

            DataColumn DiaInicial = new DataColumn();
            DiaInicial.DataType = System.Type.GetType("System.Int32");
            DiaInicial.ColumnName = "DiaInicial";
            Puestos.Columns.Add(DiaInicial);

            DataColumn AñoFinal = new DataColumn();
            AñoFinal.DataType = System.Type.GetType("System.Int32");
            AñoFinal.ColumnName = "AñoFinal";
            Puestos.Columns.Add(AñoFinal);

            DataColumn MesFinal = new DataColumn();
            MesFinal.DataType = System.Type.GetType("System.Int32");
            MesFinal.ColumnName = "MesFinal";
            Puestos.Columns.Add(MesFinal);

            DataColumn DiaFinal = new DataColumn();
            DiaFinal.DataType = System.Type.GetType("System.Int32");
            DiaFinal.ColumnName = "DiaFinal";
            Puestos.Columns.Add(DiaFinal);

            DataColumn DependenciaEntidad = new DataColumn();
            DependenciaEntidad.DataType = System.Type.GetType("System.String");
            DependenciaEntidad.ColumnName = "DependenciaEntidad";
            Puestos.Columns.Add(DependenciaEntidad);

            DataColumn Puesto = new DataColumn();
            Puesto.DataType = System.Type.GetType("System.String");
            Puesto.ColumnName = "Puesto";
            Puestos.Columns.Add(Puesto);

            DataColumn JefeInmediatoSuperior = new DataColumn();
            JefeInmediatoSuperior.DataType = System.Type.GetType("System.String");
            JefeInmediatoSuperior.ColumnName = "JefeInmediatoSuperior";
            Puestos.Columns.Add(JefeInmediatoSuperior);

            DataColumn CurrículumVitae = new DataColumn();
            CurrículumVitae.DataType = System.Type.GetType("System.String");
            CurrículumVitae.ColumnName = "CurrículumVitae";
            Puestos.Columns.Add(CurrículumVitae);

            DataColumn CargoActual = new DataColumn();
            CargoActual.DataType = System.Type.GetType("System.String");
            CargoActual.ColumnName = "CargoActual";
            Puestos.Columns.Add(CargoActual);

            DataColumn Abogado = new DataColumn();
            Abogado.DataType = System.Type.GetType("System.String");
            Abogado.ColumnName = "Abogado";
            Puestos.Columns.Add(Abogado);

            return Puestos;
        }

        public DataRow[] GetPuestos(string ID)
        {
            if (!boolPuestos)
            {
                UpLoadPuestos();
                boolPuestos = true;
            }
            //return (DataTablePuestos.Select("ID = " + ID));
            var renglón = DataTablePuestos.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = MakePuestosTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["AñoInicial"] = ren["AñoInicial"];
                row["MesInicial"] = ren["MesInicial"];
                row["DiaInicial"] = ren["DiaInicial"];
                row["AñoFinal"] = ren["AñoFinal"];
                row["MesFinal"] = ren["MesFinal"];
                row["DiaFinal"] = ren["DiaFinal"];
                row["DependenciaEntidad"] = ren["DependenciaEntidad"];
                row["Puesto"] = ren["Puesto"];
                row["JefeInmediatoSuperior"] = ren["JefeInmediantoSuperior"];
                row["CurrículumVitae"] = ren["CurrículumVitae"];
                row["CargoActual"] = ren["CargoActual"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetPuestos(string ID)");
            }
            return renglones.Select();
        }

        private DataTable MakeInformaciónGeneralTable()
        {
            DataTable InformaciónGeneral = new DataTable("InformaciónGeneral");

            DataColumn Id1 = new DataColumn();
            Id1.DataType = System.Type.GetType("System.Int64");
            Id1.ColumnName = "Id1";
            Id1.AutoIncrement = true;
            InformaciónGeneral.Columns.Add(Id1);

            DataColumn ID = new DataColumn();
            ID.DataType = System.Type.GetType("System.String");
            ID.ColumnName = "ID";
            InformaciónGeneral.Columns.Add(ID);

            DataColumn TipoDeInformación = new DataColumn();
            TipoDeInformación.DataType = System.Type.GetType("System.String");
            TipoDeInformación.ColumnName = "TipoDeInformación";
            InformaciónGeneral.Columns.Add(TipoDeInformación);

            DataColumn Referencia = new DataColumn();
            Referencia.DataType = System.Type.GetType("System.String");
            Referencia.ColumnName = "Referencia";
            InformaciónGeneral.Columns.Add(Referencia);

            DataColumn Abogado = new DataColumn();
            Abogado.DataType = System.Type.GetType("System.String");
            Abogado.ColumnName = "Abogado";
            InformaciónGeneral.Columns.Add(Abogado);

            return InformaciónGeneral;
        }


        public DataRow[] GetNotasRelevantes(string ID)
        {
            if (!boolInformaciónGeneral)
            {
                UpLoadInformaciónGeneral();
                boolInformaciónGeneral = true;
            }
            //return (DataTableInformaciónGeneral.Select("ID = " + ID));
            var renglón = DataTableInformaciónGeneral.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = MakeInformaciónGeneralTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["TipoDeInformación"] = ren["TipoDeInformación"];
                row["Referencia"] = ren["Referencia"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetNotasRelevantes(string ID)");
            }
            return renglones.Select();
        }

        public DataRow[] GetComentarios(string ID)
        {
            if (!boolInformaciónGeneral)
            {
                UpLoadInformaciónGeneral();
                boolInformaciónGeneral = true;
            }
            //return (DataTableInformaciónGeneral.Select("ID = " + ID + " and TipoDeInformación = Comentario"));
            var renglón = DataTableInformaciónGeneral.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID) & x.Field<string>("TipoDeInformación").Equals("Comentario"));
            DataTable renglones = MakeInformaciónGeneralTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["TipoDeInformación"] = ren["TipoDeInformación"];
                row["Referencia"] = ren["Referencia"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Problema de Linq en GetComentarios(string ID)");
            }
            return renglones.Select();
        }

        public string[] GetDistinctPuestos()
        {
            if (!boolPuestos)
            {
                UpLoadPuestos();
                boolPuestos = true;
            }
            //return (DataTablePuestos.Select("distinct(Puesto)"));
            return (string[]) DataTablePuestos.AsEnumerable().Select(x => x.Field<string>("Puesto")).Distinct();
        }

        public string[] GetDistinctDependencias()
        {
            if (!boolPuestos)
            {
                UpLoadPuestos();
                boolPuestos = true;
            }
            //return (DataTablePuestos.Select("distinct(DependenciaEntidad)"));
            return (string[])DataTablePuestos.AsEnumerable().Select(x => x.Field<string>("DependenciaEntidad")).Distinct();
        }

        public DataRow[] GetIDPuestos(string Puesto)
        {
            //var sql = "SELECT ID FROM Puestos where Puesto = @Puesto;";
            if (!boolPuestos)
            {
                UpLoadPuestos();
                boolPuestos = true;
            }
            //return (DataTablePuestos.Select("Puesto = " + Puesto));
            var renglón = DataTablePuestos.AsEnumerable().Where(x => x.Field<string>("Puesto").Equals(Puesto));
            DataTable renglones = MakePuestosTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["AñoInicial"] = ren["AñoInicial"];
                row["MesInicial"] = ren["MesInicial"];
                row["DiaInicial"] = ren["DiaInicial"];
                row["AñoFinal"] = ren["AñoFinal"];
                row["MesFinal"] = ren["MesFinal"];
                row["DiaFinal"] = ren["DiaFinal"];
                row["DependenciaEntidad"] = ren["DependenciaEntidad"];
                row["Puesto"] = ren["Puesto"];
                row["JefeInmediatoSuperior"] = ren["JefeInmediatoSuperior"];
                row["CurrículumVitae"] = ren["CurrículumVitae"];
                row["CargoActual"] = ren["CargoActual"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            return renglones.Select();
        }

        private DataTable MakeOrganigramaFederalTable()
        {
            DataTable OrganigramaFederal = new DataTable("OrganigramaFederal");

            DataColumn Id1 = new DataColumn();
            Id1.DataType = System.Type.GetType("System.Int64");
            Id1.ColumnName = "Id1";
            Id1.AutoIncrement = true;
            OrganigramaFederal.Columns.Add(Id1);

            DataColumn TipoRegistro = new DataColumn();
            TipoRegistro.DataType = System.Type.GetType("System.String");
            TipoRegistro.ColumnName = "TipoRegistro";
            OrganigramaFederal.Columns.Add(TipoRegistro);

            DataColumn NombrePuesto = new DataColumn();
            NombrePuesto.DataType = System.Type.GetType("System.String");
            NombrePuesto.ColumnName = "NombrePuesto";
            OrganigramaFederal.Columns.Add(NombrePuesto);

            DataColumn ID = new DataColumn();
            ID.DataType = System.Type.GetType("System.String");
            ID.ColumnName = "ID";
            OrganigramaFederal.Columns.Add(ID);

            DataColumn Sec = new DataColumn();
            Sec.DataType = System.Type.GetType("System.String");
            Sec.ColumnName = "Sec";
            OrganigramaFederal.Columns.Add(Sec);

            DataColumn Abogado = new DataColumn();
            Abogado.DataType = System.Type.GetType("System.String");
            Abogado.ColumnName = "Abogado";
            OrganigramaFederal.Columns.Add(Abogado);

            return OrganigramaFederal;
        }

        public DataRow[] GetIDPuestoAPF(string NombrePuesto)
        {
            //var sql = "SELECT ID FROM OrganigramaFederal where NombrePuesto = @Puesto;";
            if (!boolOrganigramaFederal)
            {
                UpLoadOrganigramaFederal();
                boolOrganigramaFederal = true;
            }
            // return (DataTableOrganigramaFederal.Select(" NombrePuesto = " + NombrePuesto));
            var renglón = DataTableOrganigramaFederal.AsEnumerable().Where(x => x.Field<string>("NombrePuesto").Equals(NombrePuesto));
            DataTable renglones = MakeOrganigramaFederalTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["TipoRegistro"] = ren["TipoRegistro"];
                row["NombrePuesto"] = ren["NombrePuesto"];
                row["ID"] = ren["ID"];
                row["Sec"] = ren["Sec"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Linq error en GetIDPuestoAPF(string NombrePuesto)");
            }
            return renglones.Select();
        }

        public DataRow[] GetIDDependencias(string Dependencia)
        {
            //var sql = "SELECT ID FROM Puestos where ;";
            if (!boolPuestos)
            {
                UpLoadPuestos();
                boolPuestos = true;
            }
            //return (DataTablePuestos.Select("DependenciaEntidad = " + Dependencia));
            var renglón = DataTablePuestos.AsEnumerable().Where(x => x.Field<string>("DependenciaEntidad").Equals(Dependencia));
            DataTable renglones = MakePuestosTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                DataRow row = renglones.NewRow();
                row["Id1"] = ren["Id1"];
                row["ID"] = ren["ID"];
                row["AñoInicial"] = ren["AñoInicial"];
                row["MesInicial"] = ren["MesInicial"];
                row["DiaInicial"] = ren["DiaInicial"];
                row["AñoFinal"] = ren["AñoFinal"];
                row["MesFinal"] = ren["MesFinal"];
                row["DiaFinal"] = ren["DiaFinal"];
                row["DependenciaEntidad"] = ren["DependenciaEntidad"];
                row["Puesto"] = ren["Puesto"];
                row["JefeInmediatoSuperior"] = ren["JefeInmediatoSuperior"];
                row["CurrículumVitae"] = ren["CurrículumVitae"];
                row["CargoActual"] = ren["CargoActual"];
                row["Abogado"] = ren["Abogado"];
                renglones.Rows.Add(row);
                i++;
            }
            if (i > 1)
            {
                MessageBox.Show("Linq error en GetIDDependencias(string Dependencia)");
            }
            return renglones.Select();
        }

        public string GetFoto(string ID)
        {
            //var sql = "SELECT * FROM Fotos where ID = @ID;";
            if (!boolFotos)
            {
                UpLoadFotos();
                boolFotos = true;
            }
            var foto = DataTableFotos.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            foreach (DataRow renglón in foto)
            {
                int tamaño = (renglón.Field<byte[]>("Foto")).Length;
                if (tamaño > 0)
                {
                    string FullName;
                    Guid guid;
                    FileInfo fi;
                    do
                    {
                        guid = Guid.NewGuid();
                        string UniqueFileName = guid.ToString();
                        FullName = Defines.FotoTempBasePath + UniqueFileName + renglón.Field<string>("PhotoType");
                        fi = new FileInfo(FullName);
                    } while (fi.Exists);

                    //Create the file.
                    using (FileStream fs = fi.Create())
                    {
                        fs.Write(renglón.Field<byte[]>("Foto"), 0, tamaño);
                    }
                    return FullName;
                }
                else
                {
                    return Defines.DefaultPhoto();
                }
            }
            return Defines.DefaultPhoto();
        }

        public string[] GetFotoIDs()
        {
            if (!boolFotos)
            {
                UpLoadFotos();
                boolFotos = true;
            }
            DataRow[] IDRows = DataTableFotos.Select();
            string[] IDs = new string[IDRows.Length];
            for (int j = 0; j < IDRows.Length; j++)
            {
                IDs[j] = IDRows[j]["ID"].ToString();
            }
            return IDs;
        }

        public void InsertRegistroEscolaridad(string ID, string AñoInicial, string MesInicial, string DiaInicial, string AñoFinal, string MesFinal, string DiaFinal, string Universidad, string Grado, string AbogadoResp)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Escolaridad (ID, AñoInicial, MesInicial, DiaInicial, AñoFinal, MesFinal, DiaFinal,Universidad,Grado, Abogado)  VALUES " +
                    "(@ID, @AñoInicial, @MesInicial, @DiaInicial, @AñoFinal, @MesFinal, @DiaFinal, @Universidad, @Grado, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@AñoInicial", OleDbType.Numeric, 80).Value = AñoInicial;
                    cmd.Parameters.Add("@MesInicial", OleDbType.Numeric, 80).Value = MesInicial;
                    cmd.Parameters.Add("@DiaInicial", OleDbType.Numeric, 80).Value = DiaInicial;
                    cmd.Parameters.Add("@AñoFinal", OleDbType.Numeric, 80).Value = AñoFinal;
                    cmd.Parameters.Add("@MesFinal", OleDbType.Numeric, 80).Value = MesFinal;
                    cmd.Parameters.Add("@DiaFinal", OleDbType.Numeric, 80).Value = DiaFinal;
                    cmd.Parameters.Add("@Universidad", OleDbType.VarChar, 80).Value = Universidad;
                    cmd.Parameters.Add("@Grado", OleDbType.VarChar, 80).Value = Grado;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = AbogadoResp;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
                boolEscolaridad = false;
            }
        }

        public void DeleteRegistroEscolaridad(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Escolaridad WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolEscolaridad = false;
        }

        public void InsertRegistroAP(string ID, string FechaDeInicio, string FechaDeFin, string NombreDelPartido, string Abogado)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO AdscripciónPolítica (ID,FechaDeInicio,FechaDeFin,NombreDelPartido,Abogado)  VALUES (@ID, @FechaDeInicio, @FechaDeFin, @NombreDelPartido,@Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@FechaDeInicio", OleDbType.Numeric, 80).Value = FechaDeInicio;
                    cmd.Parameters.Add("@FechaDeFin", OleDbType.VarChar, 80).Value = FechaDeFin;
                    cmd.Parameters.Add("@NombreDelPartido", OleDbType.VarChar, 80).Value = NombreDelPartido;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = Abogado;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolAdscripciónPolítica = false;
        }

        public void DeleteRegistroAP(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM AdscripciónPolítica WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolAdscripciónPolítica = false;
        }

        public void InsertRegistroINFO(string ID, string TipoDeInformación, string Referencia, string AbogadoResp)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO InformaciónGeneral (ID,TipoDeInformación,Referencia,Abogado)  VALUES (@ID, @TipoDeInformación, @Referencia, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@TipoDeInformación", OleDbType.VarChar, 80).Value = TipoDeInformación;
                    cmd.Parameters.Add("@Referencia", OleDbType.VarChar, 100000).Value = Referencia;
                    cmd.Parameters.Add("@Abogados", OleDbType.VarChar, 100000).Value = AbogadoResp;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolInformaciónGeneral = false;
        }

        public void DeleteRegistroINFO(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM InformaciónGeneral WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolInformaciónGeneral = false;
        }

        public void InsertRegistroPuestos(string ID, string AñoInicio, string MesInicio, string DiaInicio, string AñoFin, string MesFin, string DiaFin, string DependenciaEntidad,
            string Puesto, string JefeInmediantoSuperior, string CargoActual, string Abogado)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Puestos (ID,AñoInicial,MesInicial,DiaInicial,AñoFinal,MesFinal,DiaFinal,DependenciaEntidad,Puesto,JefeInmediantoSuperior,CurrículumVitae,CargoActual,Abogado)  VALUES " +
                    "(@ID, @AñoInicio, @MesInicio, @DiaInicio, @AñoFin, @MesFin, @DiaFin, @DependenciaEntidad, @Puesto, @JefeInmediantoSuperior, @CurrículumVitae, @CargoActual, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@AñoInicio", OleDbType.Numeric, 80).Value = AñoInicio;
                    cmd.Parameters.Add("@MesInicio", OleDbType.Numeric, 80).Value = MesInicio;
                    cmd.Parameters.Add("@DiaInicio", OleDbType.Numeric, 80).Value = DiaInicio;
                    cmd.Parameters.Add("@AñoFin", OleDbType.Numeric, 80).Value = AñoFin;
                    cmd.Parameters.Add("@MesFin", OleDbType.Numeric, 80).Value = MesFin;
                    cmd.Parameters.Add("@DiaFin", OleDbType.Numeric, 80).Value = DiaFin;
                    cmd.Parameters.Add("@DependenciaEntidad", OleDbType.VarChar, 10000).Value = DependenciaEntidad;
                    cmd.Parameters.Add("@Puesto", OleDbType.VarChar, 80).Value = Puesto;
                    cmd.Parameters.Add("@JefeInmediantoSuperior", OleDbType.VarChar, 80).Value = JefeInmediantoSuperior;
                    cmd.Parameters.Add("@CurrículumVitae", OleDbType.VarChar, 80).Value = "No";
                    cmd.Parameters.Add("@CargoActual", OleDbType.VarChar, 80).Value = CargoActual;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = Abogado;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolPuestos = false;
        }

        public void DeleteRegistroPuestos(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Puestos WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolPuestos = false;
        }

        public void InsertRegistroDatosContacto(string ID, string Tipo, string dato, string AbogadoResp)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO DatosContacto (ID, Tipo, dato, Abogado)  VALUES (@ID, @Tipo, @Dato, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@Tipo", OleDbType.VarChar, 80).Value = Tipo;
                    cmd.Parameters.Add("@dato", OleDbType.VarChar, 80).Value = dato;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = AbogadoResp;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolDatosContacto = false;
        }

        public void DeleteRegistroDatosContacto(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM DatosContacto WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolDatosContacto = false;
        }

        public void InsertRegistroCirculoCercano(string ID, string Nombre, string Información, string AbogadoResp)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO CirculoCercano (ID, Nombre, Información, Abogado)  VALUES (@ID, @Nombre, @Información, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@Nombre", OleDbType.VarChar, 80).Value = Nombre;
                    cmd.Parameters.Add("@Información", OleDbType.VarChar, 80).Value = Información;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = AbogadoResp;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolCirculoCercano = false;
        }

        public void DeleteRegistroCirculoCercano(string Id1)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM CirculoCercano WHERE Id1 = @Id1;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Id1", OleDbType.Numeric, 80).Value = Id1;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolCirculoCercano = false;
        }

        public void UpdateFuncionario(string ID, string PrimerNombre, string SegundoNombre, string ApellidoPaterno, string ApellidoMaterno, string Nacionalidad, string AñoNacimiento, string MesNacimiento, string DiaNacimiento, string Abogado)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Funcionarios WHERE ID = @ID;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
                sql = "INSERT INTO Funcionarios (ID, ApellidoPaterno, ApellidoMaterno, PrimerNombre, SegundoNombre, Nacionalidad, AñoNacimiento, MesNacimiento, DiaNacimiento, Abogado) " +
                    "Values (@ID, @ApellidoPaterno, @ApellidoMaterno, @PrimerNombre, @SegundoNombre, @Nacionalidad, @AñoNacimiento, @MesNacimiento, @DiaNacimiento, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@ApellidoPaterno", OleDbType.VarChar, 80).Value = ApellidoPaterno;
                    cmd.Parameters.Add("@ApellidoMaterno", OleDbType.VarChar, 80).Value = ApellidoMaterno;
                    cmd.Parameters.Add("@PrimerNombre", OleDbType.VarChar, 80).Value = PrimerNombre;
                    cmd.Parameters.Add("@SegundoNombre", OleDbType.VarChar, 80).Value = SegundoNombre;
                    cmd.Parameters.Add("@Nacionalidad", OleDbType.VarChar, 80).Value = Nacionalidad;
                    cmd.Parameters.Add("@AñoNacimiento", OleDbType.Integer, 80).Value = AñoNacimiento;
                    cmd.Parameters.Add("@MesNacimiento", OleDbType.Integer, 80).Value = MesNacimiento;
                    cmd.Parameters.Add("@DiaNacimiento", OleDbType.Integer, 80).Value = DiaNacimiento;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = Abogado;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolFuncionarios = false;
        }

        public void InsertFuncionario(string ID, string PrimerNombre, string SegundoNombre, string ApellidoPaterno, string ApellidoMaterno, string Nacionalidad, string AñoNacimiento, string MesNacimiento, string DiaNacimiento, string AbogadoIrresponsable)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO Funcionarios (ID, ApellidoPaterno, ApellidoMaterno, PrimerNombre, SegundoNombre, Nacionalidad, AñoNacimiento, MesNacimiento, DiaNacimiento, Abogado) VALUES (@ID, @ApellidoPaterno, @ApellidoMaterno, @PrimerNombre, @SegundoNombre, @Nacionalidad, @AñoNacimiento, @MesNacimiento, @DiaNacimiento, @Abogado);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@ID", OleDbType.VarChar, 80).Value = ID;
                    cmd.Parameters.Add("@ApellidoPaterno", OleDbType.VarChar, 80).Value = ApellidoPaterno;
                    cmd.Parameters.Add("@ApellidoMaterno", OleDbType.VarChar, 80).Value = ApellidoMaterno;
                    cmd.Parameters.Add("@PrimerNombre", OleDbType.VarChar, 80).Value = PrimerNombre;
                    cmd.Parameters.Add("@SegundoNombre", OleDbType.VarChar, 80).Value = SegundoNombre;
                    cmd.Parameters.Add("@Nacionalidad", OleDbType.VarChar, 80).Value = Nacionalidad;
                    cmd.Parameters.Add("@AñoNocimiento", OleDbType.Integer, 80).Value = AñoNacimiento;
                    cmd.Parameters.Add("@MesNocimiento", OleDbType.Integer, 80).Value = MesNacimiento;
                    cmd.Parameters.Add("@DiaNocimiento", OleDbType.Integer, 80).Value = DiaNacimiento;
                    cmd.Parameters.Add("@Abogado", OleDbType.VarChar, 80).Value = AbogadoIrresponsable;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            boolFuncionarios = false;
        }

        private System.Data.DataTable DataTable { get; set; }

        public void SubeFoto(string DBfilename, string DiskFileName)
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
                            boolFotos = false;
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

        public string OrganigramaMaxId1()
        {
            // var sql = "SELECT Id1 FROM OrganigramaFederal order by Id1;";
            if (!boolOrganigramaFederal)
            {
                UpLoadOrganigramaFederal();
                boolOrganigramaFederal = true;
            }
            var Rows = DataTableOrganigramaFederal.AsEnumerable().OrderBy(o => o).Last();
            return Rows.Field<long>("Id1").ToString();
        }

        public void DeleteOrganigrama(string MaxId1)
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
            boolOrganigramaFederal = false;
        }

        public void InsertRegistroOrganigrama(string TipoRegistro, string NombrePuesto, string ID, string AbogadoIrresponsable, int Sec, Func<string, int> Print)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "INSERT INTO OrganigramaFederal (TipoRegistro, NombrePuesto, Abogado, ID, Sec) VALUES ('" + TipoRegistro + "', '" + NombrePuesto + "', '" + AbogadoIrresponsable + "', '" +
                    ID + "', '" + Sec.ToString() + "');";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    Print(cmd.CommandText);
                    cn.Open();
                    if (cmd.ExecuteNonQuery() == 0) MessageBox.Show("Error al insertar datos");
                    cn.Close();
                }
            }
            boolOrganigramaFederal = false;
        }

        public bool VerificaAbogadoIrresponsable(string AbogadoIrresponsable, string Password)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            DataTableAbogados = new System.Data.DataTable();
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "SELECT Clave FROM Abogados where Nombre = @Nombre";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Nombre", OleDbType.VarChar, 80).Value = AbogadoIrresponsable;
                    cn.Open();
                    DataTableAbogados.Load(cmd.ExecuteReader());
                    cn.Close();
                }
                DataRow[] password = DataTableAbogados.Select();
                if ((password.Length > 0) && (password[0]["Clave"].ToString().Equals(PetaSecure.ComputeSha256Hash(Password))))
                {
                    // el user y el password coinciden
                    return true;
                }
                return false;
            }
        }

        public DataRow[] GetDatosDeContacto(string ID)
        {
            //var sql = "SELECT * FROM DatosContacto where ID = @ID;";
            if (!boolDatosContacto)
            {
                UpLoadDatosContacto();
                boolDatosContacto = false;
            }
            //return (DataTableDatosContacto.Select("where ID = " + ID));
            var renglón = DataTableDatosContacto.AsEnumerable().Where(x => x.Field<string>("ID").Equals(ID));
            DataTable renglones = new DataTable();
            int i = 0;
            foreach (DataRow ren in renglón)
            {
                renglones.Rows.Add(ren);
                i++;
            }
         
            return renglones.Select();
        }

        public void AltaUserPassword(string Nombre, string Clave)
        {
            Builder.Provider = Defines.StringAccessProvider;
            Builder.DataSource = Path.Combine(Defines.DataBasePath, Defines.DataBaseFileName);
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                var sql = "DELETE FROM Abogados where Nombre = @Nombre;";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Nombre", OleDbType.VarChar, 80).Value = Nombre;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
                sql = "INSERT INTO Abogados (Nombre, Clave) values (@Nombre, @Clave);";
                using (OleDbCommand cmd = new OleDbCommand { CommandText = sql, Connection = cn })
                {
                    cmd.Parameters.Add("@Nombre", OleDbType.VarChar, 80).Value = Nombre;
                    cmd.Parameters.Add("@Clave", OleDbType.VarChar, 80).Value = Clave;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            //boolAbogados = false;
        }
    }
}
