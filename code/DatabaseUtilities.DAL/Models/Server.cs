using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class Server
    {
        public Server()
        {
            this.Databases = new List<Database>();

        }
        public int Id { get; set; }
        public string Environment { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public string GetTreatedConnectionString()
        {
            return ConnectionString;
        }

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
