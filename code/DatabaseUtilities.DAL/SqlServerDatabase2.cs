using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using DatabaseUtilities.DAL.Config;

namespace DatabaseUtilities.DAL
{

    public partial class SqlServerDatabase2
    {
        public List<Connection> GetConnections()
        {
            var conns = ConfigurationManager.GetSection("CustomConnections") as Config.CustomConnectionSection;
            var list = new List<Connection>();

            var sqlConnection = new SqlConnection();

            foreach (var configConnection in conns.Instances.Cast<MyConfigInstanceElement>())
	        {
                var connection = new Connection() { FullConnectionString = configConnection.Connection + ";Initial Catalog=master;", Name = configConnection.Name, Group = configConnection.Group, Id = configConnection.id };

                sqlConnection.ConnectionString = connection.FullConnectionString;

                try
                {                    
                    sqlConnection.Open();
                    sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    continue; // don't add failing connection to list
                }

                list.Add(connection);
	        }


            return list;
        }

        public List<Database> GetDatabases(Connection connection)
        {
            var list = new List<Database>();


            var sql = @"with fs
as
(
    select database_id, type, size * 8.0 size
    from sys.master_files
)
select database_id,
		name,
    (select sum(size) from fs where type = 0 and fs.database_id = db.database_id) DataFileSizeKb,
    (select sum(size) from fs where type = 1 and fs.database_id = db.database_id) LogFileSizeKb,
	create_date    
from sys.databases db 
where database_id > 4";

            using (var con = new SqlConnection(connection.FullConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(new Database()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            DataFilesSizeKb = reader.GetDecimal(2),
                            LogFilesSizeKb = reader.GetDecimal(3),
                            Created = reader.GetDateTime(4),
                            ConnectionId = connection.Id
                        });
                    reader.Close();
                }
                con.Close();
            }

            return list;
        }

        public List<StoredProcedure> GetStoredProcedures(Connection connection, Database DataBase, DateTime? sinceDate = null)
        {
            var list = new List<StoredProcedure>();

            #region query
            var sql = @"select t.object_id id, 
    schema_Name(schema_id) schemaName,
  t.name  ,
  t.create_date,
  t.modify_date
INTO #temp_items
  from [{0}].sys.procedures t
where 1=1  " + (sinceDate.HasValue ? " AND t.modify_date > '" + sinceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "' " : "") + @"

select * from #temp_items

 " + sqlColumns + @"

SELECT id, text 
FROM [{0}].dbo.syscomments
WHERE id in (SELECT Id FROM #temp_items ) 

drop table #temp_items";
            #endregion

            using (var con = new SqlConnection(connection.FullConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = string.Format(sql, DataBase.Name);

                using (var dataset = new System.Data.DataSet())
                {
                    con.Open();
                    new SqlDataAdapter(cmd).Fill(dataset);
                    con.Close();

                    dataset.Relations.Add(new DataRelation("columns", dataset.Tables[0].Columns[0], dataset.Tables[1].Columns[0]));
                    dataset.Relations.Add(new DataRelation("content", dataset.Tables[0].Columns[0], dataset.Tables[2].Columns[0]));

                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        var sp = new StoredProcedure()
                        {
                            Id = Convert.ToInt32(row[0]),
                            Schema = row[1] as string,
                            Name = row[2] as string,
                            CreatedDate = Convert.ToDateTime(row[3]),
                            LastModifiedDate = Convert.ToDateTime(row[4]),
                            DatabaseConnectionId = DataBase.DatabaseConnectionId
                        };

                        foreach (DataRow childRow in row.GetChildRows("columns"))
                        {
                            sp.Columns.Add(GetColumns(childRow));
                        }

                        foreach (DataRow childRow in row.GetChildRows("content"))
                        {
                            sp.Text = childRow[1].ToString().Trim();
                        }


                        list.Add(sp);
                    }
                }
            }

            return list;
        }

        public List<Table> GetTables(Connection connection, Database DataBase, DateTime? sinceDate = null)
        {
            var list = new List<Table>();

            #region query
            var sql = @"
with fs
as
(
select i.object_id,
		p.rows AS Rows,
		SUM(a.total_pages) * 8 AS TotalSpaceKb
from     [{0}].sys.indexes i INNER JOIN 
		[{0}].sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id INNER JOIN 
		 [{0}].sys.allocation_units a ON p.partition_id = a.container_id
WHERE 
    i.OBJECT_ID > 255 
GROUP BY 
    i.object_id,
    p.rows
)

SELECT t.object_id Id,
	s.name,
    t.NAME AS TableName,
    fs.Rows,
    fs.TotalSpaceKb,
    t.create_date,
    t.modify_date
INTO #temp_items
FROM 
    [{0}].sys.tables t INNER JOIN      
    fs  ON t.OBJECT_ID = fs.object_id inner join
    sys.schemas s on t.schema_id = s.schema_id    
WHERE 
    t.NAME NOT LIKE 'dt%' " + (sinceDate.HasValue ? " AND t.modify_date > '" + sinceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "' " : "") + @"
    AND t.is_ms_shipped = 0
ORDER BY 
    t.Name

select * from #temp_items 

 " + sqlColumns + @"

drop table #temp_items
";
            #endregion

            using (var con = new SqlConnection(connection.FullConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = string.Format(sql, DataBase.Name);


                using (var dataset = new System.Data.DataSet())
                {
                    con.Open();
                    new SqlDataAdapter(cmd).Fill(dataset);
                    con.Close();

                    dataset.Relations.Add(new DataRelation("columns", dataset.Tables[0].Columns[0], dataset.Tables[1].Columns[0]));

                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        var table = new Table()
                        {
                            Id = Convert.ToInt32(row[0]),
                            Schema = row[1] as string,
                            Name = row[2] as string,
                            Rows = Convert.ToInt64(row[3]),
                            TotalSpaceKb = Convert.ToInt64(row[4]),
                            CreatedDate = Convert.ToDateTime(row[5]),
                            LastModifiedDate = Convert.ToDateTime(row[6]),
                            DatabaseConnectionId = DataBase.DatabaseConnectionId
                        };

                        foreach (DataRow childRow in row.GetChildRows("columns"))
                        {
                            table.Columns.Add(GetColumns(childRow));
                        }

                        list.Add(table);
                    }
                }
            }

            return list;
        }


        public List<View> GetViews(Connection connection, Database DataBase, DateTime? sinceDate = null)
        {
            var list = new List<View>();

            #region query
            var sql = @"
SELECT t.object_id Id,
	s.name,
    t.NAME AS ObjectName,
    t.create_date,
    t.modify_date
into #temp_items 
FROM 
    [{0}].sys.views t INNER JOIN      
    [{0}].sys.schemas s on t.schema_id = s.schema_id    
WHERE 
    1=1  " + (sinceDate.HasValue ? " AND t.modify_date > '" + sinceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "' " : "") + @"
ORDER BY 
    t.Name

select * from #temp_items 

 " + sqlColumns + @"

SELECT id, text 
FROM [{0}].dbo.syscomments
WHERE id in (SELECT Id FROM #temp_items ) 

drop table #temp_items
";
            #endregion

            using (var con = new SqlConnection(connection.FullConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = string.Format(sql, DataBase.Name);


                using (var dataset = new System.Data.DataSet())
                {
                    con.Open();
                    new SqlDataAdapter(cmd).Fill(dataset);
                    con.Close();

                    dataset.Relations.Add(new DataRelation("columns", dataset.Tables[0].Columns[0], dataset.Tables[1].Columns[0]));
                    dataset.Relations.Add(new DataRelation("content", dataset.Tables[0].Columns[0], dataset.Tables[2].Columns[0]));

                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        var view = new View()
                        {
                            Id = Convert.ToInt32(row[0]),
                            Schema = row[1] as string,
                            Name = row[2] as string,
                            CreatedDate = Convert.ToDateTime(row[3]),
                            LastModifiedDate = Convert.ToDateTime(row[4]),
                            DatabaseConnectionId = DataBase.DatabaseConnectionId
                        };

                        foreach (DataRow childRow in row.GetChildRows("columns"))
                        {
                            view.Columns.Add(GetColumns(childRow));
                        }

                        foreach (DataRow childRow in row.GetChildRows("content"))
                        {
                            view.Text = childRow[1].ToString().Trim();
                        }

                        list.Add(view);
                    }
                }
            }

            return list;
        }

        string sqlColumns = @"select c.Id, c.name,	t.name,	c.length, c.isnullable,  c.xprec, c.xscale,
         case when exists ( select 1 from [{0}].dbo.sysindexkeys		i  where i.id = c.id and i.colid = c.colid  and i.indid = 1 ) then 1 
         else 0 end iskey,
        isnull(( select co.text from [{0}].dbo.syscomments co  where co.id = c.cdefault ),'') colDefault
         from [{0}].dbo.syscolumns		c, 
         [{0}].dbo.systypes t 
         where c.id in ( select Id from #temp_items )
         and c.xtype = t.xtype 
         and t.name <> 'sysname' 
         order by c.colid asc ";

        private Column GetColumns(DataRow row)
        {

                var column = new Column();
                column.Name = row[1] as string;
                column.Type = row[2] as string;
                column.Length = Convert.ToInt16(row[3]);

                column.IsNullable = Convert.ToInt32(row[4]) == 0;

                column.Scale = Convert.ToByte(row[5]);
                column.Precision = Convert.ToByte(row[6]);
                column.IsPrimaryKey = Convert.ToInt32(row[7]) == 1;
                    column.Default = row[8] as string;

                    if (column.Default.StartsWith("(("))
                        column.Default = column.Default.Substring(1, column.Default.Length - 2);
  

                return column;
        }

        private List<Column> GetColumns(SqlDataReader reader)
        {
            var list = new List<Column>();

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

            return list;
        }
    }
}
