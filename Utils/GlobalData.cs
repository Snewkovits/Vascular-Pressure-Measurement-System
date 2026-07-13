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
    }
}