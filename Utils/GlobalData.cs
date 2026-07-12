using System;
using System.Drawing;
using System.IO;

namespace Vascular_Pressure_Measurement_System.Utils
{
    public static class GlobalData
    {
        public static Color UDGreen = Color.FromArgb(20, 68, 56);
        public static Color UDYellow = Color.FromArgb(245, 180, 24);

        private static bool _serialConnectionStatus;
        public static event EventHandler SerialConnectionStatusChanged;

        public static bool SerialConnectionStatus
        {
            get => _serialConnectionStatus;
            set
            {
                if (_serialConnectionStatus != value)
                {
                    _serialConnectionStatus = value;
                    SerialConnectionStatusChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static void SetParameters(int minDelta = default, int fallTreshold = default)
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localPath, "Vascular_Pressure_Measurement_System");

            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);

            string configurationFile = Path.Combine(appFolder, "config.csv");

            // Ha default értékekkel hívták meg (pl. az indításkor), megpróbáljuk beolvasni a fájlból
            if (minDelta == default && fallTreshold == default)
            {
                try
                {
                    if (File.Exists(configurationFile))
                    {
                        string[] lines = File.ReadAllLines(configurationFile);
                        if (lines.Length >= 2)
                        {
                            string[] a = lines[0].Split(';');
                            string[] b = lines[1].Split(';');
                            if (a.Length >= 2 && a[0] == "MIN_DELTA") minDelta = int.Parse(a[1]);
                            if (b.Length >= 2 && b[0] == "FALL_TRESHOLD") fallTreshold = int.Parse(b[1]);
                        }
                    }
                    else
                    {
                        // Ha nincs fájl, a gyári biztonsági értékeket mentjük el
                        minDelta = 2;
                        fallTreshold = 3;
                        string initial = $"MIN_DELTA;{minDelta}\nFALL_TRESHOLD;{fallTreshold}";
                        File.WriteAllText(configurationFile, initial);
                    }
                }
                catch
                {
                    minDelta = 2;
                    fallTreshold = 3;
                }
            }
            else
            {
                // Ha konkrét paraméterekkel hívták meg a függvényt (módosítás történt), elmentjük a fájlba
                try
                {
                    string initial = $"MIN_DELTA;{minDelta}\nFALL_TRESHOLD;{fallTreshold}";
                    File.WriteAllText(configurationFile, initial);
                }
                catch { }
            }

            // Csak akkor küldünk, ha a port ténylegesen nyitva van
            if (Connection.serialPort != null && Connection.serialPort.IsOpen)
            {
                Connection.SendMessage(Connection.CommandType.SET_PARAM, $"MD;{minDelta}");
                System.Threading.Thread.Sleep(50); // Egérút az Arduinónak
                Connection.SendMessage(Connection.CommandType.SET_PARAM, $"FT;{fallTreshold}");
            }
        }
    }
}