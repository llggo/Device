namespace Vibrate
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
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.buttonSet = new System.Windows.Forms.Button();
            this.buttonVibrate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(78, 22);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(78, 20);
            this.textBoxAddress.TabIndex = 0;
            // 
            // buttonSet
            // 
            this.buttonSet.Location = new System.Drawing.Point(178, 18);
            this.buttonSet.Name = "buttonSet";
            this.buttonSet.Size = new System.Drawing.Size(79, 26);
            this.buttonSet.TabIndex = 2;
            this.buttonSet.Text = "SET";
            this.buttonSet.UseVisualStyleBackColor = true;
            this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
            // 
            // buttonVibrate
            // 
            this.buttonVibrate.Location = new System.Drawing.Point(277, 18);
            this.buttonVibrate.Name = "buttonVibrate";
            this.buttonVibrate.Size = new System.Drawing.Size(79, 26);
            this.buttonVibrate.TabIndex = 3;
            this.buttonVibrate.Text = "Vibrate";
            this.buttonVibrate.UseVisualStyleBackColor = true;
            this.buttonVibrate.Click += new System.EventHandler(this.buttonVibrate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Address";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 64);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonVibrate);
            this.Controls.Add(this.buttonSet);
            this.Controls.Add(this.textBoxAddress);
            this.Name = "Main";
            this.Text = "Vibrate";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Button buttonSet;
        private System.Windows.Forms.Button buttonVibrate;
        private System.Windows.Forms.Label label1;
    }
}

