using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorKvm.Core.MonitorControllers
{
    public static class MonitorController
    {
        static MonitorController()
        {
            var platform = OSPlatformHelper.GetOSPlatform();
            if (platform == OSPlatform.Windows)
            {
                MonitorController._MonitorController = new WindowsMonitorController();
            }
            else if (platform == OSPlatform.OSX)
            {
                throw new Exception($"{OSPlatform.OSX} not supported");
            }
            else if (platform == OSPlatform.Linux)
            {
                throw new Exception($"{OSPlatform.Linux} not supported");
            }
            else if (platform == OSPlatform.FreeBSD)
            {
                throw new Exception($"{OSPlatform.FreeBSD} not supported");
            }
            else
            {
                throw new Exception($"{Environment.OSVersion.VersionString} not supported");
            }
        }

        private static MonitorControllerBase _MonitorController;

        public static void TurnMonitorsOff()
        {
            MonitorController._MonitorController.TurnMonitorsOff();
        }

        public static void TurnMonitorsOn()
        {
            MonitorController._MonitorController.TurnMonitorsOn();
        }
    }
}
