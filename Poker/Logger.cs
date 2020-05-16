using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitare
{
    public class Logger
    {
        private static object fileLock = new object();
        private string logFilePath;

        public Logger(string logPath)
        {
            logFilePath = logPath;
            File.Create(logPath);
        }

        public void Log(string message)
        {
            lock (fileLock)
            {
                try
                {
                    using (var appender = File.AppendText(logFilePath))
                    {
                        appender.WriteLine(message.Trim());
                    }
                }
                catch
                { }
            }
        }
    }
}
