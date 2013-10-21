using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class Table  : DatabaseObjectWithColumns
    {
   
        public Table()
        {
            this.Columns = new List<Column>();
        }

        public long Rows { get; set; }
        public decimal TotalSpaceKb { get; set; }


        public override string ToString()
        {
            return string.Format("{0}.{1} ({2} rows)", this.Schema, this.Name, this.Rows);
        }

    }
}
