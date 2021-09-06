using System;
using System.Collections.Generic;
using System.Text;

namespace MonitorKvm.Core
{
    public abstract class MonitorKvmUsbTriggerBase : MonitorKvmTriggerBase
    {
        public MonitorKvmUsbTriggerBase(String triggerDeviceName)
            : base()
        {
            this.TriggerDeviceName = triggerDeviceName;
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
