using System;
using System.Management;
using System.Linq;

namespace TwinkleTrayAvalonia.Helpers
{
    public class WMIHelper
    {
        public static int GetBrightness()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM WmiMonitorBrightness");
                using var results = searcher.Get();
                foreach (var obj in results)
                {
                    return Convert.ToInt32(obj["CurrentBrightness"]);
                }
            }
            catch { }
            return -1;
        }

        public static void SetBrightness(int brightness)
        {
            try
            {
                var scope = new ManagementScope("root\\WMI");
                using var searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM WmiMonitorBrightnessMethods"));
                using var results = searcher.Get();
                foreach (ManagementObject obj in results)
                {
                    obj.InvokeMethod("WmiSetBrightness", new object[] { uint.MaxValue, (byte)brightness });
                }
            }
            catch { }
        }
    }
}
