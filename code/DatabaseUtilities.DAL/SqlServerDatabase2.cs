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
        public List<Server> GetServers()
        {
            var conns = ConfigurationManager.GetSection("CustomConnections") as Config.CustomConnectionSection;
            var list = new List<Server>();

            var sqlConnection = new SqlConnection();

            foreach (var configConnection in conns.Instances.Cast<MyConfigInstanceElement>())
            {
                var connection = new Server() { ServerName = configConnection.Server, Name = configConnection.Name, Environment = configConnection.Environment, Id = configConnection.id };

                sqlConnection.ConnectionString = connection.ConnectionString;

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

        public List<Database> GetDatabases(Server connection)
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
    isnull((select sum(size) from fs where type = 0 and fs.database_id = db.database_id),0) DataFileSizeKb,
    isnull((select sum(size) from fs where type = 1 and fs.database_id = db.database_id),0) LogFileSizeKb,
	create_date    
from sys.databases db 
where database_id > 4";

            using (var con = new SqlConnection(connection.ConnectionString))
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
                            ServerId = connection.Id
                        });
                    reader.Close();
                }
                con.Close();
            }

            return list;
        }


        public void FillDatabase(Server server, Database DataBase, DateTime? sinceDate = null)
        {
            var list = new List<Table>();

            if (sinceDate == null) sinceDate = Convert.ToDateTime("1900-01-01"); // minimum date storable in SQL server

            #region query
            var sql = @"
set nocount on

declare @sinceDate  smalldatetime

set @sinceDate = '" + sinceDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff") + @"'


select i.object_id,
		avg(p.rows) AS Rows,
		SUM(a.total_pages) * 8 AS TotalSpaceKb
into #fs
from     [{0}].sys.indexes i INNER JOIN 
		[{0}].sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id INNER JOIN 
		 [{0}].sys.allocation_units a ON p.partition_id = a.container_id
WHERE 
    i.OBJECT_ID > 255 
GROUP BY 
    i.object_id

SELECT t.object_id Id,
	s.name [schema],
    t.NAME AS name,
    t.create_date,
    t.modify_date,
    fs.Rows,
    fs.TotalSpaceKb,
    't' as type
INTO #temp_items
FROM 
    [{0}].sys.tables t INNER JOIN      
    #fs fs  ON t.OBJECT_ID = fs.object_id inner join
    [{0}].sys.schemas s on t.schema_id = s.schema_id    
WHERE 
    t.NAME NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND t.modify_date > @sinceDate
ORDER BY 
    t.Name

drop table #fs

insert into #temp_items (Id, [schema], name, create_date, modify_date, type)
select t.object_id Id, 
    s.name,
  t.name ,
  t.create_date,
  t.modify_date,
  'p'
  from [{0}].sys.procedures         t,
		[{0}].sys.schemas           s
where s.schema_id = t.schema_id  
    AND t.modify_date > @sinceDate

insert into #temp_items (Id, [schema], name, create_date, modify_date, type)
SELECT t.object_id,
	s.name,
    t.NAME,
    t.create_date,
    t.modify_date,
    'v'
FROM 
    [{0}].sys.views t INNER JOIN      
    [{0}].sys.schemas s on t.schema_id = s.schema_id    
WHERE 
   t.modify_date > @sinceDate
ORDER BY 
    t.Name

select * from #temp_items order by type asc

select c.Id, 
        c.name,	
        t.name,	
        c.length, 
        c.isnullable,  
        c.xprec, 
        c.xscale,
        case when exists ( select 1 
			from [{0}].sys.indexes I, [{0}].sys.index_columns IC  
			where I.object_id = c.id and I.is_primary_key = 1 and I.object_id = IC.object_id AND I.index_id = IC.index_id and IC.column_id  = c.colid  ) 
		then 1  else 0 end iskey,
        isnull(( select co.text from [{0}].dbo.syscomments co  where co.id = c.cdefault ),'') colDefault
from [{0}].dbo.syscolumns		c, 
    [{0}].dbo.systypes          t 
where c.id in ( select t.Id from #temp_items t  )
        and c.xtype = t.xtype 
        and t.name <> 'sysname' 
        order by c.colid asc

SELECT id, text 
FROM [{0}].dbo.syscomments
WHERE id in (select t.Id from #temp_items t  where type in ('v','p')  ) 

select object_id, referenced_major_id
from [{0}].sys.sql_dependencies
where object_id in ( select Id from #temp_items )
union all
select parent_object_id, referenced_object_id
 from [{0}].sys.foreign_key_columns
where parent_object_id in ( select Id from #temp_items where type = 't' )

select referenced_major_id, object_id
from [{0}].sys.sql_dependencies
where  referenced_major_id in ( select Id from #temp_items )
union all
select referenced_object_id,  parent_object_id
 from [{0}].sys.foreign_key_columns
where referenced_object_id in ( select Id from #temp_items where type = 't' )

drop table #temp_items
";
            #endregion

            using (var con = new SqlConnection(server.ConnectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = string.Format(sql, DataBase.Name);

                using (var dataset = new System.Data.DataSet())
                {
                    con.Open();
                    try
                    {
                        new SqlDataAdapter(cmd).Fill(dataset);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        return;
                    }
                    finally
                    {
                        con.Close();
                    }

                    dataset.Relations.Add(new DataRelation("columns", dataset.Tables[0].Columns[0], dataset.Tables[1].Columns[0]));
                    dataset.Relations.Add(new DataRelation("content", dataset.Tables[0].Columns[0], dataset.Tables[2].Columns[0]));
                    dataset.Relations.Add(new DataRelation("references", dataset.Tables[0].Columns[0], dataset.Tables[3].Columns[0]));
                    dataset.Relations.Add(new DataRelation("referencedBy", dataset.Tables[0].Columns[0], dataset.Tables[4].Columns[0]));

                    DAL.DatabaseObjectWithColumns currentObject = null;

                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        #region read table, stored procedure, view
                        var objectType = row["type"].ToString();
                        if (objectType == "t")
                        {
                            var table = new Table()
                            {
                                Rows = Convert.ToInt64(row[5]),
                                TotalSpaceKb = Convert.ToInt64(row[6]),
                            };

                            currentObject = table;
                            DataBase.Tables.Add(table);
                        }
                        else
                        {
                            if (objectType == "v")
                            {
                                var view = new View();
                                currentObject = view;
                                foreach (DataRow childRow in row.GetChildRows("content")) view.Text += childRow[1].ToString().Trim();
                                DataBase.Views.Add(view);
                            }
                            else
                            {
                                var storedProcedure = new StoredProcedure();
                                currentObject = storedProcedure;
                                foreach (DataRow childRow in row.GetChildRows("content")) storedProcedure.Text += childRow[1].ToString().Trim();
                                DataBase.StoredProcedures.Add(storedProcedure);
                            }
                        }

                        currentObject.Id = Convert.ToInt32(row[0]);
                        currentObject.Schema = row[1] as string;
                        currentObject.Name = row[2] as string;
                        currentObject.CreatedDate = Convert.ToDateTime(row[3]);
                        currentObject.LastModifiedDate = Convert.ToDateTime(row[4]);
                        currentObject.DatabaseServerId = DataBase.DatabaseServerId;

                        #endregion

                        foreach (DataRow childRow in row.GetChildRows("columns"))
                        {
                            currentObject.Columns.Add(GetColumns(childRow));
                        }

                        foreach (DataRow childRow in row.GetChildRows("references"))
                        {
                            currentObject.ObjectsThatThisDependsOn.Add(Convert.ToInt32(childRow[1]));
                        }

                        foreach (DataRow childRow in row.GetChildRows("referencedBy"))
                        {
                            currentObject.ObjectsDependingOnThis.Add(Convert.ToInt32(childRow[1]));
                        }

                    }
                }
            }
        }

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
