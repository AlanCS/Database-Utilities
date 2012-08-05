using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;

namespace DatabaseUtilities.Core
{
    public partial class SqlServerCore : IDisposable
    {
        public DAL.IDatabase database = null;

        public SqlServerCore(DAL.IDatabase databaseImplementation = null)
        {
            if (databaseImplementation == null)
                database = new DAL.SqlServerDatabase();
            else
                database = databaseImplementation;
        }

        public delegate void ErrorOccurredHandler(string error);
        public event ErrorOccurredHandler ErrorOccurred;

        public List<DAL.Column> CurrentObjectColumns = new List<DAL.Column>();

        public bool ChangeConnection(System.Configuration.ConnectionStringSettings setting)
        {
            var error = database.ChangeConnection(setting.ConnectionString);

            if (error != string.Empty)
                ErrorOccurred.Invoke(error);

            return error == string.Empty;
        }

        public bool ChangeDatabase(System.Configuration.ConnectionStringSettings setting, string databaseName)
        {
            var error = database.ChangeDatabase(setting.ConnectionString, databaseName);


            if (error != string.Empty)
                ErrorOccurred.Invoke(error);

            return error == string.Empty;
        }

        public void GetTableColumns(string TableName)
        {
            CurrentObjectColumns.Clear();
            CurrentObjectColumns.AddRange(database.GetColumnsForTable(TableName));
        }

        public void GetStoredProcedureColumns(string SP)
        {
            CurrentObjectColumns.Clear();
            CurrentObjectColumns.AddRange(database.GetColumnsForStoredProcedure(SP));
        }


        public List<string> GetDatabases()
        {
            return database.GetDatabases();
        }

        public List<string> GetStoredProcedures(string name = "", string column = "")
        {
            return database.GetStoredProcedures(name, column);

        }

        public List<string> GetTables(string name = "", string column = "")
        {
            return database.GetTables(name, column);
        }

        public string GenerateCSharpCodeForSP(string SPName, bool CanExecuteSP)
        {
            var code = string.Empty;

            code = @"using (var com = con.CreateCommand())
{
    com.CommandText = """ + SPName + @""";
    com.CommandType = System.Data.CommandType.StoredProcedure;

";
            code += string.Join(Environment.NewLine, CurrentObjectColumns.Select(c => @"    com.Parameters.Add(new SqlParameter(""" + c.Name + @""", System.Data.SqlDbType." + c.GetTypeVisualStudio() + ", " + c.Length + ") { Value = " + c.Name.Substring(1) + " });"));

            code += Environment.NewLine + Environment.NewLine + @"   if (con.State != ConnectionState.Open)";
            code += Environment.NewLine + @"      con.Open();" + Environment.NewLine + Environment.NewLine;

            var resultSets = new List<Dictionary<string, string>>();

            if (CanExecuteSP)
            {
                #region execute SP

                string error = string.Empty;

                var spResults = database.ExecuteStoredProcedure(SPName, CurrentObjectColumns, out error);

                if (error != string.Empty)
                {
                    ErrorOccurred.Invoke(error);
                    return string.Empty;
                }

                foreach (DataTable table in spResults.Tables)
                {
                    resultSets.Add(new Dictionary<string, string>());

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        var repeatedKeyTimes = resultSets.Last().Count(c => c.Key == table.Columns[i].ColumnName);

                        resultSets.Last().Add(table.Columns[i].ColumnName + (repeatedKeyTimes == 0 ? "" : (repeatedKeyTimes + 1).ToString()), table.Columns[i].DataType.Name);
                    }
                }


                #endregion
            }


            if (resultSets.Count == 0)
                code += @"   com.ExecuteNonQuery();";
            else if (resultSets.Count == 1 && resultSets[0].Count == 1) // 1 result with 1 columns
                code += @"   var dbReturn = Convert.ToInt32(com.ExecuteScalar());";
            else
            {
                code += @"   var reader = com.ExecuteReader();" + Environment.NewLine;
                var codeDataReading = new List<string>();
                foreach (var dic in resultSets)
                {
                    #region read fields

                    var readerText = @"   while (reader.Read())
   {
      var newItem = new MyClass();" + Environment.NewLine;

                    int index = 0;
                    foreach (var item in dic)
                    {
                        readerText += Environment.NewLine + @"      newItem." + item.Key + " = reader.Get" + item.Value + "(" + index + ");";
                        index++;
                    }

                    readerText += Environment.NewLine + Environment.NewLine + @"      list.Add(newItem);
   }" + Environment.NewLine;

                    codeDataReading.Add(readerText);

                    #endregion
                }
                code += string.Join(Environment.NewLine + "   reader.NextResult();" + Environment.NewLine, codeDataReading.ToArray());
                code += @"
   reader.Close();
   reader.Dispose();";
            }            

            code += Environment.NewLine + "}";

            return code;
        }

        public string GenerateCodeCreateTable(string TableName)
        {
            this.CurrentObjectColumns.Clear();
            this.CurrentObjectColumns.AddRange(database.GetColumnsForTable(TableName));

            var code = new StringBuilder();

            code.AppendFormat("create table {0} (" + Environment.NewLine, TableName);
            code.Append(string.Join("," + Environment.NewLine, CurrentObjectColumns.Select(c => c.ToSQL())));
            code.Append(Environment.NewLine + ")" + Environment.NewLine + "go");

            return code.ToString();
        }

        public string GenerateSP_UpdateInsert(string fullTableName)
        {
            string schema;
            string tableName;
            SplitTableName(fullTableName, out schema, out tableName);

            var SP = String.Format("{0}.[up_{1}]", schema, tableName);


            var code = new StringBuilder();

            code.Append("if exists (select 1 from sysobjects where name = '" + SP + "' and xtype='p')" + Environment.NewLine);
            code.Append("   drop proc " + SP + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine);

            code.Append("create proc " + SP + Environment.NewLine + Environment.NewLine);
            code.Append(string.Join("," + Environment.NewLine, CurrentObjectColumns.Select(c => "@" + c.GetNameAndType())));
            code.Append(Environment.NewLine + Environment.NewLine + "as");
            code.Append(Environment.NewLine + Environment.NewLine + "set nocount on");
            code.Append(Environment.NewLine + Environment.NewLine);

            code.Append("update " + fullTableName + " set " + Environment.NewLine);
            code.Append(string.Join("," + Environment.NewLine, CurrentObjectColumns.Where(c => c.IsPrimaryKey == false).Select(c => c.Name + " = @" + c.Name)));
            code.Append(Environment.NewLine + "where " + string.Join(" and " + Environment.NewLine, CurrentObjectColumns.Where(c => c.IsPrimaryKey).Select(c => c.Name + " = @" + c.Name)));

            code.Append(Environment.NewLine + Environment.NewLine + "if @@ROWCOUNT = 0");
            code.Append(Environment.NewLine + "   begin" + Environment.NewLine);

            code.Append("      insert into " + fullTableName + " (" + string.Join(", ", CurrentObjectColumns.Select(c => c.Name)) + ")" + Environment.NewLine);
            code.Append("      values (" + string.Join(", ", CurrentObjectColumns.Select(c => "@" + c.Name)) + ")");
            code.Append(Environment.NewLine + "   select @@identity");
            code.Append(Environment.NewLine + "   end");
            code.Append(Environment.NewLine + "go");

            return code.ToString();
        }

        public string GenerateSP_Delete(string fullTableName)
        {
            string schema;
            string tableName;
            SplitTableName(fullTableName, out schema, out tableName);

            var SP = String.Format("{0}.[de_{1}]", schema, tableName);

            var code = new StringBuilder();

            code.Append("if exists (select 1 from sysobjects where name = '" + SP + "' and xtype='p')" + Environment.NewLine);
            code.Append("   drop proc " + SP + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine);


            code.Append("create proc " + SP + Environment.NewLine + Environment.NewLine);
            code.Append(string.Join("," + Environment.NewLine, CurrentObjectColumns.Where(c => c.IsPrimaryKey).Select(c => "@" + c.GetNameAndType())));
            code.Append(Environment.NewLine + Environment.NewLine + "as" + Environment.NewLine + Environment.NewLine);
            code.Append("delete from " + fullTableName + Environment.NewLine);
            code.Append("where " + string.Join(" and " + Environment.NewLine, CurrentObjectColumns.Where(c => c.IsPrimaryKey).Select(c => c.Name + " = @" + c.Name)) + Environment.NewLine);
            code.Append(Environment.NewLine + "go");

            return code.ToString();
        }

        public string GenerateSP_Select(string fullTableName)
        {
            var primaryKeys = CurrentObjectColumns.Where(c => c.IsPrimaryKey).ToArray();

            string schema;
            string tableName;
            SplitTableName(fullTableName, out schema, out tableName);

            var initials = GetAlias(tableName);

            var SP = String.Format("{0}.[se_{1}]", schema, tableName);

            var code = new StringBuilder();

            code.Append("if exists (select 1 from sysobjects where name = '" + SP + "' and lower(xtype)='p')" + Environment.NewLine);
            code.Append("   drop proc " + SP + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine);

            code.Append("create proc " + SP + Environment.NewLine + Environment.NewLine);
            code.Append(string.Join("," + Environment.NewLine, primaryKeys.Select(c => "@" + c.GetNameAndType())));
            code.Append(Environment.NewLine + Environment.NewLine + "as" + Environment.NewLine + Environment.NewLine);
            code.Append("select " + string.Join("," + Environment.NewLine, CurrentObjectColumns.Select(c => "\t" + initials + "." + c.Name)) + Environment.NewLine);
            code.Append("from " + fullTableName + "\t" + initials + " (nolock)" + Environment.NewLine);
            if (primaryKeys.Count() > 0)
                code.Append("where " + string.Join(" and " + Environment.NewLine, primaryKeys.Select(c => initials + "." + c.Name + " = @" + c.Name)) + Environment.NewLine);
            code.Append(Environment.NewLine + "go" + Environment.NewLine);
            code.Append("--exec " + SP + " " + string.Join(",", primaryKeys.Select(c => c.GetSampleValue(true))));

            return code.ToString();
        }

        private static void SplitTableName(string fullTableName, out string schema, out string tableName)
        {
            schema = fullTableName.Split('.')[0];
            tableName = fullTableName.Split('.')[1].Replace("[", "").Replace("]", "");
        }

        public string GetAlias(string tableName)
        {
            var initials = string.Join("", new Regex("[A-Z]").Matches(tableName).OfType<Match>().Select(c => c.Value));

            if (initials != string.Empty)
                return initials;

            initials = string.Join("", tableName.Split('_').Select(c => c[0]));

            if (initials != string.Empty)
                return initials;

            initials = tableName[0].ToString();

            return initials;
        }

        public void Dispose()
        {
            database.Dispose();
        }

        public string GenerateCodeForStoredProcedure(string SelectedStoredProcedure)
        {
            string spText = string.Empty;

            try
            {
                spText = database.GetStoredProcedureBody(SelectedStoredProcedure);
            }
            catch (SqlException ex)
            {
                ErrorOccurred.Invoke("Error when getting SP: " + Environment.NewLine + ex.ToString());
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorOccurred.Invoke("Error when getting SP: " + Environment.NewLine + ex.ToString());
                return string.Empty;
            }

            var code = new StringBuilder();
            code.Append(String.Format("if exists (select 1 from sysobjects where name = '{0}' and lower(xtype)='p'){1}", SelectedStoredProcedure, Environment.NewLine));
            code.Append(String.Format("   drop proc {0}{1}go{1}", SelectedStoredProcedure, Environment.NewLine));
            code.Append(spText);
            code.Append(String.Format("{0}go{0}", Environment.NewLine));
            //code.Append("--exec " + SelectedStoredProcedure + " " + string.Join(",", CurrentObjectColumns.Select(c => c.GetSampleValue(true))));
            //code.Append(String.Format("{0}go{0}", Environment.NewLine));

            return code.ToString();
        }
    }
}
