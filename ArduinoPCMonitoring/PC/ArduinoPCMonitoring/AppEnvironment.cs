using System.IO.Ports;
using System.Timers;

namespace ArduinoPCMonitoring
{
    internal static class AppEnvironment
    {
        internal static bool isConnect = false;
        internal static Timer updateTimer;
        internal static SerialPort port;
    }
}
