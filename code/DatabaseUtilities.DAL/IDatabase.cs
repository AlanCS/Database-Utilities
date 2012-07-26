using System;
namespace DatabaseUtilities.DAL
{
    public interface IDatabase
    {
        string ChangeConnection(string connectionString);
        string ChangeDatabase(string connectionString, string databaseName);
        System.Data.DataSet ExecuteStoredProcedure(string SPName, System.Collections.Generic.IEnumerable<Column> spParams, out string error);
        System.Collections.Generic.List<Column> GetColumnsForStoredProcedure(string sp);
        System.Collections.Generic.List<Column> GetColumnsForTable(string tableName);
        System.Collections.Generic.List<string> GetDatabases();
        string GetStoredProcedureBody(string SPName);
        System.Collections.Generic.List<string> GetStoredProcedures(string name = "", string column = "");
        System.Collections.Generic.List<string> GetTables(string name = "", string column = "");
        void Dispose();
    }
}
