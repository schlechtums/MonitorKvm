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
					   .Select(d => ((PropertyData)d.Properties["Name"]).Value.ToString())
					   .ToList();
        }

        private List<ManagementBaseObject> GetUsbDevices()
		{
			var usbDeviceAddresses = this.LookUpUsbDeviceAddresses();

			var usbDevices = new List<ManagementBaseObject>();

			foreach (var a in usbDeviceAddresses)
			{
				// query MI for the PNP device info
				// address must be escaped to be used in the query; luckily, the form we extracted previously is already escaped
				var curMoc = this.QueryMi("Select * from Win32_PnPEntity where PNPDeviceID = " + a);
				foreach (var device in curMoc)
				{
					usbDevices.Add(device);
				}
			}

			return usbDevices;
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