namespace Vascular_Pressure_Measurement_System.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.SaveButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ReinitButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.parameterBox = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.parameterValve2OpenPosition = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.parameterValve3CloseDelay = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.parameterPumpOnTime = new System.Windows.Forms.TextBox();
            this.parameterPumpOffTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.parameterFallTreshold = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.parameterMinDelta = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EnableTraceButton = new System.Windows.Forms.CheckBox();
            this.parameterBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(14, 479);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(88, 27);
            this.SaveButton.TabIndex = 2;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 361);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Recalibration";
            // 
            // ReinitButton
            // 
            this.ReinitButton.ForeColor = System.Drawing.Color.Black;
            this.ReinitButton.Location = new System.Drawing.Point(163, 355);
            this.ReinitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ReinitButton.Name = "ReinitButton";
            this.ReinitButton.Size = new System.Drawing.Size(214, 27);
            this.ReinitButton.TabIndex = 4;
            this.ReinitButton.Text = "Start";
            this.ReinitButton.UseVisualStyleBackColor = true;
            this.ReinitButton.Click += new System.EventHandler(this.ReinitButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(108, 479);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(88, 27);
            this.CancelButton.TabIndex = 5;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // parameterBox
            // 
            this.parameterBox.Controls.Add(this.label13);
            this.parameterBox.Controls.Add(this.label12);
            this.parameterBox.Controls.Add(this.label11);
            this.parameterBox.Controls.Add(this.label10);
            this.parameterBox.Controls.Add(this.label9);
            this.parameterBox.Controls.Add(this.label7);
            this.parameterBox.Controls.Add(this.parameterValve2OpenPosition);
            this.parameterBox.Controls.Add(this.label8);
            this.parameterBox.Controls.Add(this.parameterValve3CloseDelay);
            this.parameterBox.Controls.Add(this.label6);
            this.parameterBox.Controls.Add(this.parameterPumpOnTime);
            this.parameterBox.Controls.Add(this.parameterPumpOffTime);
            this.parameterBox.Controls.Add(this.label5);
            this.parameterBox.Controls.Add(this.label1);
            this.parameterBox.Controls.Add(this.parameterFallTreshold);
            this.parameterBox.Controls.Add(this.label4);
            this.parameterBox.Controls.Add(this.parameterMinDelta);
            this.parameterBox.Controls.Add(this.label3);
            this.parameterBox.Controls.Add(this.ReinitButton);
            this.parameterBox.Controls.Add(this.label2);
            this.parameterBox.ForeColor = System.Drawing.Color.White;
            this.parameterBox.Location = new System.Drawing.Point(12, 85);
            this.parameterBox.Name = "parameterBox";
            this.parameterBox.Size = new System.Drawing.Size(383, 388);
            this.parameterBox.TabIndex = 6;
            this.parameterBox.TabStop = false;
            this.parameterBox.Text = "Hardware parameters";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(349, 31);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(26, 15);
            this.label13.TabIndex = 23;
            this.label13.Text = "pcs";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(349, 59);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 15);
            this.label12.TabIndex = 22;
            this.label12.Text = "pcs";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(349, 87);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 15);
            this.label11.TabIndex = 21;
            this.label11.Text = "ms";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(349, 119);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 15);
            this.label10.TabIndex = 20;
            this.label10.Text = "ms";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(349, 147);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "%";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(349, 171);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "ms";
            // 
            // parameterValve2OpenPosition
            // 
            this.parameterValve2OpenPosition.Location = new System.Drawing.Point(163, 140);
            this.parameterValve2OpenPosition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterValve2OpenPosition.Name = "parameterValve2OpenPosition";
            this.parameterValve2OpenPosition.Size = new System.Drawing.Size(178, 22);
            this.parameterValve2OpenPosition.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 143);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "Valve 2 open position";
            // 
            // parameterValve3CloseDelay
            // 
            this.parameterValve3CloseDelay.Location = new System.Drawing.Point(163, 168);
            this.parameterValve3CloseDelay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterValve3CloseDelay.Name = "parameterValve3CloseDelay";
            this.parameterValve3CloseDelay.Size = new System.Drawing.Size(178, 22);
            this.parameterValve3CloseDelay.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(7, 171);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Flushing time";
            // 
            // parameterPumpOnTime
            // 
            this.parameterPumpOnTime.Location = new System.Drawing.Point(163, 112);
            this.parameterPumpOnTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterPumpOnTime.Name = "parameterPumpOnTime";
            this.parameterPumpOnTime.Size = new System.Drawing.Size(178, 22);
            this.parameterPumpOnTime.TabIndex = 12;
            // 
            // parameterPumpOffTime
            // 
            this.parameterPumpOffTime.Location = new System.Drawing.Point(163, 84);
            this.parameterPumpOffTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterPumpOffTime.Name = "parameterPumpOffTime";
            this.parameterPumpOffTime.Size = new System.Drawing.Size(178, 22);
            this.parameterPumpOffTime.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 115);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Pump on time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 87);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Pump off time";
            // 
            // parameterFallTreshold
            // 
            this.parameterFallTreshold.Location = new System.Drawing.Point(163, 56);
            this.parameterFallTreshold.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterFallTreshold.Name = "parameterFallTreshold";
            this.parameterFallTreshold.Size = new System.Drawing.Size(178, 22);
            this.parameterFallTreshold.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Falling Treshold";
            // 
            // parameterMinDelta
            // 
            this.parameterMinDelta.Location = new System.Drawing.Point(163, 28);
            this.parameterMinDelta.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterMinDelta.Name = "parameterMinDelta";
            this.parameterMinDelta.Size = new System.Drawing.Size(178, 22);
            this.parameterMinDelta.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 31);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Fall Delta";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EnableTraceButton);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(383, 67);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Software parameters";
            // 
            // EnableTraceButton
            // 
            this.EnableTraceButton.AutoSize = true;
            this.EnableTraceButton.Location = new System.Drawing.Point(11, 32);
            this.EnableTraceButton.Name = "EnableTraceButton";
            this.EnableTraceButton.Size = new System.Drawing.Size(95, 19);
            this.EnableTraceButton.TabIndex = 10;
            this.EnableTraceButton.Text = "Enable Trace";
            this.EnableTraceButton.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(68)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(407, 519);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.parameterBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "VRP - Settings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Settings_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Settings_KeyUp);
            this.parameterBox.ResumeLayout(false);
            this.parameterBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ReinitButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.GroupBox parameterBox;
        private System.Windows.Forms.TextBox parameterFallTreshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox parameterMinDelta;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox EnableTraceButton;
        private System.Windows.Forms.TextBox parameterPumpOnTime;
        private System.Windows.Forms.TextBox parameterPumpOffTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox parameterValve3CloseDelay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox parameterValve2OpenPosition;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
    }
}