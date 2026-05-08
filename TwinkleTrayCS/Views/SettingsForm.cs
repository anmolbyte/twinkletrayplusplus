using System;
using System.Drawing;
using System.Windows.Forms;
using TwinkleTrayCS.Worker;
using TwinkleTrayCS.Helpers;
using TwinkleTrayCS.Models;

namespace TwinkleTrayCS.Views
{
    public class SettingsForm : Form
    {
        private TabControl _tabControl;
        private TabPage _generalPage;
        private TabPage _monitorsPage;
        private TabPage _hotkeysPage;

        public SettingsForm()
        {
            this.Text = "Twinkle Tray C# Settings";
            this.Size = new Size(850, 650);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9);

            _tabControl = new TabControl { Dock = DockStyle.Fill };
            
            _generalPage = new TabPage("General") { BackColor = Color.FromArgb(32, 32, 32) };
            _monitorsPage = new TabPage("Monitors") { BackColor = Color.FromArgb(32, 32, 32) };
            _hotkeysPage = new TabPage("Hotkeys") { BackColor = Color.FromArgb(32, 32, 32) };

            _tabControl.TabPages.Add(_generalPage);
            _tabControl.TabPages.Add(_monitorsPage);
            _tabControl.TabPages.Add(_hotkeysPage);

            this.Controls.Add(_tabControl);

            SetupGeneralPage();
            SetupMonitorsPage();
            SetupHotkeysPage();
        }

        private void SetupGeneralPage()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(20) };

            var startupCb = new CheckBox 
            { 
                Text = "Launch at startup", 
                Checked = AppStartupHelper.IsStartupEnabled(),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            startupCb.CheckedChanged += (s, e) => AppStartupHelper.SetStartup(startupCb.Checked);

            var brightnessStartupCb = new CheckBox
            {
                Text = "Apply brightness at startup",
                Checked = Engine.Instance.Settings.BrightnessAtStartup,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            brightnessStartupCb.CheckedChanged += (s, e) => {
                Engine.Instance.Settings.BrightnessAtStartup = brightnessStartupCb.Checked;
                Engine.Instance.SaveSettings();
            };

            panel.Controls.Add(startupCb);
            panel.Controls.Add(brightnessStartupCb);
            _generalPage.Controls.Add(panel);
        }

        private void SetupMonitorsPage()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(20), AutoScroll = true };

            foreach (var monitor in Engine.Instance.Monitors)
            {
                var group = new GroupBox { Text = monitor.Name, Width = 750, Height = 100, ForeColor = Color.White };
                var renameLabel = new Label { Text = "Rename:", Location = new Point(10, 25), AutoSize = true };
                var renameBox = new TextBox { Location = new Point(100, 22), Width = 200, Text = Engine.Instance.Settings.Names.GetValueOrDefault(monitor.Id, "") };
                renameBox.TextChanged += (s, e) => {
                    Engine.Instance.Settings.Names[monitor.Id] = renameBox.Text;
                    Engine.Instance.SaveSettings();
                };

                group.Controls.Add(renameLabel);
                group.Controls.Add(renameBox);
                panel.Controls.Add(group);
            }

            _monitorsPage.Controls.Add(panel);
        }

        private void SetupHotkeysPage()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(20), AutoScroll = true };

            var addBtn = new Button { Text = "Add Hotkey", AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            addBtn.Click += (s, e) => {
                // Placeholder for adding hotkey
                MessageBox.Show("Hotkey recording UI goes here.");
            };

            panel.Controls.Add(addBtn);

            foreach (var hotkey in Engine.Instance.Settings.Hotkeys)
            {
                var lbl = new Label { Text = $"{hotkey.Accelerator} -> {hotkey.Actions[0].Type} {hotkey.Actions[0].Value}", AutoSize = true, ForeColor = Color.White };
                panel.Controls.Add(lbl);
            }

            _hotkeysPage.Controls.Add(panel);
        }
    }
}
