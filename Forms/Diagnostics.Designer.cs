namespace Vascular_Pressure_Measurement_System.Forms
{
    partial class Diagnostics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Diagnostics));
            this.SuspendLayout();
            // 
            // Diagnostics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(651, 450);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Diagnostics";
            this.Text = "Disconnected";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Diagnostics_FormClosing);
            this.Load += new System.EventHandler(this.Diagnostics_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Diagnostics_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Diagnostics_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

