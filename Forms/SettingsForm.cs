using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Vascular_Pressure_Measurement_System.Utils;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class SettingsForm : Form
    {

        int prev_minDelta, prev_fallTreshold;

        public SettingsForm()
        {
            InitializeComponent();

            if (Connection.isConnected)
            {
                Dictionary<string, string> configs = Configuration.ReadConfiguration();

                parameterMinDelta.Text = configs["MIN_DELTA"];
                parameterFallTreshold.Text = configs["FALL_THRESHOLD"];

            }
            else
            {
                parameterBox.Enabled = false;
                prev_minDelta = int.TryParse(parameterMinDelta.Text, out var pd) ? pd : -1;
                prev_fallTreshold = int.TryParse(parameterFallTreshold.Text, out var pf) ? pf : -1;
            }
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

            if ((minDelta <= 0 || fallTreshold <= 0) && parameterBox.Enabled)
            {
                MessageBox.Show("Parameters must be positive! Please check your input and try again.", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<string, string> configs = new Dictionary<string, string>
            {
                { "MIN_DELTA", minDelta.ToString() },
                { "FALL_THRESHOLD", fallTreshold.ToString() }
            };

            if (parameterBox.Enabled) 
                Configuration.SetParameters(configs);

            CloseForm();
        }

        private void CloseForm()
        {
            this.Close();
        }
    }
}
