namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class ValveControl
    {
        public static bool OpenValve(int valveId)
        {
            if (!Connection.isConnected) return false;
            Connection.serialPort.ReadTimeout = 5000;
            Connection.serialPort.WriteTimeout = 5000;

            if (Connection.SendMessage(Connection.CommandType.OPEN_VALVE, valveId.ToString())[0] == "ERR")
            {
                Connection.serialPort.ReadTimeout = 100;
                Connection.serialPort.WriteTimeout = 100;
                return false;
            }
            Connection.serialPort.ReadTimeout = 100;
            Connection.serialPort.WriteTimeout = 100;
            return true;
        }

        public static bool CloseValvePosition(int valveId, int position)
        {
            if (!Connection.isConnected) return false;
            Connection.serialPort.ReadTimeout = 5000;
            Connection.serialPort.WriteTimeout = 5000;
            if (Connection.SendMessage(Connection.CommandType.CLOSE_VALVE_TO, $"{valveId.ToString()};{position.ToString()}")
                [0] == "ERR")
            {
                Connection.serialPort.ReadTimeout = 100;
                Connection.serialPort.WriteTimeout = 100;
                return false;
            }
            Connection.serialPort.ReadTimeout = 100;
            Connection.serialPort.WriteTimeout = 100;
            return true;
        }

        public static bool CloseValve(int valveId)
        {
            if (!Connection.isConnected) return false;
            Connection.serialPort.ReadTimeout = 5000;
            Connection.serialPort.WriteTimeout = 5000;
            if (Connection.SendMessage(Connection.CommandType.CLOSE_VALVE, valveId.ToString())[0] == "ERR")
            {
                Connection.serialPort.ReadTimeout = 100;
                Connection.serialPort.WriteTimeout = 100;
                return false;
            }
            Connection.serialPort.ReadTimeout = 100;
            Connection.serialPort.WriteTimeout = 100;
            return true;
        }
    }
}
