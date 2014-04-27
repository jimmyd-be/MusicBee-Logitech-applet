namespace MusicBeePlugin
{
    partial class Settings
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
      this.AlwaysOnTopRadioButton = new System.Windows.Forms.RadioButton();
      this.NotAlwaysOnTopRadioButton = new System.Windows.Forms.RadioButton();
      this.label1 = new System.Windows.Forms.Label();
      this.saveButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.volumeChangerSpinBox = new System.Windows.Forms.NumericUpDown();
      this.label2 = new System.Windows.Forms.Label();
      this.disabledScreensList = new System.Windows.Forms.ListBox();
      this.enabledScreensList = new System.Windows.Forms.ListBox();
      this.upButton = new System.Windows.Forms.Button();
      this.downButton = new System.Windows.Forms.Button();
      this.enableButton = new System.Windows.Forms.Button();
      this.disableButton = new System.Windows.Forms.Button();
      this.backgroundLabel = new System.Windows.Forms.Label();
      this.BackgroundDefaultButton = new System.Windows.Forms.RadioButton();
      this.BackgroundCustomButton = new System.Windows.Forms.RadioButton();
      this.browseButton = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.logitechLabel = new System.Windows.Forms.Label();
      this.defaultScreencomboBox = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.volumeChangerSpinBox)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // AlwaysOnTopRadioButton
      // 
      this.AlwaysOnTopRadioButton.AutoSize = true;
      this.AlwaysOnTopRadioButton.Location = new System.Drawing.Point(5, 13);
      this.AlwaysOnTopRadioButton.Name = "AlwaysOnTopRadioButton";
      this.AlwaysOnTopRadioButton.Size = new System.Drawing.Size(91, 17);
      this.AlwaysOnTopRadioButton.TabIndex = 0;
      this.AlwaysOnTopRadioButton.TabStop = true;
      this.AlwaysOnTopRadioButton.Text = "Always on top";
      this.AlwaysOnTopRadioButton.UseVisualStyleBackColor = true;
      this.AlwaysOnTopRadioButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
      // 
      // NotAlwaysOnTopRadioButton
      // 
      this.NotAlwaysOnTopRadioButton.AutoSize = true;
      this.NotAlwaysOnTopRadioButton.Location = new System.Drawing.Point(5, 36);
      this.NotAlwaysOnTopRadioButton.Name = "NotAlwaysOnTopRadioButton";
      this.NotAlwaysOnTopRadioButton.Size = new System.Drawing.Size(110, 17);
      this.NotAlwaysOnTopRadioButton.TabIndex = 1;
      this.NotAlwaysOnTopRadioButton.TabStop = true;
      this.NotAlwaysOnTopRadioButton.Text = "Not always on top";
      this.NotAlwaysOnTopRadioButton.UseVisualStyleBackColor = true;
      this.NotAlwaysOnTopRadioButton.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(12, 53);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Plugin Mode:";
      this.label1.Click += new System.EventHandler(this.label1_Click);
      // 
      // saveButton
      // 
      this.saveButton.Location = new System.Drawing.Point(256, 432);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(75, 23);
      this.saveButton.TabIndex = 3;
      this.saveButton.Text = "Save";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(337, 432);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 4;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.button2_Click);
      // 
      // volumeChangerSpinBox
      // 
      this.volumeChangerSpinBox.Location = new System.Drawing.Point(119, 111);
      this.volumeChangerSpinBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.volumeChangerSpinBox.Name = "volumeChangerSpinBox";
      this.volumeChangerSpinBox.Size = new System.Drawing.Size(120, 20);
      this.volumeChangerSpinBox.TabIndex = 5;
      this.volumeChangerSpinBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.volumeChangerSpinBox.ValueChanged += new System.EventHandler(this.volumeChangerSpinBox_ValueChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(12, 111);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(102, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Volume changer:";
      // 
      // disabledScreensList
      // 
      this.disabledScreensList.FormattingEnabled = true;
      this.disabledScreensList.Location = new System.Drawing.Point(15, 152);
      this.disabledScreensList.Name = "disabledScreensList";
      this.disabledScreensList.Size = new System.Drawing.Size(118, 147);
      this.disabledScreensList.TabIndex = 7;
      // 
      // enabledScreensList
      // 
      this.enabledScreensList.FormattingEnabled = true;
      this.enabledScreensList.Location = new System.Drawing.Point(199, 152);
      this.enabledScreensList.Name = "enabledScreensList";
      this.enabledScreensList.Size = new System.Drawing.Size(120, 147);
      this.enabledScreensList.TabIndex = 8;
      // 
      // upButton
      // 
      this.upButton.Location = new System.Drawing.Point(337, 166);
      this.upButton.Name = "upButton";
      this.upButton.Size = new System.Drawing.Size(75, 23);
      this.upButton.TabIndex = 9;
      this.upButton.Text = "Up";
      this.upButton.UseVisualStyleBackColor = true;
      this.upButton.Click += new System.EventHandler(this.upButton_Click);
      // 
      // downButton
      // 
      this.downButton.Location = new System.Drawing.Point(337, 213);
      this.downButton.Name = "downButton";
      this.downButton.Size = new System.Drawing.Size(75, 23);
      this.downButton.TabIndex = 10;
      this.downButton.Text = "Down";
      this.downButton.UseVisualStyleBackColor = true;
      this.downButton.Click += new System.EventHandler(this.downButton_Click);
      // 
      // enableButton
      // 
      this.enableButton.Location = new System.Drawing.Point(139, 166);
      this.enableButton.Name = "enableButton";
      this.enableButton.Size = new System.Drawing.Size(54, 23);
      this.enableButton.TabIndex = 11;
      this.enableButton.Text = "Add";
      this.enableButton.UseVisualStyleBackColor = true;
      this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
      // 
      // disableButton
      // 
      this.disableButton.Location = new System.Drawing.Point(139, 213);
      this.disableButton.Name = "disableButton";
      this.disableButton.Size = new System.Drawing.Size(54, 23);
      this.disableButton.TabIndex = 12;
      this.disableButton.Text = "Remove";
      this.disableButton.UseVisualStyleBackColor = true;
      this.disableButton.Click += new System.EventHandler(this.disableButton_Click);
      // 
      // backgroundLabel
      // 
      this.backgroundLabel.AutoSize = true;
      this.backgroundLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.backgroundLabel.Location = new System.Drawing.Point(12, 358);
      this.backgroundLabel.Name = "backgroundLabel";
      this.backgroundLabel.Size = new System.Drawing.Size(79, 13);
      this.backgroundLabel.TabIndex = 13;
      this.backgroundLabel.Text = "Background:";
      this.backgroundLabel.Click += new System.EventHandler(this.label3_Click_1);
      // 
      // BackgroundDefaultButton
      // 
      this.BackgroundDefaultButton.AutoSize = true;
      this.BackgroundDefaultButton.Location = new System.Drawing.Point(111, 357);
      this.BackgroundDefaultButton.Name = "BackgroundDefaultButton";
      this.BackgroundDefaultButton.Size = new System.Drawing.Size(81, 17);
      this.BackgroundDefaultButton.TabIndex = 14;
      this.BackgroundDefaultButton.TabStop = true;
      this.BackgroundDefaultButton.Text = "Use Default";
      this.BackgroundDefaultButton.UseVisualStyleBackColor = true;
      this.BackgroundDefaultButton.CheckedChanged += new System.EventHandler(this.BackgroundDefaultButton_CheckedChanged);
      // 
      // BackgroundCustomButton
      // 
      this.BackgroundCustomButton.AutoSize = true;
      this.BackgroundCustomButton.Location = new System.Drawing.Point(111, 380);
      this.BackgroundCustomButton.Name = "BackgroundCustomButton";
      this.BackgroundCustomButton.Size = new System.Drawing.Size(82, 17);
      this.BackgroundCustomButton.TabIndex = 16;
      this.BackgroundCustomButton.TabStop = true;
      this.BackgroundCustomButton.Text = "Use Custom";
      this.BackgroundCustomButton.UseVisualStyleBackColor = true;
      this.BackgroundCustomButton.CheckedChanged += new System.EventHandler(this.BackgroundCustomButton_CheckedChanged);
      // 
      // browseButton
      // 
      this.browseButton.Enabled = false;
      this.browseButton.Location = new System.Drawing.Point(131, 403);
      this.browseButton.Name = "browseButton";
      this.browseButton.Size = new System.Drawing.Size(75, 23);
      this.browseButton.TabIndex = 17;
      this.browseButton.Text = "Browse";
      this.browseButton.UseVisualStyleBackColor = true;
      this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(14, 319);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(80, 13);
      this.label4.TabIndex = 18;
      this.label4.Text = "Start screen:";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.AlwaysOnTopRadioButton);
      this.panel1.Controls.Add(this.NotAlwaysOnTopRadioButton);
      this.panel1.Location = new System.Drawing.Point(119, 37);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(132, 59);
      this.panel1.TabIndex = 21;
      // 
      // logitechLabel
      // 
      this.logitechLabel.AutoSize = true;
      this.logitechLabel.Location = new System.Drawing.Point(14, 9);
      this.logitechLabel.Name = "logitechLabel";
      this.logitechLabel.Size = new System.Drawing.Size(159, 13);
      this.logitechLabel.TabIndex = 22;
      this.logitechLabel.Text = "No Logitech device found so far";
      // 
      // defaultScreencomboBox
      // 
      this.defaultScreencomboBox.FormattingEnabled = true;
      this.defaultScreencomboBox.Location = new System.Drawing.Point(119, 316);
      this.defaultScreencomboBox.Name = "defaultScreencomboBox";
      this.defaultScreencomboBox.Size = new System.Drawing.Size(120, 21);
      this.defaultScreencomboBox.TabIndex = 19;
      this.defaultScreencomboBox.SelectedIndexChanged += new System.EventHandler(this.defaultScreencomboBox_SelectedIndexChanged);
      // 
      // Settings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(424, 467);
      this.Controls.Add(this.logitechLabel);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.BackgroundCustomButton);
      this.Controls.Add(this.BackgroundDefaultButton);
      this.Controls.Add(this.browseButton);
      this.Controls.Add(this.defaultScreencomboBox);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.backgroundLabel);
      this.Controls.Add(this.disableButton);
      this.Controls.Add(this.enableButton);
      this.Controls.Add(this.downButton);
      this.Controls.Add(this.upButton);
      this.Controls.Add(this.enabledScreensList);
      this.Controls.Add(this.disabledScreensList);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.volumeChangerSpinBox);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.label1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Settings";
      this.Text = "Settings";
      this.TopMost = true;
      ((System.ComponentModel.ISupportInitialize)(this.volumeChangerSpinBox)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton AlwaysOnTopRadioButton;
        private System.Windows.Forms.RadioButton NotAlwaysOnTopRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.NumericUpDown volumeChangerSpinBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox disabledScreensList;
        private System.Windows.Forms.ListBox enabledScreensList;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button enableButton;
        private System.Windows.Forms.Button disableButton;
        private System.Windows.Forms.Label backgroundLabel;
        private System.Windows.Forms.RadioButton BackgroundDefaultButton;
        private System.Windows.Forms.RadioButton BackgroundCustomButton;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label logitechLabel;
        private System.Windows.Forms.ComboBox defaultScreencomboBox;

    }
}