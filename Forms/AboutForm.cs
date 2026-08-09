using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            
            KeyPreview = true;

            string assamblyPath = Assembly.GetExecutingAssembly().Location;

            if (string.IsNullOrEmpty(assamblyPath))
            {
                assamblyPath = AppContext.BaseDirectory;
            }

            DateTime lastModified = File.GetLastWriteTime(assamblyPath);

            version.Text = $"v1.{lastModified:yyMMdd}";
            releaseDate.Text = $"{lastModified.Year}. {lastModified.Month:00}. {lastModified.Day:00}";
        }

        private bool isCtrlPressed = false;
        private void About_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = true;

            if (e.KeyCode.Equals(Keys.Escape))
                this.Close();
        }

        private void About_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.ControlKey))
                isCtrlPressed = false;
        }
    }
}
