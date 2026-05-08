using System.Windows;
using TwinkleTrayWPF.Worker;

namespace TwinkleTrayWPF.Views
{
    public partial class BrightnessFlyout : Window
    {
        public BrightnessFlyout()
        {
            InitializeComponent();
            this.Deactivated += (s, e) => this.Hide();
        }

        public void LoadMonitors()
        {
            MonitorList.Children.Clear();
            foreach (var monitor in Engine.Instance.Monitors)
            {
                var slider = new MonitorSlider(monitor.Id, monitor.Name, monitor.Brightness);
                slider.BrightnessChanged += (s, val) =>
                {
                    Engine.Instance.SetBrightness(monitor.Id, val);
                };
                MonitorList.Children.Add(slider);
            }
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            // Open settings
        }
    }
}
