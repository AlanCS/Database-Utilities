﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class Snapshot
    {
        public Snapshot()
        {
            Servers = new List<Server>();
        }
        public List<Server> Servers { get; set; }

        public DifferentialSnapshot GetChangesSince(DateTime time)
        {
            var result = new DifferentialSnapshot();

            result.TablesAddedOrChanged = this.Servers.SelectMany(c => c.Databases.SelectMany(d => d.Tables)).Where(c => c.LastModifiedDate > time).ToList();
            result.StoredProceduresAddedOrChanged = this.Servers.SelectMany(c => c.Databases.SelectMany(d => d.StoredProcedures)).Where(c => c.LastModifiedDate > time).ToList();
            result.ViewsAddedOrChanged = this.Servers.SelectMany(c => c.Databases.SelectMany(d => d.Views)).Where(c => c.LastModifiedDate > time).ToList();

            return result;
        }

        public DateTime? SnapshotTaken { get; set; }

        public DateTime? LastUpdatedObject
        {
            get
            {
                var lastUpdates = new List<DateTime>();

                lastUpdates.Add(Servers.SelectMany(c => c.Databases).Max(c => c.Created));
                lastUpdates.Add(Servers.SelectMany(c => c.Databases).SelectMany(c => c.Tables).Max(c => c.LastModifiedDate));
                lastUpdates.Add(Servers.SelectMany(c => c.Databases).SelectMany(c => c.StoredProcedures).Max(c => c.LastModifiedDate));
                lastUpdates.Add(Servers.SelectMany(c => c.Databases).SelectMany(c => c.Views).Max(c => c.LastModifiedDate));
                return lastUpdates.Max();
            }
        }

        public class DifferentialSnapshot
        {
            public DifferentialSnapshot()
            {
                TablesAddedOrChanged = new List<Table>();
                StoredProceduresAddedOrChanged = new List<StoredProcedure>();
                ViewsAddedOrChanged = new List<View>();
            }
            public List<Table> TablesAddedOrChanged { get; set; }
            public List<StoredProcedure> StoredProceduresAddedOrChanged { get; set; }
            public List<View> ViewsAddedOrChanged { get; set; }

            public override string ToString()
            {
                return string.Format("{0} tables added or changed & {1} SPs added or changed & {2} Views added or changed", this.TablesAddedOrChanged.Count, this.StoredProceduresAddedOrChanged.Count, this.ViewsAddedOrChanged.Count);
            }
        }
    }
}
