using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.UI.Models
{
    public class Environment
    {
        public Environment(string name, IEnumerable<DAL.Server> servers)
        {
            this.Name = name;
            Servers = new ObservableCollection<DAL.Server>();
            foreach (var item in servers) Servers.Add(item);
        }

        public string Name { get; set; }

        public ObservableCollection<DAL.Server> Servers { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
