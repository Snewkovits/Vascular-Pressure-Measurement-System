using System;

namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class Trace
    {
        public static bool IsTraceEnabled { get; set; } = false;

        public static string appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Vascular_Pressure_Measurement_System");

        public static void WriteTrace(string message)
        {
            if (!IsTraceEnabled) return;

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

        public static void WriteSerialTrace(string message)
        {
            if (!IsTraceEnabled) return;

            string traceFile = System.IO.Path.Combine(appFolder, "serial_trace.log");
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
