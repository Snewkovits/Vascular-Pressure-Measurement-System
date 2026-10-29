using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Vascular_Pressure_Measurement_System.Utils;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class SettingsForm : Form
    {

        public SettingsForm()
        {
            InitializeComponent();

            KeyPreview = true;

            if (Connection.isConnected)
            {
                Dictionary<string, string> configs = Configuration.Hardware.ReadConfiguration();

                parameterMinDelta.Text = configs["MIN_DELTA"];
                parameterFallTreshold.Text = configs["FALL_THRESHOLD"];
                parameterPumpOffTime.Text = configs["PUMP_OFF_TIME"];
                parameterPumpOnTime.Text = configs["PUMP_ON_TIME"];
                parameterValve2ClosePosition.Text = configs["VALVE2_CLOSE_POSITION"];
                parameterValve3CloseDelay.Text = configs["VALVE3_CLOSE_DELAY_MS"];
            }
            else
            {
                parameterBox.Enabled = false;
            }

            EnableTraceButton.Checked = Trace.IsTraceEnabled;
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult userResponse = MessageBox.Show("Changes will not be saved!", "Parameters", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (userResponse.Equals(DialogResult.OK))
            {
                CloseForm();
            }
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            int minDelta = -1, fallTreshold = -1;

            int.TryParse(parameterMinDelta.Text, out minDelta);
            int.TryParse(parameterFallTreshold.Text, out fallTreshold);
            int.TryParse(parameterPumpOffTime.Text, out int pumpOffTime);
            int.TryParse(parameterPumpOnTime.Text, out int pumpOnTime);
            int.TryParse(parameterValve2ClosePosition.Text, out int valve2ClosePosition);
            int.TryParse(parameterValve3CloseDelay.Text, out int valve3CloseDelay);

            if ((minDelta <= 0          || 
                fallTreshold <= 0       || 
                pumpOffTime <= 0        || 
                pumpOnTime <= 0         || 
                valve2ClosePosition < 0 || 
                valve3CloseDelay <= 0)  && 
                parameterBox.Enabled)
            {
                MessageBox.Show("Parameters must be positive! Please check your input and try again.", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (valve2ClosePosition < 0 || valve2ClosePosition > 100)
            {
                MessageBox.Show("Valve 2 close position must be between 0 and 100! Please check your input and try again.", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<string, string> configs = new Dictionary<string, string>
            {
                { "MIN_DELTA", minDelta.ToString() },
                { "FALL_THRESHOLD", fallTreshold.ToString() },
                { "PUMP_OFF_TIME", pumpOffTime.ToString() },
                { "PUMP_ON_TIME", pumpOnTime.ToString() },
                { "VALVE2_CLOSE_POSITION", valve2ClosePosition.ToString() },
                { "VALVE3_CLOSE_DELAY_MS", valve3CloseDelay.ToString() }
            };

            if (parameterBox.Enabled) 
                Configuration.Hardware.SetParameters(configs);

            Trace.IsTraceEnabled = EnableTraceButton.Checked;

            CloseForm();
        }

        private void CloseForm()
        {
            this.Close();
        }

        private bool isCtrlPressed = false;
        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = true;

            if (e.KeyCode.Equals(Keys.Escape))
                this.Close();
        }

        private void Settings_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = false;
        }

        private void ReinitButton_Click(object sender, EventArgs e)
        {
            Connection.SendMessage(Connection.CommandType.RECALIBRATE, "");
        }
    }
}
