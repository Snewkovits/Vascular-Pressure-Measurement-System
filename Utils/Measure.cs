using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Vascular_Pressure_Measurement_System.Utils
{
    internal class Measure
    {
        bool stopMeasure = false;
        bool running = false;

        public object _lock;
        public Queue Buffer;

        Form mainForm;
        bool isFormClosing = false;

        public Measure(Form mainForm) { // A konstruktorban inicializáljuk a Buffer-t és a lock objektumot, hogy később használhassuk őket a szálbiztos műveletekhez.
            Buffer = new Queue();
            _lock = new object();
            this.mainForm = mainForm;
        }

        public bool isRunning() { return running; }

        public void Stop(bool isFormClosing = false) // A Stop metódusban beállítjuk a stopMeasure változót true-ra, ami jelzi a mérő szálnak, hogy le kell állnia. Ez egy egyszerű és hatékony módja annak, hogy jelezzük a szálnak, hogy befejezze a munkáját, anélkül, hogy erőszakosan megszakítanánk azt.
        {
            this.isFormClosing = isFormClosing;
            stopMeasure = true;
        }

        public void Start()
        {
            if (!Connection.isConnected) return;

            running = true;
            stopMeasure = false; // Biztonsági reset indításkor
            int counter = 0;
            
            bool hardwareStop = false; // Jelzi, hogy a STOP_MEASURE parancsot megkaptuk az Arduinótól
            TimeSpan afterStop = TimeSpan.FromSeconds(20); // A mérés leállítása után mennyi ideig várjon a szál, mielőtt teljesen leállna.
            TimeSpan cureent = TimeSpan.Zero;

            // Elküldjük a start parancsot. (A SendMessage le-lockol, elküldi, majd feloldja)
            string[] data = Connection.SendMessage("START_MEASURE", "");

            if (data.Length < 2 || data[0] != "ACK")
            {
                running = false;
                return; // Ha nem jött ACK a startra, el se indítjuk a szálat
            }

            Configuration.Hardware.SetParameters(); // A mérés megkezdése előtt beállítjuk a paramétereket az Arduino-n.

            new Thread(() =>
            {
                // Itt lefoglaljuk a portot a teljes mérés idejére!
                // A PING szál (ContinousTest) addig ide be se tud lépni, amíg ebből a lockból ki nem jövünk.
                lock (Connection._serialPortLock)
                {
                    while (!stopMeasure && Connection.isConnected)
                    {
                        data = Connection.SendMessage(Connection.CommandType.GET_MEASURE_DATA, "");

                        // Ha hiba történt a beolvasásnál (pl. timeout), ne tegyük a queue-ba
                        if (data[0] == "ERR")
                        {
                            Connection.faildAttempt++;
                            if (Connection.faildAttempt == 3)
                            {
                                Connection.stopConnection = true;
                                break;
                            }
                        }


                        // Ha az Arduino magától állt le
                        if (data[0] == "STOPPED")
                        {
                            hardwareStop = true;
                        }

                        if (hardwareStop)
                        {
                            cureent += TimeSpan.FromMilliseconds(1);
                            if (cureent >= afterStop)
                            {
                                stopMeasure = true;
                            }
                        }

                        // Ha normál mérési adat jött
                        if (data[0] == "MEASURE_DATA")
                        {
                            string measurementData = data[1].Split(';')[1];
                            lock (_lock)
                            {
                                Buffer.Enqueue(measurementData);
                            }
                            counter++;
                        }

                        Thread.Sleep(1);
                    }

                    // Ha a PC oldali gombbal állítottuk le a mérést, le kell lőni az Arduinót is
                    if (stopMeasure && Connection.isConnected)
                    {
                        // Mivel már benne vagyunk a lock-ban, a SendMessage-en belüli lock 
                        // gond nélkül lefut (re-entrant), nem fogja magát blokkolni.
                        Connection.SendMessage("STOP_MEASURE", "");
                    }
                }

                if (!isFormClosing)
                {
                    mainForm.Invoke(new Action(() =>
                    {
                        mainForm.Controls["ForceStopButton"].Enabled = false;
                        mainForm.Controls["ForceStopButton"].BackColor = Color.Gray;

                        mainForm.Controls["StartMeasuring"].Enabled = true;
                        mainForm.Controls["StartMeasuring"].BackColor = Color.WhiteSmoke;
                        mainForm.Controls["SaveButton"].Enabled = true;
                        mainForm.Controls["SaveButton"].BackColor = Color.WhiteSmoke;
                        mainForm.Controls["SettingsButton"].Enabled = true;
                        mainForm.Controls["SettingsButton"].BackColor = Color.WhiteSmoke;
                    }));
                }

                running = false;
                stopMeasure = false;
            }).Start();
        }
    }
}
