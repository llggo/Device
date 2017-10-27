namespace Demo
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
            this.richTextBoxQueue = new System.Windows.Forms.RichTextBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.textBoxIdInput = new System.Windows.Forms.TextBox();
            this.buttonVir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxQueue
            // 
            this.richTextBoxQueue.Dock = System.Windows.Forms.DockStyle.Left;
            this.richTextBoxQueue.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxQueue.Name = "richTextBoxQueue";
            this.richTextBoxQueue.Size = new System.Drawing.Size(93, 261);
            this.richTextBoxQueue.TabIndex = 0;
            this.richTextBoxQueue.Text = "";
            // 
            // buttonNext
            // 
            this.buttonNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonNext.Location = new System.Drawing.Point(248, 0);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(90, 261);
            this.buttonNext.TabIndex = 1;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // textBoxIdInput
            // 
            this.textBoxIdInput.Location = new System.Drawing.Point(124, 229);
            this.textBoxIdInput.Name = "textBoxIdInput";
            this.textBoxIdInput.Size = new System.Drawing.Size(84, 20);
            this.textBoxIdInput.TabIndex = 2;
            this.textBoxIdInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxIdInput_KeyPress);
            // 
            // buttonVir
            // 
            this.buttonVir.Location = new System.Drawing.Point(133, 12);
            this.buttonVir.Name = "buttonVir";
            this.buttonVir.Size = new System.Drawing.Size(75, 23);
            this.buttonVir.TabIndex = 3;
            this.buttonVir.Text = "Vir";
            this.buttonVir.UseVisualStyleBackColor = true;
            this.buttonVir.Click += new System.EventHandler(this.buttonVir_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 261);
            this.Controls.Add(this.buttonVir);
            this.Controls.Add(this.textBoxIdInput);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.richTextBoxQueue);
            this.Name = "Main";
            this.Text = "Demo";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxQueue;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.TextBox textBoxIdInput;
        private System.Windows.Forms.Button buttonVir;
    }
}

