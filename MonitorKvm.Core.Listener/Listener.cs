using Microsoft.Extensions.Logging;
using MonitorKvm.UsbProvider;
using System;

namespace MonitorKvm.Core.Listener
{
    public class Listener
    {
        public Listener(ILogger logger, String usbTriggerDeviceName)
        {
            this._Logger = logger;

            this._KvmTrigger = UsbTriggerBase.GetUsbTriggerForOS(this._Logger, usbTriggerDeviceName);
            this._KvmTrigger.OnSleepMonitors += this.HandleSleepMonitors;
            this._KvmTrigger.OnWakeMonitors += this.HandleWakeMonitors;
        }

        private ILogger _Logger;
        private MonitorKvmTriggerBase _KvmTrigger;

        private void HandleSleepMonitors(Object sender, MonitorKvmEventArgs e)
        {
            this._Logger.LogInformation($"{e.Message}  Sleeping monitors...");
            MonitorController.TurnMonitorsOff();
        }

        private void HandleWakeMonitors(Object sender, MonitorKvmEventArgs e)
        {
            this._Logger.LogInformation($"{e.Message}  Waking monitors...");
            MonitorController.TurnMonitorsOn();
        }

        public void PollForChange()
        {
            this._KvmTrigger.PollForChange();
        }
    }
}
