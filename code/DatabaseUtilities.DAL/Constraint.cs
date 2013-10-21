using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class Constraint
    {
        public string Name { get; set; }
        public string Table { get; set; }

        public override string ToString()
        {
            return Table;
        }
    }
}
