using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MonitorKvm.Service.Types
{
	public class NAudioEndpointNotificationHandler : IMMNotificationClient
	{
		public NAudioEndpointNotificationHandler(ILogger<Worker> logger)
        {
			this._Logger = logger;

			this.FindTriggerDevice("Logitech G35 Headset");
		}

		private void FindTriggerDevice(String triggerDeviceFriendlyName)
        {
			var de = new MMDeviceEnumerator();

			de.RegisterEndpointNotificationCallback(this);

			var headsetSpeakerDevice = de.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All)
											 .Single(aep => aep.DeviceFriendlyName.Contains(triggerDeviceFriendlyName) && aep.State == DeviceState.Active);

			this._TriggerDeviceId = headsetSpeakerDevice.ID;
		}

		private ILogger<Worker> _Logger;
		private String _TriggerDeviceId;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);
		private const int MOUSEEVENTF_MOVE = 0x0001;

		public void OnDefaultDeviceChanged(DataFlow flow, Role role, String defaultDeviceId)
		{

		}

		public void OnDeviceAdded(String pwstrDeviceId)
		{

		}

		public void OnDeviceRemoved(String deviceId)
		{

		}

		public void OnDeviceStateChanged(string deviceId, DeviceState newState)
		{
			if (deviceId == this._TriggerDeviceId)
			{
				this._Logger.LogInformation($"State Changed: {newState}");
				
				if (newState == DeviceState.NotPresent)
				{
					this._Logger.LogInformation("Sleeping monitors...");
					//thread dies for some reason, run in new task
					new Task(() => { SendMessage(new IntPtr(0xffff), 0x0112, new IntPtr(0xf170), new IntPtr(2)); }).Start();
				}
				else
                {
					this._Logger.LogInformation("Waking monitors...");
					mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
				}
			}
		}

		public void OnPropertyValueChanged(String pwstrDeviceId, PropertyKey key)
		{

		}
	}
}