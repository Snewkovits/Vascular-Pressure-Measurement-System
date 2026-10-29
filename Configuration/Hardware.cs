using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Vascular_Pressure_Measurement_System.Utils;

namespace Vascular_Pressure_Measurement_System.Configuration
{
    internal class Hardware
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
                configs["PUMP_OFF_TIME"] = "800";
                configs["PUMP_ON_TIME"] = "200";
                configs["VALVE2_CLOSE_POSITION"] = "0";
                configs["VALVE3_CLOSE_DELAY_MS"] = "15000";
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
                try
                {
                    using (StreamWriter sw = new StreamWriter(configureFile))
                    {
                        sw.Write(overall);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static Dictionary<string, string> ReadConfiguration()
        {
            Dictionary<string, string> defaults =
                new Dictionary<string, string>
                {
                    { "MIN_DELTA", "2" },
                    { "FALL_THRESHOLD", "3" },
                    { "PUMP_OFF_TIME", "800" },
                    { "PUMP_ON_TIME", "200" },
                    { "VALVE2_CLOSE_POSITION", "0" },
                    { "VALVE3_CLOSE_DELAY_MS", "15000" }
                };

            // Ha még nincs konfigurációs fájl, létrehozzuk.
            if (!File.Exists(configureFile))
            {
                Directory.CreateDirectory(appFolder);
                WriteConfiguration(defaults, WriteType.File);
                return defaults;
            }

            Dictionary<string, string> result =
                new Dictionary<string, string>();

            // 1. Beolvasás, majd a fájl lezárása.
            using (StreamReader sr = new StreamReader(configureFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] data = line.Split(';');

                    if (data.Length >= 2)
                        result[data[0].Trim()] = data[1].Trim();
                }
            }

            // 2. Ellenőrzés a fájl lezárása után.
            bool valid =
                result.TryGetValue("MIN_DELTA", out string minDelta) &&
                double.TryParse(minDelta,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double minDeltaValue) &&
                minDeltaValue >= 0 &&

                result.TryGetValue("FALL_THRESHOLD", out string fallThreshold) &&
                double.TryParse(fallThreshold,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double fallThresholdValue) &&
                fallThresholdValue >= 0 &&

                result.TryGetValue("PUMP_OFF_TIME", out string pumpOffTime) &&
                double.TryParse(pumpOffTime,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double pumpOffTimeValue) &&
                pumpOffTimeValue >= 0 &&

                result.TryGetValue("PUMP_ON_TIME", out string pumpOnTime) &&
                double.TryParse(pumpOnTime,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double pumpOnTimeValue) &&
                pumpOnTimeValue >= 0 &&

                result.TryGetValue("VALVE2_CLOSE_POSITION", out string valve2ClosePosition) &&
                double.TryParse(valve2ClosePosition,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double valve2ClosePositionValue) &&

                result.TryGetValue("VALVE3_CLOSE_DELAY_MS", out string valve3CloseDelay) &&
                double.TryParse(valve3CloseDelay,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double valve3CloseDelayValue) &&
                valve3CloseDelayValue >= 0;

            if (!valid)
            {
                // Csak a fájlt javítjuk, az eszközzel itt nem kommunikálunk.
                WriteConfiguration(defaults, WriteType.File);
                return defaults;
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