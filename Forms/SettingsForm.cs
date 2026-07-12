using System;
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
                string[] minDelta = Connection.SendMessage("GET_PARAM", "MD");
                string[] fallTreshold = Connection.SendMessage("GET_PARAM", "FT");

                // válaszok validálása
                if (minDelta == null || fallTreshold == null
                    || minDelta.Length < 2 || fallTreshold.Length < 2)
                {
                    MessageBox.Show("Invalid parameter response from device.", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseForm();
                    return;
                }

                if (minDelta[0] != Connection.CommandType.ACK || fallTreshold[0] != Connection.CommandType.ACK)
                {
                    MessageBox.Show("Failed to retrieve parameters! Please try again later.", "Parameters", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseForm();
                    return;
                }

                parameterMinDelta.Text = minDelta[1];
                parameterFallTreshold.Text = fallTreshold[1];

                prev_minDelta = int.TryParse(parameterMinDelta.Text, out var pd) ? pd : -1;
                prev_fallTreshold = int.TryParse(parameterFallTreshold.Text, out var pf) ? pf : -1;
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

            if (parameterBox.Enabled) 
                GlobalData.SetParameters(minDelta, fallTreshold);

            CloseForm();
        }

        private void CloseForm()
        {
            this.Close();
        }
    }
}
