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
            this.parameterFallTreshold = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.parameterMinDelta = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.parameterBox.SuspendLayout();
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
            this.label2.Location = new System.Drawing.Point(7, 215);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Restart initialization";
            // 
            // ReinitButton
            // 
            this.ReinitButton.Location = new System.Drawing.Point(162, 209);
            this.ReinitButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ReinitButton.Name = "ReinitButton";
            this.ReinitButton.Size = new System.Drawing.Size(214, 27);
            this.ReinitButton.TabIndex = 4;
            this.ReinitButton.Text = "Start";
            this.ReinitButton.UseVisualStyleBackColor = true;
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
            this.parameterBox.Controls.Add(this.parameterFallTreshold);
            this.parameterBox.Controls.Add(this.label4);
            this.parameterBox.Controls.Add(this.parameterMinDelta);
            this.parameterBox.Controls.Add(this.label3);
            this.parameterBox.Controls.Add(this.ReinitButton);
            this.parameterBox.Controls.Add(this.label2);
            this.parameterBox.Location = new System.Drawing.Point(12, 231);
            this.parameterBox.Name = "parameterBox";
            this.parameterBox.Size = new System.Drawing.Size(383, 242);
            this.parameterBox.TabIndex = 6;
            this.parameterBox.TabStop = false;
            this.parameterBox.Text = "Hardware parameters";
            // 
            // parameterFallTreshold
            // 
            this.parameterFallTreshold.Location = new System.Drawing.Point(163, 56);
            this.parameterFallTreshold.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterFallTreshold.Name = "parameterFallTreshold";
            this.parameterFallTreshold.Size = new System.Drawing.Size(213, 22);
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
            this.parameterMinDelta.Size = new System.Drawing.Size(213, 22);
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
            // SettingsForm
            // 
            this.AcceptButton = this.SaveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(407, 519);
            this.ControlBox = false;
            this.Controls.Add(this.parameterBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "VRP - Settings";
            this.parameterBox.ResumeLayout(false);
            this.parameterBox.PerformLayout();
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
    }
}