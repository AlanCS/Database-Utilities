using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL.Models
{
    [Serializable]
    public partial class Connection
    {
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return ConnectionString.GetHashCode();
        }

    }

    [Serializable]
    public partial class Environment
    {
        public Environment()
        {
            this.Connections = new List<Connection>();
        }
        public string Name { get; set; }

        public List<Connection> Connections { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}
