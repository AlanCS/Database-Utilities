using DatabaseUtilities.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.UI
{
    public class Global
    {

        public static Database[] CurrentSearchableDatabases = new Database[0];

        public static string SearchDBText = string.Empty;

        public static Snapshot Snapshot = null;
    }
}
