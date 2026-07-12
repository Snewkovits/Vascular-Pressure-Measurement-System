using System;
using System.Linq;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vascular_Pressure_Measurement_System.Forms
{
    public partial class AnalysisForm : Form
    {
        public AnalysisForm(List<DataPoint> originalPoints, List<double> derivativeX, List<double> derivatives)
        {
            InitializeComponent();

            // Form elrendezés beállítása
            chartDerivative.Left = 200;
            chartDerivative.Top = 0;
            chartDerivative.Width = this.ClientSize.Width - 200;
            chartDerivative.Height = this.ClientSize.Height;

            // Grafikon tengelyek automatikus, de szoros skálázása a szebb nézetért
            chartDerivative.ChartAreas[0].AxisY.IsStartedFromZero = false;

            // Eredeti (kék) pontok kirajzolása
            chartDerivative.Series["Original"].Points.Clear();
            foreach (var p in originalPoints)
            {
                chartDerivative.Series["Original"].Points.AddXY(p.XValue, p.YValues[0]);
            }

            // Derivált (sárga) pontok kirajzolása
            chartDerivative.Series["Derivated"].Points.Clear();
            for (int i = 0; i < derivatives.Count; i++)
            {
                chartDerivative.Series["Derivated"].Points.AddXY(derivativeX[i], derivatives[i]);
            }

            // Statisztikák kiszámítása és kiírása
            if (derivatives != null && derivatives.Count > 0)
            {
                double avgDerivative = derivatives.Average();

                double maxSlope = derivatives.Max();
                int maxIndex = derivatives.IndexOf(maxSlope);
                double maxLocation = derivativeX[maxIndex];

                double minSlope = derivatives.Min();
                int minIndex = derivatives.IndexOf(minSlope);
                double minLocation = derivativeX[minIndex];

                // Feliratok frissítése (Formázva 4 tizedesjegyre, X pozíció 2 tizedesjegyre)
                dxAvg.Text = $"{avgDerivative:F4}";
                dxRise.Text = $"{maxSlope:F4} (at X = {maxLocation:F2})";
                dxFall.Text = $"{minSlope:F4} (at X = {minLocation:F2})";
                dxEvaluated.Text = $"{derivatives.Count}";

                // Ha a Label neve még a régi, a UI-on írjuk át "Average Derivative"-re!
            }
        }

        private void Derivate_SizeChanged(object sender, EventArgs e)
        {
            chartDerivative.Left = 200;
            chartDerivative.Top = 0;
            chartDerivative.Width = this.ClientSize.Width - 200;
            chartDerivative.Height = this.ClientSize.Height;
        }
    }
}