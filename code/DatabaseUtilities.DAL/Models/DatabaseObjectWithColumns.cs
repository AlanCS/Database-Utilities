using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class DatabaseObjectWithColumns
    {
        public ulong DatabaseConnectionId { get; set; }

        public int Id { get; set; }
        public List<Column> Columns { get; set; }


        public string Schema { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
