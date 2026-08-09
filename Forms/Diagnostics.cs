using Vascular_Pressure_Measurement_System.Utils;
using System.Windows.Forms;
using System.Threading;
using System;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class Diagnostics : Form
    {
        Main mainForm = null;

        string BOARD_TYPE = string.Empty;
        int DIGITAL_PINS = 0;
        int ANALOG_PINS = 0;

        public bool closeThreads = false;

        public Diagnostics(Main mainForm)
        {
            InitializeComponent();

            this.mainForm = mainForm;

            KeyPreview = true;
        }

        private void Diagnostics_Load(object sender, EventArgs e)
        {
            if (Connection.isConnected)
                BoardInitialize();
            GlobalData.SerialConnectionStatusChanged += ConnectionChanged;
        }

        private void ConnectionChanged(object sender, EventArgs e)
        {
            if (Connection.isConnected)
                closeThreads = false;
            else
                closeThreads = true;

            BoardInitialize();
        }

        private void BoardInitialize()
        {
            string[] boardDatas = Connection.SendMessage(Connection.CommandType.GET_BOARD_DATAS, "")[1].Split(';');

            if (!IsHandleCreated || IsDisposed)
                return;


            Invoke(new Action(() =>
            {
                if (!closeThreads)
                {

                    BOARD_TYPE = boardDatas[0];
                    Text = $"{BOARD_TYPE} connected";
                    int.TryParse(boardDatas[1], out DIGITAL_PINS);
                    int.TryParse(boardDatas[2], out ANALOG_PINS);
                    GenerateMembers();
                }
                else
                {
                    Text = "Disconnected";
                    BOARD_TYPE = string.Empty;
                    DIGITAL_PINS = 0;
                    ANALOG_PINS = 0;
                    DeleteAllMembers();
                }
            }));
        }

        private void Diagnostics_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeThreads = true;

            mainForm.testPad = null;
        }

        private void GenerateMembers()
        {
            closeThreads = false;
            int labelTop = 6;
            int textTop = 10;
            int gap = 28;
            for (int i = 0; i < ANALOG_PINS; i++)
            {
                Label label = new Label()
                {
                    Text = $"Analog IN {i}",
                    Top = textTop,
                    Left = 9
                };
                TextBox textBox = new TextBox()
                {
                    Top = labelTop,
                    Left = 209,
                    Name = "A" + i
                };
                this.Controls.Add(textBox);
                this.Controls.Add(label);

                labelTop += gap;
                textTop += gap;
            }

            for (int i = 0; i < DIGITAL_PINS; i++)
            {
                Label label = new Label()
                {
                    Text = "???",
                    Top = textTop,
                    Name = "D" + i + "L",
                    Left = 9,
                    Width = 200
                };

                Button button = new Button()
                {
                    Top = labelTop,
                    Left = 209,
                    Name = "D" + i,
                    Text = ""
                };

                button.Click += DigitalButtonClicked;

                this.Controls.Add(button);
                this.Controls.Add(label);

                labelTop += gap;
                textTop += gap;
            }

            new Thread(() =>
            {
                Invoke(new Action(() =>
                {
                    for (int i = 0; i < DIGITAL_PINS; i++)
                    {
                        foreach (Control control in Controls)
                        {
                            if (control.Name == $"D{i}L")
                            {
                                string pinMode = Connection.SendMessage(Connection.CommandType.GET_PIN_MODE, $"D{i}")[1];
                                control.Text = $"Digital {pinMode} {i}";
                            }
                        }
                    }
                }));
            }).Start();
            RefreshMembers();
        }

        private void RefreshMembers()
        {
            new Thread(() =>
            {
                while (Connection.isConnected && !closeThreads)
                {
                    try
                    {
                        string data = string.Empty;
                        for (int i = 0; i < ANALOG_PINS; i++)
                        {
                            data = Connection.SendMessage(Connection.CommandType.GET_IO, "A" + i)[1];
                            Invoke(new Action(() =>
                            {
                                foreach (Control control in Controls)
                                {
                                    if (control.Name == "A" + i)
                                        control.Text = data;
                                }
                            }));
                        }
                        for (int i = 0; i < DIGITAL_PINS; i++)
                        {
                            data = Connection.SendMessage(Connection.CommandType.GET_IO, "D" + i)[1];
                            Invoke(new Action(() =>
                            {
                                foreach (Control control in Controls)
                                {
                                    if (control.Name == "D" + i)
                                        control.Text = data;
                                }
                            }));
                        }
                    }
                    catch { }
                    Thread.Sleep(10);
                }
            }).Start();
        }

        private void DeleteAllMembers()
        {
            closeThreads = true;
            Controls.Clear();
        }

        private void DigitalButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string pinName = button.Name;
            string pinMode = Connection.SendMessage(Connection.CommandType.GET_PIN_MODE, pinName)[1];
            if (pinMode == "OUTPUT")
            {
                string currentValue = Connection.SendMessage(Connection.CommandType.GET_IO, pinName)[1];
                string newValue = currentValue == "1" ? "LOW" : "HIGH";
                Connection.SendMessage(Connection.CommandType.SET_IO, $"{pinName};{newValue}");
            }
        }

        private bool isCtrlPressed = false;
        private void Diagnostics_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = true;

            if (e.KeyCode.Equals(Keys.Escape))
                this.Close();
        }

        private void Diagnostics_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = false;
        }
    }
}
