using System;

namespace MonitorKvm.Core
{
    public abstract class MonitorKvmTriggerBase
    {
        protected abstract Boolean ShouldBeActive(out String message);

        private Boolean? _Active;

        public void PollForChange()
        {
            if (this._Active == null)
                this._Active = this.ShouldBeActive(out String _);
            else
            {
                var active = this.ShouldBeActive(out String message);
                if (this._Active != active)
                {
                    this._Active = active;
                    if (active)
                        this.OnWakeMonitors(this, new MonitorKvmEventArgs(message));
                    else
                        this.OnSleepMonitors(this, new MonitorKvmEventArgs(message));
                }
            }
        }

        public EventHandler<MonitorKvmEventArgs> OnWakeMonitors;
        public EventHandler<MonitorKvmEventArgs> OnSleepMonitors;
    }
}