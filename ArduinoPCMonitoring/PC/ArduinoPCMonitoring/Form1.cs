using OpenHardwareMonitor.Hardware;

using System;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
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

            var menu = new ContextMenuStrip();

            menu.Items.Add(exitItem);
            menu.Items.Add(settingItem);


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

        #region Window styles

        [Flags]
        public enum ExtendedWindowStyles
        {
            WS_EX_TOOLWINDOW = 0x00000080,
        }

        public enum GetWindowLongFields
        {
            GWL_EXSTYLE = (-20),
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;

            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                int tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        #endregion
    }
}
