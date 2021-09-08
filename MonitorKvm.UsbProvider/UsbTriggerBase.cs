using Microsoft.Extensions.Logging;
using MonitorKvm.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonitorKvm.UsbProvider
{
    public abstract class UsbTriggerBase : MonitorKvmTriggerBase
    {
        public UsbTriggerBase(String triggerDeviceName)
            : base()
        {
            this.TriggerDeviceName = triggerDeviceName;
        }

        public static UsbTriggerBase GetUsbTriggerForOS(ILogger logger, String triggerDeviceName)
        {
            var platform = OSPlatformHelper.GetOSPlatform();
            if (platform == OSPlatform.Windows)
            {
                logger.LogInformation("Using UsbTriggerBase for Windows");
                return new WindowsUsbTrigger(triggerDeviceName);
            }
            else if (platform == OSPlatform.OSX)
            {
                logger.LogInformation("Using UsbTriggerBase for OSX");
                throw new Exception($"{OSPlatform.OSX} not supported");
            }
            else if (platform == OSPlatform.Linux)
            {
                logger.LogInformation("Using UsbTriggerBase for Linux");
                throw new Exception($"{OSPlatform.Linux} not supported");
            }
            else if (platform == OSPlatform.FreeBSD)
            {
                logger.LogInformation("Using UsbTriggerBase for FreeBSD");
                throw new Exception($"{OSPlatform.FreeBSD} not supported");
            }
            else
            {
                throw new Exception($"{Environment.OSVersion.VersionString} not supported");
            }
        }

        protected String TriggerDeviceName { get; private set; }

        protected abstract List<String> GetUsbDeviceNames();

        protected override Boolean ShouldBeActive(out String message)
        {
            var ret = this.GetUsbDeviceNames().Contains(this.TriggerDeviceName);

            if (ret)
                message = $"'{this.TriggerDeviceName}' connected.";
            else
                message = $"'{this.TriggerDeviceName}' disconnected.";

            return ret;
        }
    }
}
