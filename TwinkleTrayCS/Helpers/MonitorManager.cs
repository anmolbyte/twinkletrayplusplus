using System;
using System.Collections.Generic;
using TwinkleTrayCS.Models;
using TwinkleTrayCS.Worker;

namespace TwinkleTrayCS.Helpers
{
    public class MonitorInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public string Type { get; set; } = "none"; // ddcci, wmi
        public int Brightness { get; set; }
        public IntPtr PhysicalHandle { get; set; }
    }

    public class MonitorManager
    {
        public static List<MonitorInfo> DiscoverMonitors()
        {
            var monitors = new List<MonitorInfo>();

            // 1. DDC/CI Monitors
            var ddcciMonitors = DDCCIHelper.GetPhysicalMonitors();
            foreach (var dm in ddcciMonitors)
            {
                if (DDCCIHelper.GetBrightness(dm.Handle, out uint current, out _))
                {
                    int currentBrightness = (int)current;
                    if (Engine.Instance.Settings.Remaps.TryGetValue(dm.Description, out var points))
                    {
                        currentBrightness = Utils.GetCalibratedValue(currentBrightness, points, true);
                    }

                    monitors.Add(new MonitorInfo
                    {
                        Id = dm.DeviceName + dm.Handle.ToString(),
                        Name = dm.Description,
                        DeviceName = dm.DeviceName,
                        Type = "ddcci",
                        Brightness = currentBrightness,
                        PhysicalHandle = dm.Handle
                    });
                }
            }

            // 2. WMI (Internal) Monitors
            int wmiBrightness = WMIHelper.GetBrightness();
            if (wmiBrightness != -1)
            {
                monitors.Add(new MonitorInfo
                {
                    Id = "WMI_Internal",
                    Name = "Internal Display",
                    Type = "wmi",
                    Brightness = wmiBrightness
                });
            }

            return monitors;
        }

        public static void SetBrightness(MonitorInfo monitor, int brightness)
        {
            int hwBrightness = brightness;
            if (Engine.Instance.Settings.Remaps.TryGetValue(monitor.Name, out var points))
            {
                hwBrightness = Utils.GetCalibratedValue(brightness, points, false);
            }

            if (monitor.Type == "ddcci")
            {
                DDCCIHelper.SetBrightness(monitor.PhysicalHandle, (uint)hwBrightness);
            }
            else if (monitor.Type == "wmi")
            {
                WMIHelper.SetBrightness(hwBrightness);
            }
            monitor.Brightness = brightness;
        }
    }
}
