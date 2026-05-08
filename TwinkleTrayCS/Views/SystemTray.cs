using System;
using System.Drawing;
using System.Windows.Forms;
using TwinkleTrayCS.Worker;

namespace TwinkleTrayCS.Views
{
    public class SystemTray : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;

        public SystemTray()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Settings", null, OnSettingsClick);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("Quit", null, OnQuitClick);

            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Replace with actual logo later
                ContextMenuStrip = _contextMenu,
                Text = "Twinkle Tray C#",
                Visible = true
            };

            _notifyIcon.MouseClick += OnIconMouseClick;
        }

        private void OnIconMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show flyout panel
                ShowFlyout();
            }
        }

        private void ShowFlyout()
        {
            var flyout = new BrightnessFlyout();
            flyout.Show();
        }

        private void OnSettingsClick(object? sender, EventArgs e)
        {
            // Show settings form
            var settings = new SettingsForm();
            settings.Show();
        }

        private void OnQuitClick(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }
}
