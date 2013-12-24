using System;
using System.Collections.Generic;
using System.IO;
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

            foreach (var server in sqlserver.GetServers())
            {
                foreach (var database in sqlserver.GetDatabases(server))
                {
                    sqlserver.FillDatabase(server, database);

                    // only add if it contains something to see
                    // it might not have anything if user has no permission to read that given database
                    // or the DB is genuinely empty
                    if(database.Views.Count > 0 || database.Tables.Count > 0 || database.StoredProcedures.Count > 0 ) 
                        server.Databases.Add(database);
                }

                if (server.Databases.Count == 0) continue; // don't add connections with no databases
                result.Servers.Add(server);
            }

            result.SnapshotTaken = DateTime.Now;
            return result;
        }

        public void FillSnapshotWithLatestChanges(Snapshot snapshot, DateTime? sinceDate)
        {
            foreach (var connection in snapshot.Servers)
            {
                foreach (var databaseFromDB in sqlserver.GetDatabases(connection))
                {
                    var databaseInMemory = connection.Databases.SingleOrDefault(c => c.Id == databaseFromDB.Id);

                    if (databaseInMemory == null)
                    {
                        connection.Databases.Add(databaseFromDB);
                        databaseInMemory = databaseFromDB;
                    }

                }
            }
        }

        public Snapshot GetCachedSnapshot(bool forceRefresh = false)
        {
            Snapshot snapshot = ApplicationKeeper.Get("LatestSnapshot") as Snapshot;

            if (forceRefresh) snapshot = null;

            if (snapshot == null)
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Snapshot));

                var fileName = Path.GetTempPath() + @"\LatestSnapshot.xml";


                if (File.Exists(fileName) && forceRefresh == false)
                    try
                    {
                        snapshot = (Snapshot)serializer.Deserialize(new FileStream(fileName, FileMode.Open, FileAccess.Read));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        snapshot = null;
                    }                    
                
                if(snapshot == null)
                {
                    snapshot = GetFullSnapshot();
                    try
                    {
                        serializer.Serialize(new FileStream(fileName, FileMode.Create), snapshot);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

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

        public Database GetDatabase(long DatabaseConnectionId)
        {
            var database = GetCachedSnapshot().Servers.SelectMany(c => c.Databases).SingleOrDefault(c => c.DatabaseServerId == DatabaseConnectionId);

            return database;
        }
    }
}
