using OpenHardwareMonitor.Hardware;

using System;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace ArduinoPCMonitoring
{
    public partial class Form1 : Form
    {
        Computer computer = new Computer()
        {
            CPUEnabled = true,
            GPUEnabled = true,
            MainboardEnabled = true,
            FanControllerEnabled = true,
            RAMEnabled = true,
            HDDEnabled = true
        };

        IHardware cpu;
        IHardware gpu;
        IHardware mainboard;
        IHardware superIO;

        float?[] temp = { 0, 0 }; // CPU Temp, GPU Temp
        float?[] fanSpeed = { 0, 0 }; // CPU Fan RPM, GPU Fan RPM

        public Form1()
        {
            InitializeComponent();

            Init();
            InitTray();

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PortName) &&
                SerialPort.GetPortNames().Contains(Properties.Settings.Default.PortName))
            {
                AppEnvironment.port.PortName = Properties.Settings.Default.PortName;
                AppEnvironment.port.Open();

                AppEnvironment.updateTimer.Start();
            }
        }

        private void InitTray()
        {
            // Create tray icon menu items

            var exitItem = new ToolStripMenuItem()
            {
                Text = "Exit"
            };
            exitItem.Click += delegate { Close(); };

            var settingItem = new ToolStripMenuItem()
            {
                Text = "Setting"
            };
            settingItem.Click += delegate
            {
                var settingForm = new SettingForm();
                settingForm.Show();
            };


            // Add menu items in menu

            var menu = new ContextMenuStrip();

            menu.Items.Add(settingItem);
            menu.Items.Add(exitItem);


            // Create tray icon

            var tray = new NotifyIcon()
            {
                Icon = Properties.Resources.mainboard,
                Visible = true,
                Text = "AVR PC Monitoring",
                ContextMenuStrip = menu
            };
        }

        private void Init()
        {
            Visible = false;
            Opacity = 0;

            AppEnvironment.port = new SerialPort()
            {
                Parity = Parity.Odd,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true,
                BaudRate = 9600
            };

            computer.Open();

            cpu = (from h in computer.Hardware where h.HardwareType == HardwareType.CPU select h).First();
            gpu = (from h in computer.Hardware where h.HardwareType == HardwareType.GpuAti select h).First();
            mainboard = (from h in computer.Hardware where h.HardwareType == HardwareType.Mainboard select h).First();
            superIO = (from h in mainboard.SubHardware where h.HardwareType == HardwareType.SuperIO select h).First();

            AppEnvironment.updateTimer = new System.Timers.Timer();
            AppEnvironment.updateTimer.Elapsed += delegate { UpdateInfo(); };
            AppEnvironment.updateTimer.Interval = Properties.Settings.Default.UpdateInterval;
            AppEnvironment.updateTimer.Stop();
        }

        private void UpdateInfo()
        {
            cpu.Update();
            gpu.Update();
            mainboard.Update();
            superIO.Update();

            try
            {
                temp[0] = (from s in cpu.Sensors where (s.SensorType == SensorType.Temperature) && (s.Name == "CPU Package") select s).First().Value ?? 0;
            }
            catch
            {
                temp[0] = 0;
            }

            try
            {
                temp[1] = (from s in gpu.Sensors where (s.SensorType == SensorType.Temperature) && (s.Name == "GPU Core") select s).First().Value ?? 0;
            }
            catch
            {
                temp[1] = 0;
            }

            try
            {
                fanSpeed[0] = (from s in superIO.Sensors where (s.SensorType == SensorType.Fan) select s).First().Value ?? 0;
            }
            catch
            {
                fanSpeed[0] = 0;
            }
            finally
            {
                fanSpeed[0] = Convert.ToInt32(fanSpeed[0]);
            }

            try
            {
                fanSpeed[1] = (from s in gpu.Sensors where s.SensorType == SensorType.Fan select s).First().Value ?? 0;
            }
            catch
            {
                fanSpeed[1] = 0;
            }

            SendInfo();
        }

        private void SendInfo()
        {
            if (!AppEnvironment.port.IsOpen)
            {
                return;
            }

            try
            {
                AppEnvironment.port.Write($"c{temp[0]}g{temp[1]}f{fanSpeed[0]}s{fanSpeed[1]}*");
            }
            catch
            {
                MessageBox.Show("Disconnected Port", "Sending Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppEnvironment.port.Close();
            }
        }
    }
}
