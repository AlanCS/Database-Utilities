using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class DatabaseService
    {
        SqlServerDatabase2 sqlserver = new SqlServerDatabase2();

        public Snapshot GetFullSnapshot()
        {
            var result = new Snapshot();

            foreach (var connection in sqlserver.GetConnections())
            {
                foreach (var database in sqlserver.GetDatabases(connection))
                {
                    database.Tables = sqlserver.GetTables(connection, database);
                    database.StoredProcedures = sqlserver.GetStoredProcedures(connection, database);
                    database.Views = sqlserver.GetViews(connection, database);
                    connection.Databases.Add(database);
                }

                if (connection.Databases.Count == 0) continue; // don't add connections with no databases
                result.Connections.Add(connection);
            }


            return result;
        }

        public void FillSnapshotWithLatestChanges(Snapshot snapshot, DateTime? sinceDate)
        {
            foreach (var connection in snapshot.Connections)
            {
                foreach (var databaseFromDB in sqlserver.GetDatabases(connection))
                {
                    var databaseInMemory = connection.Databases.SingleOrDefault(c => c.Id == databaseFromDB.Id);

                    if (databaseInMemory == null)
                    {
                        connection.Databases.Add(databaseFromDB);
                        databaseInMemory = databaseFromDB;
                    }

                    var changedTables = sqlserver.GetTables(connection, databaseFromDB, sinceDate);
                    if (changedTables.Count > 0)
                    {
                        databaseInMemory.Tables.RemoveAll(c => changedTables.Select(d => d.Id).Contains(c.Id));
                        databaseInMemory.Tables.AddRange(changedTables);
                    }

                    var changedStoredProcedures = sqlserver.GetStoredProcedures(connection, databaseFromDB, sinceDate);
                    if (changedStoredProcedures.Count > 0)
                    {
                        databaseInMemory.StoredProcedures.RemoveAll(c => changedStoredProcedures.Select(d => d.Id).Contains(c.Id));
                        databaseInMemory.StoredProcedures.AddRange(changedStoredProcedures);
                    }

                    var changedViews = sqlserver.GetViews(connection, databaseFromDB, sinceDate);
                    if (changedViews.Count > 0)
                    {
                        databaseInMemory.Views.RemoveAll(c => changedViews.Select(d => d.Id).Contains(c.Id));
                        databaseInMemory.Views.AddRange(changedViews);
                    }

                }
            }
        }

        public Snapshot GetCachedSnapshot()
        {
            Snapshot snapshot = ApplicationKeeper.Get("LatestSnapshot") as Snapshot;

            if (snapshot == null)
            {
                snapshot = GetFullSnapshot();
                ApplicationKeeper.AddUpdate("LatestSnapshot", snapshot);
            }
            else
            {
                if (ShouldUpdateCachedSnapshot())
                    lock(this)
                    {
                        if (ShouldUpdateCachedSnapshot())
                        {
                            FillSnapshotWithLatestChanges(snapshot, snapshot.LastUpdatedObject);
                            ApplicationKeeper.AddUpdate("LastTimeCheckedForUpdates", DateTime.Now);
                        }
                    }
            }

            return snapshot;
        }

        private bool ShouldUpdateCachedSnapshot()
        {
            var LastTimeCheckedForUpdates = ApplicationKeeper.Get("LastTimeCheckedForUpdates") as DateTime?;
            if (!LastTimeCheckedForUpdates.HasValue) return true;

            return LastTimeCheckedForUpdates.Value.AddMinutes(2) < DateTime.Now;
        }

        public Database GetDatabase(ulong DatabaseConnectionId)
        {
            var database = GetCachedSnapshot().Connections.SelectMany(c => c.Databases).SingleOrDefault(c => c.DatabaseConnectionId == DatabaseConnectionId);

            return database;
        }
    }
}
