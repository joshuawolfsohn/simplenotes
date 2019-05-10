namespace Simple_Notes
{
    partial class Dialog_More
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
            this.button_settings = new System.Windows.Forms.Button();
            this.button_about = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_settings
            // 
            this.button_settings.Location = new System.Drawing.Point(41, 64);
            this.button_settings.Name = "button_settings";
            this.button_settings.Size = new System.Drawing.Size(435, 38);
            this.button_settings.TabIndex = 0;
            this.button_settings.Text = "Settings";
            this.button_settings.UseVisualStyleBackColor = true;
            this.button_settings.Click += new System.EventHandler(this.button_settings_Click);
            // 
            // button_about
            // 
            this.button_about.Location = new System.Drawing.Point(41, 132);
            this.button_about.Name = "button_about";
            this.button_about.Size = new System.Drawing.Size(435, 38);
            this.button_about.TabIndex = 1;
            this.button_about.Text = "About";
            this.button_about.UseVisualStyleBackColor = true;
            this.button_about.Click += new System.EventHandler(this.button_about_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 205);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(435, 38);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go back";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Dialog_More
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 312);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_about);
            this.Controls.Add(this.button_settings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog_More";
            this.Text = "Simple Notes - More";
            this.Load += new System.EventHandler(this.Dialog_More_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_settings;
        private System.Windows.Forms.Button button_about;
        private System.Windows.Forms.Button button1;
    }
}