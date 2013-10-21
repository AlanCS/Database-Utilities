using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class DatabaseObjectWithColumns
    {
        public DatabaseObjectWithColumns()
        {
            this.ObjectsDependingOnThis = new List<int>();
            this.ObjectsThatThisDependsOn = new List<int>();
        }

        public long DatabaseServerId { get; set; }

        public int Id { get; set; }
        public List<Column> Columns { get; set; }

        public List<int> ObjectsThatThisDependsOn { get; set; }
        public List<int> ObjectsDependingOnThis { get; set; }


        public string Schema { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public string SchemaAndName
        {
            get
            {
                return Schema + "." + Name;
            }
        }

        public string FormattedSchemaAndName
        {
            get
            {
                return "[" + Schema + "].[" + Name + "]";
            }
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
