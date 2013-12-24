using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DatabaseUtilities.DAL
{
    public static class Logger
    {
        private static string folder = Path.Combine(System.Environment.CurrentDirectory, "log");

        public static void Log(Exception ex)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var text = Environment.NewLine + "--- " + DateTime.Now.ToString("dd/MM/yy HH:mm") + Environment.NewLine + ex.ToString();

            var fileName = String.Format(@"{0}\{1}.txt", folder, DateTime.Now.ToString("yyyy-MM-dd"));
            try
            {                
                File.AppendAllText(fileName, text);
            }
            catch (Exception logEx)
            {
                // nothing can be done if we can't even log the previous error
            }
        }
    }
}
