namespace BlazeSharp.UI
{
    partial class LiveStatsUI
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            this.txtLastKeyChar = new System.Windows.Forms.TextBox();
            this.txtLastKeycode = new System.Windows.Forms.TextBox();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.chkIsCapturing = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(48, 13);
            label1.TabIndex = 0;
            label1.Text = "Last Key";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 35);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(54, 13);
            label3.TabIndex = 4;
            label3.Text = "Command";
            // 
            // txtLastKeyChar
            // 
            this.txtLastKeyChar.Enabled = false;
            this.txtLastKeyChar.Location = new System.Drawing.Point(89, 6);
            this.txtLastKeyChar.Name = "txtLastKeyChar";
            this.txtLastKeyChar.ReadOnly = true;
            this.txtLastKeyChar.Size = new System.Drawing.Size(74, 20);
            this.txtLastKeyChar.TabIndex = 1;
            // 
            // txtLastKeycode
            // 
            this.txtLastKeycode.Enabled = false;
            this.txtLastKeycode.Location = new System.Drawing.Point(178, 6);
            this.txtLastKeycode.Name = "txtLastKeycode";
            this.txtLastKeycode.ReadOnly = true;
            this.txtLastKeycode.Size = new System.Drawing.Size(74, 20);
            this.txtLastKeycode.TabIndex = 3;
            // 
            // txtCommand
            // 
            this.txtCommand.Enabled = false;
            this.txtCommand.Location = new System.Drawing.Point(89, 32);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ReadOnly = true;
            this.txtCommand.Size = new System.Drawing.Size(163, 20);
            this.txtCommand.TabIndex = 5;
            // 
            // chkIsCapturing
            // 
            this.chkIsCapturing.AutoSize = true;
            this.chkIsCapturing.Enabled = false;
            this.chkIsCapturing.Location = new System.Drawing.Point(258, 34);
            this.chkIsCapturing.Name = "chkIsCapturing";
            this.chkIsCapturing.Size = new System.Drawing.Size(71, 17);
            this.chkIsCapturing.TabIndex = 6;
            this.chkIsCapturing.Text = "Capturing";
            this.chkIsCapturing.UseVisualStyleBackColor = true;
            // 
            // LiveStatsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(378, 65);
            this.Controls.Add(this.chkIsCapturing);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(label3);
            this.Controls.Add(this.txtLastKeycode);
            this.Controls.Add(this.txtLastKeyChar);
            this.Controls.Add(label1);
            this.Name = "LiveStatsUI";
            this.Text = "Live Stats";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtLastKeyChar;
        private System.Windows.Forms.TextBox txtLastKeycode;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.CheckBox chkIsCapturing;
    }
}