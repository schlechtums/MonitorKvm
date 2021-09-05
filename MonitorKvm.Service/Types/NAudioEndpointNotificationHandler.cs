using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

		const UInt32 WM_KEYDOWN = 0x0100;
		const int VK_F5 = 0x7E;

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

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
					new Task(() =>
					{
						SendMessage(new IntPtr(0xffff), 0x0112, new IntPtr(0xf170), new IntPtr(2));
					}).Start();

					Thread.Sleep(5000);
					this._F15CTS = new CancellationTokenSource();
					new Task(this.DoF15).Start();
				}
				else
                {
					this._F15CTS.Cancel();
				}
			}
		}

		private CancellationTokenSource _F15CTS;
		private void DoF15()
        {
			Process[] processes = Process.GetProcessesByName("explorer");

			while (!this._F15CTS.IsCancellationRequested)
            {
				foreach (var proc in processes)
				{
					PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
				}

				Thread.Sleep(1000);
            }

			this._Logger.LogInformation("Waking monitors...");
			Thread.Sleep(1500);
			mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
		}


		public void OnPropertyValueChanged(String pwstrDeviceId, PropertyKey key)
		{

		}
	}
}