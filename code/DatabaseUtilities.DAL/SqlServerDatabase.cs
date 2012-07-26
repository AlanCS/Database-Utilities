using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseUtilities.DAL
{

    public partial class SqlServerDatabase : IDisposable, DatabaseUtilities.DAL.IDatabase
    {
        SqlConnection con = new SqlConnection();

        private List<string> GetList(string sql)
        {
            var list = new List<string>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(reader.GetString(0));
                    reader.Close();
                }
            }

            return list;
        }

        public string GetStoredProcedureBody(string SPName)
        {
            return GetString(String.Format("sp_helptext '{0}'", SPName));
        }

        public DataSet ExecuteStoredProcedure(string SPName, IEnumerable<Column> spParams, out string error)
        {
            error = string.Empty;
            DataSet result = new DataSet();
            using (var com = con.CreateCommand())
            {
                com.CommandText = SPName;
                com.CommandType = System.Data.CommandType.StoredProcedure;

                foreach (var column in spParams)
                    com.Parameters.Add(new SqlParameter(column.Name, column.GetSampleValue(false)));

                using (var adapter = new SqlDataAdapter(com))
                {
                    try
                    {
                        adapter.Fill(result);
                    }
                    catch (Exception ex)
                    {
                        error = ex.ToString();
                        return null;
                    }

                }
            }


            return result;
        }

        private string GetString(string sql)
        {
            return string.Join("", GetList(sql));
        }

        public List<string> GetTables(string name = "", string column = "")
        {
            string filters = string.Empty;

            if (name != string.Empty)
                filters += String.Format(" and (t.name like '%{0}%' or schema_Name(schema_id) like '%{0}%') ", name);

            if (column != string.Empty)
                filters += String.Format(" and exists ( select 1 from syscolumns c where t.object_id = c.id and c.name like '%{0}%')", column);

            //return GetList(string.Format("select name from sysobjects o where lower(xtype)='u' {0} order by name asc", filters));

            return GetList(string.Format("select '[' + schema_Name(schema_id) + '].[' + t.name + ']' from sys.tables t where 1=1 {0} order by 1 asc", filters));
        }

        public List<string> GetStoredProcedures(string name = "", string column = "")
        {
            string filters = string.Empty;

            if (name != string.Empty)
                filters += String.Format(" and (t.name like '%{0}%' or schema_Name(schema_id) like '%{0}%') ", name);

            if (column != string.Empty)
                filters += String.Format(" and exists ( select 1 from syscolumns c where t.object_id = c.id and c.name like '%{0}%')", column);

            return GetList(string.Format("select '[' + schema_Name(schema_id) + '].[' + t.name + ']' from sys.procedures t where 1=1 {0} order by 1 asc", filters));
        }


        public List<Column> GetColumnsForTable(string tableName)
        {
            string sql = string.Format(@"select c.name,	t.name,	c.length, c.isnullable,  c.xprec, c.xscale,
         case when exists ( select 1 from sysindexkeys		i  where i.id = object_id('{0}') and i.colid = c.colid  and i.indid = 1 ) then 1 
         else 0 end iskey,
        isnull(( select co.text from syscomments co  where co.id = c.cdefault ),'') colDefault
         from syscolumns		c, 
         systypes t 
         where c.id = object_id('{0}') 
         and c.xtype = t.xtype 
         and t.name <> 'sysname' 
         order by c.colid asc ", tableName);

            return GetColumns(sql);
        }



        public List<Column> GetColumnsForStoredProcedure(string sp)
        {
            string sql = string.Format(@"select c.name,	t.name,	c.length, c.isnullable,  c.xprec, c.xscale,
         case when exists ( select 1 from sysindexkeys		i  where i.id = object_id('{0}') and i.colid = c.colid  and i.indid = 1 ) then 1 
         else 0 end iskey,
        isnull(( select co.text from syscomments co  where co.id = c.cdefault ),'') colDefault
         from syscolumns		c, 
         systypes t 
         where c.id = object_id('{0}') 
         and c.xtype = t.xtype 
         and t.name <> 'sysname' 
         order by c.colid asc ", sp);
            return GetColumns(sql);
        }

        public string ChangeConnection(string connectionString)
        {
            if (con.State != System.Data.ConnectionState.Closed)
                con.Close();

            con.ConnectionString = connectionString;

            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return con.ConnectionString + Environment.NewLine + ex.ToString();
            }

            return string.Empty;
        }

        public string ChangeDatabase(string connectionString, string databaseName)
        {
            if (con.State != System.Data.ConnectionState.Closed)
                con.Close();

            string connString = connectionString.Replace("master;", databaseName + ";");
            con.ConnectionString = connString;

            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return connString + Environment.NewLine + ex.ToString();
            }

            return string.Empty;
        }


        public List<string> GetDatabases()
        {
            return GetList("select name from sysdatabases order by 1 asc");
        }


        private List<Column> GetColumns(string sql)
        {
            var list = new List<Column>();

            using (var com = con.CreateCommand())
            {
                com.CommandText = sql;

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var column = new Column();
                        column.Name = reader.GetString(0);
                        column.Type = reader.GetString(1);
                        column.Length = reader.GetInt16(2);
                        column.IsNullable = reader.GetInt32(3) == 0;
                        if (reader.FieldCount >= 5)
                        {
                            column.Scale = reader.GetByte(4);
                            column.Precision = reader.GetByte(5);
                            column.IsPrimaryKey = reader.GetInt32(6) == 1;
                            column.Default = reader.GetString(7);

                            if (column.Default.StartsWith("(("))
                                column.Default = column.Default.Substring(1, column.Default.Length - 2);
                        }

                        list.Add(column);
                    }

                    reader.Close();
                }
            }

            return list;
        }

        public void Dispose()
        {
            if (con.State != System.Data.ConnectionState.Closed)
                con.Close();

            con.Dispose();
        }
    }
}
