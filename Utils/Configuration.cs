using System;
using System.Collections.Generic;
using System.IO;

namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class Configuration
    {
        static string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string appFolder = Path.Combine(localPath, "Vascular_Pressure_Measurement_System");
        static string configureFile = Path.Combine(appFolder, "hardware_configuration.csv");

        public static void SetParameters(Dictionary<string, string> configs = null, bool writeDefault = false)
        {
            if (configs == null)
                configs = new Dictionary<string, string>();

            if (!Directory.Exists(appFolder) || !File.Exists(configureFile) || writeDefault)
            {
                Directory.CreateDirectory(appFolder);
                configs["MIN_DELTA"] = "2";
                configs["FALL_THRESHOLD"] = "3";
                WriteConfiguration(configs, WriteType.Both);
                return;
            }

            if (configs.Count == 0)
            {
                WriteConfiguration(ReadConfiguration(), WriteType.Device);
                return;
            }

            WriteConfiguration(configs, WriteType.Both);
        }

        public static void WriteConfiguration(Dictionary<string, string> configs, string writeType)
        {
            string current = string.Empty;
            string overall = string.Empty;

            foreach (KeyValuePair<string, string> config in configs)
            {
                current = $"{config.Key};{config.Value}";
                overall += current + "\n";

                if (writeType.Contains(WriteType.Device))
                {
                    Connection.SendMessage(Connection.CommandType.SET_PARAM, current);
                }
            }

            if (writeType.Contains(WriteType.File) && configs.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter(configureFile))
                {
                    sw.Write(overall);
                }
            }
        }

        public static Dictionary<string, string> ReadConfiguration()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!File.Exists(configureFile))
            {
                return result;
            }

            using (StreamReader sr = new StreamReader(configureFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] data = line.Split(';');

                    if (data.Length >= 2)
                    {
                        string key = data[0].Trim();
                        string value = data[1].Trim();

                        result[key] = value;
                    }
                }

                double minDeltaVal;
                double fallThresholdVal;

                if (result.Count <= 0 ||
                    !result.TryGetValue("MIN_DELTA", out string minDeltaStr) ||
                    !double.TryParse(minDeltaStr, out minDeltaVal) || minDeltaVal < 0 ||
                    !result.TryGetValue("FALL_THRESHOLD", out string fallThresholdStr) ||
                    !double.TryParse(fallThresholdStr, out fallThresholdVal) || fallThresholdVal < 0)
                {
                    SetParameters(null, true);
                    result = ReadConfiguration();
                }
            }

            return result;
        }

        static class WriteType
        {
            public const string Device = "DEVICE";
            public const string File = "FILE";
            public const string Both = Device + File;
        }
    }
}