using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{

    public class Column
    {
        public string Name = string.Empty;
        public string Type = string.Empty;
        public Int16 Length = 0;
        public bool IsNullable = false;
        public Int32 Precision = 0;
        public Int32 Scale = 0;
        public bool IsPrimaryKey = false;
        public string Default = string.Empty;

        public override string ToString()
        {
            return Name + " " + Type;
        }

        public string GetTypeVisualStudio()
        {
            return this.Type.Replace("char", "Char").Replace("var", "Var").Replace("int", "Int").Replace("date", "Date").Replace("time", "Time").Replace("small", "Small").Replace("bit", "Bit").Replace("tiny", "Tiny").Replace("money", "Money");
        }

        public string GetSampleValue(bool WithAmpers)
        {
            if (this.Type.Contains("char"))
                if (WithAmpers)
                    return "'abc'";
                else
                    return "abc";

            if (this.Type.Contains("date"))
                if (WithAmpers)
                    return "'2011-01-07'";
                else
                    return "2011-01-07";


            return "1";
        }

        public string ToSQL()
        {
            var line = new StringBuilder();
            line.Append(this.GetNameAndType());

            line.Append(new String(' ', 70 - line.Length));
            line.Append("\t" + (this.IsNullable ? " null" : " not null"));
            if (this.Default != "")
                line.Append("\t default" + this.Default);
            if (this.IsPrimaryKey)
                line.Append("\t primary key");
            return line.ToString();
        }

        public string GetNameAndType()
        {
            var line = new StringBuilder();
            line.Append(this.Name);
            var position = 40 - line.Length;
            if (position < 0) position = 0;
            line.Append(new String(' ', 40 - line.Length));
            line.Append(this.Type);
            if (this.Type.IndexOf("char") >= 0)
                line.Append("(" + this.Length + ")");
            if (this.Type == "numeric")
                line.Append("(" + this.Scale + ", " + this.Precision + ")");
            return line.ToString();
        }
    }
}
