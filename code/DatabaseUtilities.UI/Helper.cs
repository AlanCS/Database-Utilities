using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.UI
{
    public static class Helper
    {
        public static string SizeSuffix(Int64 valueinKb)
        {
            string[] suffixes = { "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int i = 0;
            decimal dValue = (decimal)valueinKb;
            while (Math.Round(dValue / 1024) >= 1)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n1} {1}", dValue, suffixes[i]);
        }

        public static bool ContainsCaseInsensitive(this string source, string toCheck)
        {
            return source.IndexOf(toCheck,  StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
