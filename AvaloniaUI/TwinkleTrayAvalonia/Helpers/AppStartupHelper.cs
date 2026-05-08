using System;
using Microsoft.Win32;
using System.IO;

namespace TwinkleTrayAvalonia.Helpers
{
    public class AppStartupHelper
    {
        private const string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "TwinkleTrayAvalonia";

        public static void SetStartup(bool enable)
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true)!;
                if (enable)
                {
                    string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        key.SetValue(APP_NAME, $"\"{exePath}\"");
                    }
                }
                else
                {
                    key.DeleteValue(APP_NAME, false);
                }
            }
            catch { }
        }

        public static bool IsStartupEnabled()
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_KEY, false)!;
                return key.GetValue(APP_NAME) != null;
            }
            catch { return false; }
        }
    }
}
