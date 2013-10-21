using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.Core.Models
{
    public class DatabaseObjectWithParents
    {
        public DatabaseObjectWithParents(DAL.DatabaseObjectWithColumns obj)
        {
            this.Object = obj;

            var dups = Global.CurrentSearchableDatabases.GroupBy(c => c.DatabaseServerId).Where(c => c.Count() > 1).ToArray();

            var database = Global.CurrentSearchableDatabases.SingleOrDefault(c => c.DatabaseServerId == obj.DatabaseServerId);
            this.Database = database.Name;
            this.DatabaseServerId = obj.DatabaseServerId;

            var server = Global.Snapshot.Servers.SingleOrDefault(c => c.Id == database.ServerId);
            this.Server = server.Name;
            this.Environment = server.Environment;
            
        }
        public DAL.DatabaseObjectWithColumns Object { get; set; }

        public string ObjectType
        {
            get
            {
                if (Object is DAL.Table) return "Table";
                if (Object is DAL.StoredProcedure) return "Stored Procedure";
                return "View";
            }

        }

        public long DatabaseServerId { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string Environment { get; set; }
    }
}
