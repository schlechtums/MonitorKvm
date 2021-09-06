# MonitorKvm

This is a tool to serve as a software video kvm switch. It works in the scenario where you have monitors which have an auto input detection feature and you have an actual way of switching a usb device between two or more computers.

To use, run the MonitorKVM.Server executable passing the name of the USB device to use as the switch trigger.  The tool will sleep or wake the monitors for that computer when the specified USB decide is connected or disconnected.

Currently only windows is implemented despite there being multiple packages in the release.
