using System;
using Microsoft.Win32;
using System.Windows.Forms;

namespace TwinkleTrayWPF.Helpers
{
    public class AppStartupHelper
    {
        private const string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "TwinkleTrayWPF";

        public static void SetStartup(bool enable)
        {
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true)!;
                if (enable)
                {
                    key.SetValue(APP_NAME, $"\"{Application.ExecutablePath}\"");
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
