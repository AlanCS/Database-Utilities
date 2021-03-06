﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public class StoredProcedure : DatabaseObjectWithColumns
    {
        public StoredProcedure()
        {
            this.Columns = new List<Column>();
            this.Text = string.Empty;
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Schema, this.Name);
        }

    }
}
