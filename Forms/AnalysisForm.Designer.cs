namespace Vascular_Pressure_Measurement_System.Forms
{
    partial class AnalysisForm
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisForm));
            this.chartDerivative = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dxAvg = new System.Windows.Forms.TextBox();
            this.dxRise = new System.Windows.Forms.TextBox();
            this.dxFall = new System.Windows.Forms.TextBox();
            this.dxEvaluated = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartDerivative)).BeginInit();
            this.SuspendLayout();
            // 
            // chartDerivative
            // 
            chartArea1.CursorX.Interval = 0D;
            chartArea1.CursorX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chartArea1.Name = "ChartArea1";
            this.chartDerivative.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartDerivative.Legends.Add(legend1);
            this.chartDerivative.Location = new System.Drawing.Point(200, 15);
            this.chartDerivative.Margin = new System.Windows.Forms.Padding(4);
            this.chartDerivative.Name = "chartDerivative";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Derivated";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Original";
            this.chartDerivative.Series.Add(series1);
            this.chartDerivative.Series.Add(series2);
            this.chartDerivative.Size = new System.Drawing.Size(350, 369);
            this.chartDerivative.TabIndex = 0;
            this.chartDerivative.Text = "chart1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("DIN Pro Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Selection statistics";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Avarage Derivative";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 92);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Max Rise";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 138);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Max Fall";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(15, 184);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Evaluated Segments";
            // 
            // dxAvg
            // 
            this.dxAvg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.dxAvg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dxAvg.ForeColor = System.Drawing.Color.White;
            this.dxAvg.Location = new System.Drawing.Point(18, 65);
            this.dxAvg.Name = "dxAvg";
            this.dxAvg.ReadOnly = true;
            this.dxAvg.Size = new System.Drawing.Size(175, 17);
            this.dxAvg.TabIndex = 6;
            // 
            // dxRise
            // 
            this.dxRise.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.dxRise.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dxRise.ForeColor = System.Drawing.Color.White;
            this.dxRise.Location = new System.Drawing.Point(18, 111);
            this.dxRise.Name = "dxRise";
            this.dxRise.ReadOnly = true;
            this.dxRise.Size = new System.Drawing.Size(175, 17);
            this.dxRise.TabIndex = 7;
            // 
            // dxFall
            // 
            this.dxFall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.dxFall.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dxFall.ForeColor = System.Drawing.Color.White;
            this.dxFall.Location = new System.Drawing.Point(18, 157);
            this.dxFall.Name = "dxFall";
            this.dxFall.ReadOnly = true;
            this.dxFall.Size = new System.Drawing.Size(175, 17);
            this.dxFall.TabIndex = 8;
            // 
            // dxEvaluated
            // 
            this.dxEvaluated.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.dxEvaluated.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dxEvaluated.ForeColor = System.Drawing.Color.White;
            this.dxEvaluated.Location = new System.Drawing.Point(18, 203);
            this.dxEvaluated.Name = "dxEvaluated";
            this.dxEvaluated.ReadOnly = true;
            this.dxEvaluated.Size = new System.Drawing.Size(175, 17);
            this.dxEvaluated.TabIndex = 9;
            // 
            // AnalysisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(927, 554);
            this.Controls.Add(this.dxEvaluated);
            this.Controls.Add(this.dxFall);
            this.Controls.Add(this.dxRise);
            this.Controls.Add(this.dxAvg);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chartDerivative);
            this.Font = new System.Drawing.Font("DIN Pro Regular", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AnalysisForm";
            this.Text = "VRP - Analysis";
            this.SizeChanged += new System.EventHandler(this.Derivate_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chartDerivative)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartDerivative;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox dxAvg;
        private System.Windows.Forms.TextBox dxRise;
        private System.Windows.Forms.TextBox dxFall;
        private System.Windows.Forms.TextBox dxEvaluated;
    }
}