using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static TwinkleTrayAvalonia.Helpers.NativeMethods;

namespace TwinkleTrayAvalonia.Helpers
{
    public class DDCCIHelper
    {
        public class PhysicalMonitor : IDisposable
        {
            public IntPtr Handle { get; set; }
            public string Description { get; set; } = "";
            public string DeviceName { get; set; } = "";

            public void Dispose()
            {
                if (Handle != IntPtr.Zero)
                {
                    DestroyPhysicalMonitors(1, new[] { new PHYSICAL_MONITOR { hPhysicalMonitor = Handle } });
                    Handle = IntPtr.Zero;
                }
            }
        }

        public static List<PhysicalMonitor> GetPhysicalMonitors()
        {
            var monitors = new List<PhysicalMonitor>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
            {
                if (GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint count))
                {
                    var physicalMonitors = new PHYSICAL_MONITOR[count];
                    if (GetPhysicalMonitorsFromHMONITOR(hMonitor, count, physicalMonitors))
                    {
                        var info = MonitorInfoEx.Create();
                        GetMonitorInfo(hMonitor, ref info);

                        foreach (var pm in physicalMonitors)
                        {
                            monitors.Add(new PhysicalMonitor
                            {
                                Handle = pm.hPhysicalMonitor,
                                Description = pm.szPhysicalMonitorDescription,
                                DeviceName = info.DeviceName
                            });
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            return monitors;
        }

        public static bool GetBrightness(IntPtr hPhysicalMonitor, out uint current, out uint max)
        {
            current = 0;
            max = 100;
            return GetMonitorBrightness(hPhysicalMonitor, out _, out current, out max);
        }

        public static bool SetBrightness(IntPtr hPhysicalMonitor, uint brightness)
        {
            return SetMonitorBrightness(hPhysicalMonitor, brightness);
        }

        public static bool GetVCP(IntPtr hPhysicalMonitor, byte vcpCode, out uint current, out uint max)
        {
            return GetVCPFeatureAndVCPFeatureReply(hPhysicalMonitor, vcpCode, out _, out current, out max);
        }

        public static bool SetVCP(IntPtr hPhysicalMonitor, byte vcpCode, uint value)
        {
            return SetVCPFeature(hPhysicalMonitor, vcpCode, value);
        }
    }
}
