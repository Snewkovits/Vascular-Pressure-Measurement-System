namespace Vascular_Pressure_Measurement_System.Forms
{
    partial class TestPad
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mov0OpenSignal = new System.Windows.Forms.CheckBox();
            this.mov1OpenSignal = new System.Windows.Forms.CheckBox();
            this.mov2OpenSignal = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.mov2CloseSignal = new System.Windows.Forms.CheckBox();
            this.mov1CloseSignal = new System.Windows.Forms.CheckBox();
            this.mov0CloseSignal = new System.Windows.Forms.CheckBox();
            this.mov0OpenCMD = new System.Windows.Forms.Button();
            this.mov0CloseCMD = new System.Windows.Forms.Button();
            this.mov1CloseCMD = new System.Windows.Forms.Button();
            this.mov1OpenCMD = new System.Windows.Forms.Button();
            this.mov2CloseCMD = new System.Windows.Forms.Button();
            this.mov2OpenCMD = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "MOV0 gömbcsap";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "MOV1 gömbcsap";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "MOV2 gömbcsap";
            // 
            // mov0OpenSignal
            // 
            this.mov0OpenSignal.AutoCheck = false;
            this.mov0OpenSignal.AutoSize = true;
            this.mov0OpenSignal.Location = new System.Drawing.Point(136, 35);
            this.mov0OpenSignal.Name = "mov0OpenSignal";
            this.mov0OpenSignal.Size = new System.Drawing.Size(15, 14);
            this.mov0OpenSignal.TabIndex = 3;
            this.mov0OpenSignal.UseVisualStyleBackColor = true;
            // 
            // mov1OpenSignal
            // 
            this.mov1OpenSignal.AutoCheck = false;
            this.mov1OpenSignal.AutoSize = true;
            this.mov1OpenSignal.Location = new System.Drawing.Point(136, 60);
            this.mov1OpenSignal.Name = "mov1OpenSignal";
            this.mov1OpenSignal.Size = new System.Drawing.Size(15, 14);
            this.mov1OpenSignal.TabIndex = 4;
            this.mov1OpenSignal.UseVisualStyleBackColor = true;
            // 
            // mov2OpenSignal
            // 
            this.mov2OpenSignal.AutoCheck = false;
            this.mov2OpenSignal.AutoSize = true;
            this.mov2OpenSignal.Location = new System.Drawing.Point(136, 85);
            this.mov2OpenSignal.Name = "mov2OpenSignal";
            this.mov2OpenSignal.Size = new System.Drawing.Size(15, 14);
            this.mov2OpenSignal.TabIndex = 5;
            this.mov2OpenSignal.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(122, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Nyitva";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("DIN Pro Regular", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(167, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Zárva";
            // 
            // mov2CloseSignal
            // 
            this.mov2CloseSignal.AutoCheck = false;
            this.mov2CloseSignal.AutoSize = true;
            this.mov2CloseSignal.Location = new System.Drawing.Point(177, 84);
            this.mov2CloseSignal.Name = "mov2CloseSignal";
            this.mov2CloseSignal.Size = new System.Drawing.Size(15, 14);
            this.mov2CloseSignal.TabIndex = 10;
            this.mov2CloseSignal.UseVisualStyleBackColor = true;
            // 
            // mov1CloseSignal
            // 
            this.mov1CloseSignal.AutoCheck = false;
            this.mov1CloseSignal.AutoSize = true;
            this.mov1CloseSignal.Location = new System.Drawing.Point(177, 59);
            this.mov1CloseSignal.Name = "mov1CloseSignal";
            this.mov1CloseSignal.Size = new System.Drawing.Size(15, 14);
            this.mov1CloseSignal.TabIndex = 9;
            this.mov1CloseSignal.UseVisualStyleBackColor = true;
            // 
            // mov0CloseSignal
            // 
            this.mov0CloseSignal.AutoCheck = false;
            this.mov0CloseSignal.AutoSize = true;
            this.mov0CloseSignal.Location = new System.Drawing.Point(177, 34);
            this.mov0CloseSignal.Name = "mov0CloseSignal";
            this.mov0CloseSignal.Size = new System.Drawing.Size(15, 14);
            this.mov0CloseSignal.TabIndex = 8;
            this.mov0CloseSignal.UseVisualStyleBackColor = true;
            // 
            // mov0OpenCMD
            // 
            this.mov0OpenCMD.Location = new System.Drawing.Point(235, 30);
            this.mov0OpenCMD.Name = "mov0OpenCMD";
            this.mov0OpenCMD.Size = new System.Drawing.Size(75, 23);
            this.mov0OpenCMD.TabIndex = 14;
            this.mov0OpenCMD.Text = "Nyitás";
            this.mov0OpenCMD.UseVisualStyleBackColor = true;
            this.mov0OpenCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // mov0CloseCMD
            // 
            this.mov0CloseCMD.Location = new System.Drawing.Point(316, 30);
            this.mov0CloseCMD.Name = "mov0CloseCMD";
            this.mov0CloseCMD.Size = new System.Drawing.Size(75, 23);
            this.mov0CloseCMD.TabIndex = 15;
            this.mov0CloseCMD.Text = "Zárás";
            this.mov0CloseCMD.UseVisualStyleBackColor = true;
            this.mov0CloseCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // mov1CloseCMD
            // 
            this.mov1CloseCMD.Location = new System.Drawing.Point(316, 55);
            this.mov1CloseCMD.Name = "mov1CloseCMD";
            this.mov1CloseCMD.Size = new System.Drawing.Size(75, 23);
            this.mov1CloseCMD.TabIndex = 17;
            this.mov1CloseCMD.Text = "Zárás";
            this.mov1CloseCMD.UseVisualStyleBackColor = true;
            this.mov1CloseCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // mov1OpenCMD
            // 
            this.mov1OpenCMD.Location = new System.Drawing.Point(235, 55);
            this.mov1OpenCMD.Name = "mov1OpenCMD";
            this.mov1OpenCMD.Size = new System.Drawing.Size(75, 23);
            this.mov1OpenCMD.TabIndex = 16;
            this.mov1OpenCMD.Text = "Nyitás";
            this.mov1OpenCMD.UseVisualStyleBackColor = true;
            this.mov1OpenCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // mov2CloseCMD
            // 
            this.mov2CloseCMD.Location = new System.Drawing.Point(316, 80);
            this.mov2CloseCMD.Name = "mov2CloseCMD";
            this.mov2CloseCMD.Size = new System.Drawing.Size(75, 23);
            this.mov2CloseCMD.TabIndex = 19;
            this.mov2CloseCMD.Text = "Zárás";
            this.mov2CloseCMD.UseVisualStyleBackColor = true;
            this.mov2CloseCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // mov2OpenCMD
            // 
            this.mov2OpenCMD.Location = new System.Drawing.Point(235, 80);
            this.mov2OpenCMD.Name = "mov2OpenCMD";
            this.mov2OpenCMD.Size = new System.Drawing.Size(75, 23);
            this.mov2OpenCMD.TabIndex = 18;
            this.mov2OpenCMD.Text = "Nyitás";
            this.mov2OpenCMD.UseVisualStyleBackColor = true;
            this.mov2OpenCMD.Click += new System.EventHandler(this.TestPad_ButtonClicked);
            // 
            // TestPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 149);
            this.Controls.Add(this.mov2CloseCMD);
            this.Controls.Add(this.mov2OpenCMD);
            this.Controls.Add(this.mov1CloseCMD);
            this.Controls.Add(this.mov1OpenCMD);
            this.Controls.Add(this.mov0CloseCMD);
            this.Controls.Add(this.mov0OpenCMD);
            this.Controls.Add(this.mov2CloseSignal);
            this.Controls.Add(this.mov1CloseSignal);
            this.Controls.Add(this.mov0CloseSignal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mov2OpenSignal);
            this.Controls.Add(this.mov1OpenSignal);
            this.Controls.Add(this.mov0OpenSignal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("DIN Pro Regular", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "TestPad";
            this.Text = "TestPad";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TestPad_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mov0OpenSignal;
        private System.Windows.Forms.CheckBox mov1OpenSignal;
        private System.Windows.Forms.CheckBox mov2OpenSignal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox mov2CloseSignal;
        private System.Windows.Forms.CheckBox mov1CloseSignal;
        private System.Windows.Forms.CheckBox mov0CloseSignal;
        private System.Windows.Forms.Button mov0OpenCMD;
        private System.Windows.Forms.Button mov0CloseCMD;
        private System.Windows.Forms.Button mov1CloseCMD;
        private System.Windows.Forms.Button mov1OpenCMD;
        private System.Windows.Forms.Button mov2CloseCMD;
        private System.Windows.Forms.Button mov2OpenCMD;
    }
}