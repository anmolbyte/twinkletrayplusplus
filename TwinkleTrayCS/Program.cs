using System;
using System.Windows.Forms;
using TwinkleTrayCS.Views;
using TwinkleTrayCS.Worker;

namespace TwinkleTrayCS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Start the background engine
            Engine.Instance.Start();

            // Create the system tray
            using var tray = new SystemTray();

            Application.Run();
        }
    }
}