using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Vascular_Pressure_Measurement_System.Utils
{
    public static class Connection
    {
        public static bool isConnected = false;
        public static bool stopConnection = false;
        public static SerialPort serialPort = null;

        private static int msgId = 0;
        public static int faildAttempt = 0;

        internal static readonly object _serialPortLock = new object();

        public static SerialPort GetSerialPort() // Ez a metódus végigmegy az összes elérhető soros porton, megpróbálja megnyitni őket, és elküldi a transmissionMessage-t. Ha a válasz megegyezik a receiveMessage-el, akkor visszaadja a megnyitott SerialPort objektumot, különben bezárja a portot és folytatja a keresést. Ha egyik port sem válaszol megfelelően, akkor null-t ad vissza.
        {
            string[] ports = SerialPort.GetPortNames();
            serialPort = null;

            foreach (string port in ports) {
                serialPort = new SerialPort(port, 1000000, Parity.None, 8, StopBits.One) 
                {
                    Encoding = Encoding.ASCII,
                    ReadTimeout = 100,
                    WriteTimeout = 100,
                    ReadBufferSize = 65536,
                    WriteBufferSize = 65536
                };

                try
                {
                    serialPort.Open();
                }
                catch (Exception)
                {
                    continue;
                }

                try
                {
                    string response = SendMessage("PING", "")[0];
                    if (response == "PONG")
                    {
                        return serialPort;
                    }
                }
                catch (TimeoutException)
                {
                    // Nincs válasz, próbáljuk a következő portot
                }
                finally
                {
                    serialPort.Close();
                }
            }

            return null;
        }

        public static void ContinousTest()
        {
            new Thread(() =>
            {
                serialPort = null;
                while (!stopConnection)
                {
                    // Lockolunk, hogy amíg a kapcsolatot ellenőrizzük/javítjuk, 
                    // más szál ne küldhessen adatot a SendMessage-en keresztül
                    lock (_serialPortLock)
                    {
                        if (serialPort == null || !serialPort.IsOpen)
                        {
                            serialPort = GetSerialPort();
                            try
                            {
                                Configuration.SetParameters();
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteTrace(ex.Message);
                            }

                            if (serialPort != null)
                            {
                                try
                                {
                                    serialPort.Open();
                                    isConnected = true;
                                    GlobalData.SerialConnectionStatus = true;
                                }
                                catch
                                {
                                    isConnected = false;
                                    GlobalData.SerialConnectionStatus = false;
                                }
                            }
                            else
                            {
                                isConnected = false;
                                GlobalData.SerialConnectionStatus = false;
                            }
                        }
                        else
                        {
                            try
                            {
                                if (SendMessage(CommandType.PING, "")[0] == CommandType.PONG)
                                {
                                    isConnected = true;
                                    GlobalData.SerialConnectionStatus = true;
                                }
                                else
                                {
                                    CloseConnection();
                                }
                            }
                            catch (Exception)
                            {
                                CloseConnection();
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            }).Start();
        }

        // Egy kis segédfüggvény, hogy ne kelljen háromszor leírni a bezárást
        private static void CloseConnection()
        {
            isConnected = false;
            GlobalData.SerialConnectionStatus = false;
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        static byte CalculateChecksum(string message)
        {
            byte checksum = 0;
            foreach (char c in message)
            {
                checksum ^= (byte)c;
            }
            return checksum;
        }

        // Ez a metódus egy üzenetet küld a soros porton a megadott cmd és data paraméterekkel, majd várja a választ. Az üzenet formátuma: <ID|CMD|DATA|CHK>, ahol ID egy növekvő számláló, CMD a parancs, DATA a parancs adatai, és CHK a payload (ID|CMD|DATA) XOR értéke. A válasz formátuma: <ID|ACK|DATA|CHK>, ahol ID megegyezik a küldött üzenet ID-jével, ACK egy állandó string, DATA a válasz adatai, és CHK a payload (ID|ACK|DATA) XOR értéke. Ha bármilyen hiba történik (pl. nincs kapcsolat, nem megfelelő válasz), akkor egy "ERR" és "0" értékeket tartalmazó string tömböt ad vissza.
        public static string[] SendMessage(string cmd, string data) // ID|CMD|DATA|CHK
        {
            if (serialPort == null || !serialPort.IsOpen)
                return new string[] { CommandType.ERR, "Device is not connected" };

            lock (_serialPortLock)
            {
                string id = (msgId++).ToString();
                string payload = $"{id}|{cmd}|{data}";
                byte chk = CalculateChecksum(payload);

                string msg = $"<{payload}|{chk:X2}>";

                // Puffer ürítés és küldés már a zároláson belül
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.Write(msg);

                string response = string.Empty;

                try
                {
                    response = serialPort.ReadTo(">") + ">";
                }
                catch
                {
                    HandleFailure();
                    return new string[] { CommandType.ERR, "" };
                }

                response = response.Trim('<', '>');
                string[] responsePayload = response.Split('|');

                if (responsePayload.Length != 4)
                {
                    HandleFailure();
                    return new string[] { CommandType.ERR, "" };
                }
                if (responsePayload[0] != id)
                {
                    return new string[] { CommandType.ERR, "" };
                }
                if (CalculateChecksum($"{responsePayload[0]}|{responsePayload[1]}|{responsePayload[2]}") != Convert.ToByte(responsePayload[3], 16))
                {
                    return new string[] { CommandType.ERR, "" };
                }

                // Sikeres kommunikáció nullázzuk
                faildAttempt = 0;

                return new string[] { responsePayload[1], responsePayload[2] };
            }
        }

        // Segédfüggvény a hiba kezelésére
        private static void HandleFailure()
        {
            faildAttempt++;
            if (faildAttempt >= 3)
            {
                serialPort.Close();
                isConnected = false;
                GlobalData.SerialConnectionStatus = false;
                faildAttempt = 0;
            }
        }

        // Ez a metódus csak olvas egy üzenetet a soros porton, és visszaadja a parancsot és az adatot.
        public static string[] ReadMessage()
        {
            string response = string.Empty;

            try
            {
                response = serialPort.ReadTo(">") + ">";
            }
            catch
            {
                faildAttempt++;
                if (faildAttempt >= 3)
                {
                    serialPort.Close();
                    isConnected = false;
                    GlobalData.SerialConnectionStatus = false;
                    faildAttempt = 0;
                }
                return new string[] { CommandType.ERR, "" };
            }

            response = response.Trim('<', '>');
            string[] responsePayload = response.Split('|');


            if (responsePayload.Length != 4)
            {
                faildAttempt++;
                if (faildAttempt >= 3)
                {
                    serialPort.Close();
                    isConnected = false;
                    GlobalData.SerialConnectionStatus = false;
                    faildAttempt = 0;
                }
                return new string[] { CommandType.ERR, "" };
            }
            if (CalculateChecksum($"{responsePayload[0]}|{responsePayload[1]}|{responsePayload[2]}") != Convert.ToByte(responsePayload[3], 16)) return new string[] { CommandType.ERR, "" };
            return new string[] { responsePayload[1], responsePayload[2] };
        }

        public static class CommandType
        {
            public const string PING = "PING";
            public const string PONG = "PONG";
            public const string SET_PARAM = "SET_PARAM";
            public const string GET_PARAM = "GET_PARAM";
            public const string START_MEASURE = "START_MEASURE";
            public const string STOP_MEASURE = "STOP_MEASURE";
            public const string GET_MEASURE_DATA = "GET_MEASURE_DATA";
            public const string ACK = "ACK";
            public const string ERR = "ERR";
        }
    }
}
