using System;
using System.Runtime.InteropServices;

namespace MonitorKvm.Core
{
    public static class OSPlatformHelper
    {
        public static OSPlatform GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OSPlatform.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OSPlatform.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OSPlatform.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return OSPlatform.FreeBSD;
            else
                throw new Exception($"Unknown OS platform: {Environment.OSVersion.VersionString}");
        }
    }
}
