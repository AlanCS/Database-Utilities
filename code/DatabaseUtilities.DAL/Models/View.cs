using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class View  : DatabaseObjectWithColumns
    {

        public View()
        {
            this.Columns = new List<Column>();
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Schema, this.Name);
        }

    }
}
