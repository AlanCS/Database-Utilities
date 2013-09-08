using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class Connection
    {
        public Connection()
        {
            this.Databases = new List<Database>();

        }
        public int Id { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string FullConnectionString { get; set; }
        public List<Database> Databases { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
