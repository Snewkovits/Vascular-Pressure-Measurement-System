namespace Vascular_Pressure_Measurement_System
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.StartMeasuring = new System.Windows.Forms.Button();
            this.ForceStopButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.AboutButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.SuspendLayout();
            // 
            // Chart
            // 
            this.Chart.BackColor = System.Drawing.Color.Transparent;
            this.Chart.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.ScaleView.Zoomable = false;
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisX2.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.ScaleView.Zoomable = false;
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY2.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            this.Chart.Location = new System.Drawing.Point(117, 0);
            this.Chart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Chart.Name = "Chart";
            this.Chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.Chart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(180)))), ((int)(((byte)(24))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(68)))))};
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "Series2";
            series1.YValuesPerPoint = 2;
            this.Chart.Series.Add(series1);
            this.Chart.Size = new System.Drawing.Size(803, 505);
            this.Chart.TabIndex = 6;
            title1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title1.DockingOffset = 2;
            title1.Font = new System.Drawing.Font("DIN Pro Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            title1.ForeColor = System.Drawing.Color.White;
            title1.Name = "Pressure";
            title1.Text = "Pressure (hgmm)";
            title2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title2.Font = new System.Drawing.Font("DIN Pro Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            title2.ForeColor = System.Drawing.Color.White;
            title2.Name = "Time";
            title2.Text = "Time (s)";
            this.Chart.Titles.Add(title1);
            this.Chart.Titles.Add(title2);
            this.Chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseUp);
            // 
            // StartMeasuring
            // 
            this.StartMeasuring.BackColor = System.Drawing.Color.WhiteSmoke;
            this.StartMeasuring.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.StartMeasuring.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartMeasuring.Location = new System.Drawing.Point(2, 12);
            this.StartMeasuring.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StartMeasuring.Name = "StartMeasuring";
            this.StartMeasuring.Size = new System.Drawing.Size(107, 27);
            this.StartMeasuring.TabIndex = 5;
            this.StartMeasuring.Text = "Start measuring";
            this.StartMeasuring.UseVisualStyleBackColor = false;
            this.StartMeasuring.Click += new System.EventHandler(this.StartMeasuring_Click);
            // 
            // ForceStopButton
            // 
            this.ForceStopButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ForceStopButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ForceStopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ForceStopButton.Location = new System.Drawing.Point(2, 45);
            this.ForceStopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ForceStopButton.Name = "ForceStopButton";
            this.ForceStopButton.Size = new System.Drawing.Size(107, 27);
            this.ForceStopButton.TabIndex = 4;
            this.ForceStopButton.Text = "Force stop";
            this.ForceStopButton.UseVisualStyleBackColor = false;
            this.ForceStopButton.Click += new System.EventHandler(this.ForceStopButton_Click);
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SettingsButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.SettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsButton.Location = new System.Drawing.Point(2, 456);
            this.SettingsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(107, 27);
            this.SettingsButton.TabIndex = 3;
            this.SettingsButton.Text = "Settings";
            this.SettingsButton.UseVisualStyleBackColor = false;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SaveButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Location = new System.Drawing.Point(2, 370);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(107, 27);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadButton
            // 
            this.LoadButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LoadButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.LoadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadButton.Location = new System.Drawing.Point(2, 403);
            this.LoadButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(107, 27);
            this.LoadButton.TabIndex = 0;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = false;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // AboutButton
            // 
            this.AboutButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.AboutButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.AboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AboutButton.Location = new System.Drawing.Point(2, 489);
            this.AboutButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.Size = new System.Drawing.Size(107, 27);
            this.AboutButton.TabIndex = 2;
            this.AboutButton.Text = "About";
            this.AboutButton.UseVisualStyleBackColor = false;
            this.AboutButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AboutButton);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.ForceStopButton);
            this.Controls.Add(this.StartMeasuring);
            this.Controls.Add(this.Chart);
            this.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Main";
            this.Text = "VRP - Disconnected";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.Button StartMeasuring;
        private System.Windows.Forms.Button ForceStopButton;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button AboutButton;
    }
}

