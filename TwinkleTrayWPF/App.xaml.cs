using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using TwinkleTrayWPF.Views;
using TwinkleTrayWPF.Worker;
using System.Drawing;
using System;

namespace TwinkleTrayWPF
{
    public partial class App : Application
    {
        private TaskbarIcon? _notifyIcon;
        private BrightnessFlyout? _flyout;

        protected override void OnStartup(StartupEventArgs e)
        {
            try 
            {
                base.OnStartup(e);

                // Prevent blank window
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Setup Tray Icon FIRST
                _notifyIcon = new TaskbarIcon();
                _notifyIcon.Icon = SystemIcons.Application;
                _notifyIcon.ToolTipText = "Twinkle Tray (WPF)";
                _notifyIcon.TrayLeftMouseDown += (s, ev) => ShowFlyout();

                // Start Engine SECOND (in case it takes time)
                Engine.Instance.Start();

                // Create flyout LAST
                _flyout = new BrightnessFlyout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup Error: {ex.Message}\n\n{ex.StackTrace}", "Twinkle Tray Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ShowFlyout()
        {
            try 
            {
                if (_flyout == null) return;

                _flyout.LoadMonitors();

                // Refresh position
                var desktopWorkingArea = SystemParameters.WorkArea;
                _flyout.Left = desktopWorkingArea.Right - _flyout.Width - 10;
                _flyout.Top = desktopWorkingArea.Bottom - _flyout.Height - 10;

                _flyout.Show();
                _flyout.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UI Error: {ex.Message}", "Twinkle Tray Error");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
