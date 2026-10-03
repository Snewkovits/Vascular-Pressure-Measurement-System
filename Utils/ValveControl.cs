using System.Threading;

namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class ValveControl
    {
        public static bool OpenValve(int valveId)
        {
            if (!Connection.isConnected) return false;
            Connection.isBusy = true;

            int errCount = 0;

            Connection.SendMessage(Connection.CommandType.OPEN_VALVE, valveId.ToString());

            while (Connection.ReadMessage()[0] == "ERR")
            {
                errCount++;
                if (errCount > 100)
                {
                    Connection.isBusy = false;
                    return false;
                }
            }

            Connection.isBusy = false;
            return true;
        }

        public static bool CloseValvePosition(int valveId, int position)
        {
            if (!Connection.isConnected) return false;
            Connection.isBusy = true;

            int errCount = 0;

            Connection.SendMessage(Connection.CommandType.CLOSE_VALVE_TO, valveId.ToString() + "," + position.ToString());

            while (Connection.ReadMessage()[0] == "ERR")
            {
                errCount++;
                if (errCount > 100)
                {
                    Connection.isBusy = false;
                    return false;
                }
            }

            Connection.isBusy = false;
            return true;
        }

        public static bool CloseValve(int valveId)
        {
            if (!Connection.isConnected) return false;
            Connection.isBusy = true;

            int errCount = 0;

            Connection.SendMessage(Connection.CommandType.CLOSE_VALVE, valveId.ToString());

            while (Connection.ReadMessage()[0] == "ERR")
            {
                errCount++;
                if (errCount > 100)
                {
                    Connection.isBusy = false;
                    return false;
                }
            }

            Connection.isBusy = false;
            return true;
        }
    }
}
