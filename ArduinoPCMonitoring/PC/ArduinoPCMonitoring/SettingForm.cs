using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ArduinoPCMonitoring
{
    public partial class SettingForm : Form
    {
        Timer connectedLabelTimer = new Timer();

        public SettingForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.mainboard;

            PortSelectComboBox.Items.Clear();
            PortSelectComboBox.Items.AddRange(SerialPort.GetPortNames());
            UpdateIntervalUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.UpdateInterval);

            connectedLabelTimer.Interval = 300;
            connectedLabelTimer.Tick += delegate { ConnectStautsLabel.Text = AppEnvironment.port.IsOpen ? "Connected" : "Disconnected"; };
            connectedLabelTimer.Start();
        }

        private void UpdateIntervalUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppEnvironment.updateTimer.Interval = Properties.Settings.Default.UpdateInterval = decimal.ToDouble((sender as NumericUpDown).Value);

            Properties.Settings.Default.Save();
        }

        private void PortConnectButton_Click(object sender, EventArgs e)
        {
            string portName = PortSelectComboBox.Text;

            if (string.IsNullOrWhiteSpace(portName))
            {
                return;
            }

            if (!AppEnvironment.port.IsOpen)
            {
                AppEnvironment.port.PortName = portName;
                AppEnvironment.port.Open();

                AppEnvironment.updateTimer.Start();

                Properties.Settings.Default.PortName = portName;
                Properties.Settings.Default.Save();
            }
        }

        private void PortDisconnectButton_Click(object sender, EventArgs e)
        {
            if (AppEnvironment.port.IsOpen)
            {
                AppEnvironment.port.Close();
            }
        }
    }
}
