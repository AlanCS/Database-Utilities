using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DatabaseUtilities.Core
{
    public partial class LogManagement : IDisposable, ILogManagement
    {
        System.Timers.Timer timerWriter = null;
        Dictionary<string, string> ValuesToWrite = null;
        private string folder = Path.Combine(System.Environment.CurrentDirectory, "logs");

        public LogManagement()
        {
            timerWriter = new System.Timers.Timer(3 * 1000); // 3 seconds
            timerWriter.Elapsed += new System.Timers.ElapsedEventHandler(timerWriter_Elapsed);

            ValuesToWrite = new Dictionary<string, string>();

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        void timerWriter_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerWriter.Stop();

            lock (this)
            {
                foreach (var valueToWrite in ValuesToWrite)
                    File.WriteAllText(String.Format(@"{0}\{1}.txt", folder, valueToWrite.Key), valueToWrite.Value);

                ValuesToWrite.Clear();
            }
        }

        public string Read(string key)
        {
            var file = String.Format(@"{0}\{1}.txt", folder, key);
            if (File.Exists(file))
                return File.ReadAllText(file);
            else
                return string.Empty;
        }

        public void Write(string key, string content)
        {
            timerWriter.Stop();

            ValuesToWrite[key] = content;

            timerWriter.Start();
        }

        public void Dispose()
        {
            timerWriter.Stop();
            timerWriter.Dispose();

            ValuesToWrite = null;
        }
    }
}
