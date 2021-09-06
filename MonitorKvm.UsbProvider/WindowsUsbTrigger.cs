using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace MonitorKvm.UsbProvider
{
	public class WindowsUsbTrigger : UsbTriggerBase
	{
		public WindowsUsbTrigger(String triggerDeviceName)
			: base(triggerDeviceName)
        { }

        protected override List<String> GetUsbDeviceNames()
        {
			return this.GetUsbDevices()
					   .Select(this.GetUsbDeviceName)
					   .ToList();
        }

		private String GetUsbDeviceName(ManagementBaseObject d)
		{
			return ((PropertyData)d.Properties["Name"]).Value.ToString();
		}

		private String _TriggerDeviceAddress;

        private List<ManagementBaseObject> GetUsbDevices()
		{
			if (this._TriggerDeviceAddress == null)
			{
				var usbDeviceAddresses = this.LookUpUsbDeviceAddresses();

				foreach (var a in usbDeviceAddresses)
				{
					// query MI for the PNP device info
					// address must be escaped to be used in the query; luckily, the form we extracted previously is already escaped
					var devices = this.GetUsbDevicesFromAddress(a);
					foreach (var d in devices)
					{
						if (this.GetUsbDeviceName(d) == this.TriggerDeviceName)
						{
							this._TriggerDeviceAddress = a;
							return devices;
						}
					}
				}

				return new List<ManagementBaseObject>();
			}
			else
			{
				return this.GetUsbDevicesFromAddress(this._TriggerDeviceAddress);
			}
		}

		private List<ManagementBaseObject> GetUsbDevicesFromAddress(String deviceAddress)
        {
			return this.QueryMi("Select * from Win32_PnPEntity where PNPDeviceID = " + deviceAddress).OfType<ManagementBaseObject>().ToList();
		}

		private List<String> LookUpUsbDeviceAddresses()
		{
			// this query gets the addressing information for connected USB devices
			var usbDeviceAddressInfo = this.QueryMi(@"Select * from Win32_USBControllerDevice");

			var usbDeviceAddresses = new List<String>();

			foreach (var device in usbDeviceAddressInfo)
			{
				var curPnpAddress = (string)device.GetPropertyValue("Dependent");
				// split out the address portion of the data; note that this includes escaped backslashes and quotes
				curPnpAddress = curPnpAddress.Split(new String[] { "DeviceID=" }, 2, StringSplitOptions.None)[1];

				usbDeviceAddresses.Add(curPnpAddress);
			}

			return usbDeviceAddresses;
		}

		// run a query against Windows Management Infrastructure (MI) and return the resulting collection
		private ManagementObjectCollection QueryMi(String query)
		{
			var managementObjectSearcher = new ManagementObjectSearcher(query);
			var result = managementObjectSearcher.Get();

			managementObjectSearcher.Dispose();
			return result;
		}

	}
}