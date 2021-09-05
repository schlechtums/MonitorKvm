using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
											 .Single(aep => aep.DeviceFriendlyName == triggerDeviceFriendlyName);

			this._TriggerDeviceId = headsetSpeakerDevice.ID;
		}

		private ILogger<Worker> _Logger;
		private String _TriggerDeviceId;

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

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
					SendMessage(new IntPtr(0xffff), 0x0112, new IntPtr(0xf170), new IntPtr(2));
				}
			}
		}

		public void OnPropertyValueChanged(String pwstrDeviceId, PropertyKey key)
		{

		}
	}
}