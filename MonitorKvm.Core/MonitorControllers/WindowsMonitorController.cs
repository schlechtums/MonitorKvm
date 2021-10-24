using Schlechtums.Core.Common.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorKvm.Core.MonitorControllers
{
    internal class WindowsMonitorController : MonitorControllerBase
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;

        internal override void TurnMonitorsOff()
        {
            new Task(() => { SendMessage(new IntPtr(0xffff), 0x0112, new IntPtr(0xf170), new IntPtr(2)); }).Start();
        }

        internal override void TurnMonitorsOn()
        {
            new Task(() =>
            {
                Thread.Sleep(1500);
                mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);

                Thread.Sleep(10000);
                ProcessExtensions.RunProcessInBackground("EnsureWindowsNightMode.exe", null, null, false);
            }).Start();
        }
    }
}