using System;

namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class Trace
    {
        public static void WriteTrace(string message)
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = System.IO.Path.Combine(localPath, "Vascular_Pressure_Measurement_System");
            string traceFile = System.IO.Path.Combine(appFolder, "trace.log");
            if (!System.IO.Directory.Exists(appFolder))
            {
                System.IO.Directory.CreateDirectory(appFolder);
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(traceFile, true))
            {
                sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
    }
}
