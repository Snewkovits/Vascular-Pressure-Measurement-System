using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Vascular_Pressure_Measurement_System.Forms;
using Vascular_Pressure_Measurement_System.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Vascular_Pressure_Measurement_System
{
    public partial class Main : Form
    {
        SettingsForm settingsForm = null;
        Measure measure = null;
        int counter = 0;
        // Chart variables
        System.Windows.Forms.Timer chartUpdateTimer;

        public Main()
        {
            InitializeComponent();
            Connection.ContinousTest();
            GlobalData.SerialConnectionStatusChanged += GlobalData_SerialConnectionStatus;
            measure = new Measure(this);
            chartUpdateTimer = new System.Windows.Forms.Timer();

            DisableButton(ForceStopButton);
            DisableButton(StartMeasuring);
            DisableButton(SaveButton);
        }

        #region UI Manipulations
        public void EnableButton(Button button)
        {
            button.Enabled = true;
            button.BackColor = Color.WhiteSmoke;
        }

        public void DisableButton(Button button)
        {
            button.Enabled = false;
            button.BackColor = Color.Gray;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Chart.Top = 10;
            Chart.Size = new Size(this.ClientSize.Width - Chart.Left - 10, this.ClientSize.Height - 20);
            RefreshButtonPosition();

            Chart.Series[0].ChartType = SeriesChartType.FastLine;
            Chart.ChartAreas[0].AxisX.Minimum = double.NaN;
            Chart.ChartAreas[0].AxisX.Maximum = double.NaN;
            DisableButton(ForceStopButton);
            DisableButton(StartMeasuring);
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            Chart.Size = new System.Drawing.Size(this.ClientSize.Width - Chart.Left - 10, this.ClientSize.Height - 20);
            RefreshButtonPosition();
        }

        private void RefreshButtonPosition()
        {
            int heights = AboutButton.ClientSize.Height + 10;
            AboutButton.Top = ClientSize.Height - heights;
            heights += SettingsButton.ClientSize.Height + 10;
            SettingsButton.Top = ClientSize.Height - heights;
            heights += LoadButton.ClientSize.Height + 20;
            LoadButton.Top = ClientSize.Height - heights;
            heights += SaveButton.ClientSize.Height + 10;
            SaveButton.Top = ClientSize.Height - heights;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e) // Amikor a form bezáródik, leállítjuk a mérést, hogy biztosan ne maradjon futó szál, ami a serial portot használja, és ne hagyjuk nyitva a kapcsolatot a készülékkel.
        {
            Connection.stopConnection = true;
            measure.Stop(true);
        }

        #endregion UI Manipulations

        private void GlobalData_SerialConnectionStatus(object sender, EventArgs e) // Amikor a kapcsolat állapota megváltozik, frissítjük a form címét és a gombok állapotát ennek megfelelően. Ha a kapcsolat létrejött, engedélyezzük a StartMeasuring gombot, ha pedig megszakadt, letiltjuk a gombokat és figyelmeztető üzenetet jelenítünk meg.
        {
            if (Connection.isConnected)
            {
                Invoke(new Action(() =>
                {
                    this.Text = "VRP - Device connected";
                    EnableButton(StartMeasuring);
                }));
            }
            else
            {
                Invoke(new Action(() =>
                {
                    this.Text = "VRP - Device disconnected";
                    DisableButton(ForceStopButton);
                    DisableButton(StartMeasuring);
                    if (settingsForm != null)
                    {
                        settingsForm.Close();
                        settingsForm = null;
                        this.Enabled = true;
                    }
                }));
                new Thread(() => MessageBox.Show("No connection to the device!", "CONNECTION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
            }
        }

        #region Chart update
        // A Chart folyamatos frissítése a Measure osztály bufferéből, amíg az mérés fut. A frissítés 10 ms-onként történik, és csak akkor, ha van új adat a bufferben.
        private void ContinousChartUpdate(object sender, EventArgs e)
        {
            if (!measure.isRunning()) // Ha a mérés nem fut, leállítjuk a timer-t, hogy ne próbáljon frissíteni a bufferből, és ne okozzon hibát.
            {
                chartUpdateTimer.Stop();
                return;
            }


            lock (measure._lock) // A buffer elérését lock-oljuk, hogy elkerüljük a párhuzamos hozzáférésből adódó problémákat, mivel a Measure osztály egy külön szálon írja a bufferbe az adatokat.
            {
                while (measure.Buffer.Count > 0)
                {
                    string data = measure.Buffer.Dequeue().ToString(); // Kivesszük a bufferből az első elemet, és stringgé alakítjuk, mivel a Measure osztály stringként tárolja az adatokat.
                    if (double.TryParse(data, out double result)) // Megpróbáljuk double-á alakítani a stringet, hogy numerikus értékként tudjuk megjeleníteni a Chart-on. Ha sikerül, hozzáadjuk a Chart-hoz egy új pontként, ahol az X érték a counter * 0.01 (ami a mért időt jelenti másodpercben), és az Y érték pedig a result (a mért nyomás értéke).
                    {
                        Chart.Series[0].Points.AddXY(counter * 0.01, result);
                        counter++;
                    }
                }
            }

        }
        #endregion

        #region Buttons

        private void StartMeasuring_Click(object sender, EventArgs e) // Amikor a StartMeasuring gombra kattintunk, elindítjuk a mérést, és beállítjuk a Chart-ot, hogy megjelenítse a mért adatokat. Először is, töröljük a Chart-on lévő pontokat, hogy tiszta legyen a megjelenítés. Ezután engedélyezzük a ForceStopButton-t, hogy lehetőséget adjunk a felhasználónak a mérés leállítására, és letiltjuk a StartMeasuring gombot, hogy ne lehessen újra elindítani a mérést, amíg az már fut. Végül beállítjuk a chartUpdateTimer-t, hogy 10 ms-onként frissítse a Chart-ot a Measure osztály bufferéből származó adatokkal, és elindítjuk a timer-t.
        {
            if (measure.isRunning()) return;

            Chart.Series[0].Points.Clear();

            EnableButton(ForceStopButton);
            DisableButton(StartMeasuring);
            DisableButton(SaveButton);
            DisableButton(SettingsButton);

            chartUpdateTimer.Interval = 10;
            chartUpdateTimer.Tick += ContinousChartUpdate;
            chartUpdateTimer.Start();

            measure.Start();
        }

        private void SettingsButton_Click(object sender, EventArgs e) // Amikor a SettingsButton gombra kattintunk, megnyitjuk a SettingsForm-ot, ahol a felhasználó beállíthatja a mérés paramétereit, például a mintavételi frekvenciát vagy a kommunikációs portot. Amíg a SettingsForm nyitva van, letiltjuk a Main formot, hogy ne lehessen interakcióba lépni vele, amíg a beállítások módosítása folyamatban van. Amikor a SettingsForm bezáródik, visszaállítjuk a Main formot, hogy újra használható legyen.
        {
            settingsForm = new SettingsForm();
            this.Enabled = false;

            settingsForm.FormClosed += (s, args) =>
            {
                settingsForm = null;
                this.Enabled = true;
            };

            settingsForm.Show();
        }

        private void ForceStopButton_Click(object sender, EventArgs e) // Amikor a Force Stop gombra kattintunk, leállítjuk a mérést, hogy azonnal megálljon a mérés, függetlenül attól, hogy éppen milyen állapotban van a kapcsolat a készülékkel. Ez hasznos lehet, ha valami probléma adódik a mérés során, és gyorsan meg kell állítani a folyamatot.
        {
            measure.Stop();
            DisableButton(ForceStopButton);
            EnableButton(StartMeasuring);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string data = "timestamp;measured\n";
            foreach (DataPoint point in Chart.Series[0].Points)
            {
                data += $"{point.XValue};{point.YValues[0]}\n";
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Save CSV file";
                dialog.Filter = "CSV file (*.csv)|*.csv";
                dialog.DefaultExt = "csv";
                dialog.AddExtension = true;

                DateTime dateTime = DateTime.Now;

                dialog.FileName = $"{dateTime:yyMMdd}.csv";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;

                    File.WriteAllText(path, data);
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string[] data = null;
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Load CSV file",
                Filter = "CSV file (*.csv)|*.csv",
                DefaultExt = "csv",
                AddExtension = true
            };

            DialogResult loadCsvResult = dialog.ShowDialog();


            if (loadCsvResult == DialogResult.OK)
            {
                string path = dialog.FileName;

                data = File.ReadAllLines(path);
            }
            else if (loadCsvResult == DialogResult.Cancel)
            {
                return;
            }

            if (data == null || data.Length < 2)
            {
                MessageBox.Show("An error occurred during selection.", "Load CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DisableButton(SaveButton);

            Chart.Series[0].Points.Clear();
            for (int i = 1; i < data.Length; i++)
            {
                string[] line = data[i].Split(';');
                if (line.Length < 2) continue;

                if (double.TryParse(line[0], out double x) && double.TryParse(line[1], out double y))
                {
                    Chart.Series[0].Points.AddXY(x, y);
                }
            }
        }

        #endregion Buttons

        private void Chart_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                double selectionStart = Chart.ChartAreas[0].CursorX.SelectionStart;
                double selectionEnd = Chart.ChartAreas[0].CursorX.SelectionEnd;
                if (selectionStart == selectionEnd) return;

                double minX = Math.Min(selectionStart, selectionEnd);
                double maxX = Math.Max(selectionStart, selectionEnd);

                var allPoints = Chart.Series[0].Points;
                List<DataPoint> selectedPoints = new List<DataPoint>();

                foreach (var point in allPoints)
                {
                    if (point.XValue >= minX && point.XValue <= maxX)
                        selectedPoints.Add(point);
                }

                if (selectedPoints.Count < 2) return;

                List<double> derivatives = new List<double>();
                List<double> derivativeX = new List<double>();

                for (int i = 0; i < selectedPoints.Count - 1; i++)
                {
                    double x1 = selectedPoints[i].XValue;
                    double y1 = selectedPoints[i].YValues[0];
                    double x2 = selectedPoints[i + 1].XValue;
                    double y2 = selectedPoints[i + 1].YValues[0];

                    double dx = x2 - x1;
                    if (dx != 0)
                    {
                        derivatives.Add((y2 - y1) / dx);
                        derivativeX.Add((x1 + x2) / 2);
                    }
                }

                AnalysisForm analysisForm = new AnalysisForm(selectedPoints, derivativeX, derivatives);

                analysisForm.Show();
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }
    }
}
