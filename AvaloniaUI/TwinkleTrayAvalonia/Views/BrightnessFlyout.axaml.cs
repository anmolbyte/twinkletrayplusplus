using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using TwinkleTrayAvalonia.Worker;

namespace TwinkleTrayAvalonia.Views
{
    public partial class BrightnessFlyout : Window
    {
        public BrightnessFlyout()
        {
            InitializeComponent();
            this.Deactivated += (s, e) => this.Hide();
            LoadMonitors();
        }

        public void LoadMonitors()
        {
            var panel = this.FindControl<StackPanel>("MonitorList");
            if (panel == null) return;
            panel.Children.Clear();

            foreach (var monitor in Engine.Instance.Monitors)
            {
                var slider = new MonitorSlider(monitor.Id, monitor.Name, monitor.Brightness);
                slider.BrightnessChanged += (s, val) =>
                {
                    Engine.Instance.SetBrightness(monitor.Id, val);
                };
                panel.Children.Add(slider);
            }
        }

        private void OnSettingsClick(object? sender, RoutedEventArgs e)
        {
            // Open settings window here
            this.Hide();
        }
    }
}
