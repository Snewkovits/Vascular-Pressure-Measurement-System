using System;
using System.Windows.Forms;

using Vascular_Pressure_Measurement_System.Utils;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class TestPad : Form
    {

        public TestPad()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        private void TestPad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void TestPad_ButtonClicked(object sender, EventArgs e)
        {
            TurnToLow();
            Button currentButton = (Button)sender;
            
            switch (currentButton.Name)
            {
                case "mov0OpenCMD":
                    Connection.SendMessage("SET_IO", "D0;HIGH");
                    break;
                case "mov1OpenCMD":
                    Connection.SendMessage("SET_IO", "D1;HIGH");
                    break;
                case "mov2OpenCMD":
                    Connection.SendMessage("SET_IO", "D2;HIGH");
                    break;

                case "mov0CloseCMD":
                    Connection.SendMessage("SET_IO", "D3;HIGH");
                    break;
                case "mov1CloseCMD":
                    Connection.SendMessage("SET_IO", "D4;HIGH");
                    break;
                case "mov2CloseCMD":
                    Connection.SendMessage("SET_IO", "D5;HIGH");
                    break;
            }
        }

        private void TurnToLow()
        {
            Connection.SendMessage("SET_IO", "D0;LOW");
            Connection.SendMessage("SET_IO", "D1;LOW");
            Connection.SendMessage("SET_IO", "D2;LOW");
            Connection.SendMessage("SET_IO", "D3;LOW");
            Connection.SendMessage("SET_IO", "D4;LOW");
            Connection.SendMessage("SET_IO", "D5;LOW");
        }
    }
}
