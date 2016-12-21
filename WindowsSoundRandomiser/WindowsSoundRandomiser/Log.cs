using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsSoundRandomiser
{
    class Log
    {
        private Log() { }

        public static void WriteToLog(string entry)
        {
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

            StreamWriter stream = File.AppendText(filepath);

            stream.WriteLine("");
            stream.Write(DateTime.Now.ToString() + ": ");
            stream.Write(entry);

            stream.Close();
        }
    }
}
