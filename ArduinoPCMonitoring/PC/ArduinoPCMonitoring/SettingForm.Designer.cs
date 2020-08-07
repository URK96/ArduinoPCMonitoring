namespace ArduinoPCMonitoring
{
    partial class SettingForm
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
            this.ConnectStautsLabel = new System.Windows.Forms.Label();
            this.PortDisconnectButton = new System.Windows.Forms.Button();
            this.PortConnectButton = new System.Windows.Forms.Button();
            this.PortSelectComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.UpdateIntervalUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // ConnectStautsLabel
            // 
            this.ConnectStautsLabel.AutoSize = true;
            this.ConnectStautsLabel.Location = new System.Drawing.Point(161, 135);
            this.ConnectStautsLabel.Name = "ConnectStautsLabel";
            this.ConnectStautsLabel.Size = new System.Drawing.Size(57, 12);
            this.ConnectStautsLabel.TabIndex = 7;
            this.ConnectStautsLabel.Text = "연결 안됨";
            // 
            // PortDisconnectButton
            // 
            this.PortDisconnectButton.Location = new System.Drawing.Point(292, 37);
            this.PortDisconnectButton.Name = "PortDisconnectButton";
            this.PortDisconnectButton.Size = new System.Drawing.Size(74, 20);
            this.PortDisconnectButton.TabIndex = 6;
            this.PortDisconnectButton.Text = "연결 해제";
            this.PortDisconnectButton.UseVisualStyleBackColor = true;
            this.PortDisconnectButton.Click += new System.EventHandler(this.PortDisconnectButton_Click);
            // 
            // PortConnectButton
            // 
            this.PortConnectButton.Location = new System.Drawing.Point(212, 37);
            this.PortConnectButton.Name = "PortConnectButton";
            this.PortConnectButton.Size = new System.Drawing.Size(74, 20);
            this.PortConnectButton.TabIndex = 5;
            this.PortConnectButton.Text = "연결";
            this.PortConnectButton.UseVisualStyleBackColor = true;
            this.PortConnectButton.Click += new System.EventHandler(this.PortConnectButton_Click);
            // 
            // PortSelectComboBox
            // 
            this.PortSelectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PortSelectComboBox.FormattingEnabled = true;
            this.PortSelectComboBox.Location = new System.Drawing.Point(85, 37);
            this.PortSelectComboBox.Name = "PortSelectComboBox";
            this.PortSelectComboBox.Size = new System.Drawing.Size(121, 20);
            this.PortSelectComboBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "포트 설정";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "갱신 주기";
            // 
            // UpdateIntervalUpDown
            // 
            this.UpdateIntervalUpDown.Location = new System.Drawing.Point(85, 80);
            this.UpdateIntervalUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.UpdateIntervalUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.UpdateIntervalUpDown.Name = "UpdateIntervalUpDown";
            this.UpdateIntervalUpDown.Size = new System.Drawing.Size(120, 21);
            this.UpdateIntervalUpDown.TabIndex = 10;
            this.UpdateIntervalUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.UpdateIntervalUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.UpdateIntervalUpDown.ValueChanged += new System.EventHandler(this.UpdateIntervalUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(211, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "ms";
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 156);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.UpdateIntervalUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConnectStautsLabel);
            this.Controls.Add(this.PortDisconnectButton);
            this.Controls.Add(this.PortConnectButton);
            this.Controls.Add(this.PortSelectComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.Text = "Setting";
            ((System.ComponentModel.ISupportInitialize)(this.UpdateIntervalUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ConnectStautsLabel;
        private System.Windows.Forms.Button PortDisconnectButton;
        private System.Windows.Forms.Button PortConnectButton;
        private System.Windows.Forms.ComboBox PortSelectComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown UpdateIntervalUpDown;
        private System.Windows.Forms.Label label4;
    }
}