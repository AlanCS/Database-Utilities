using DatabaseUtilities.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.Core.Models
{
    public static class Global
    {

        public static Database[] CurrentSearchableDatabases = new Database[0];

        public static Snapshot Snapshot = null;
    }
}
