using System.Windows.Controls;
using System.Windows;

namespace TwinkleTrayWPF.Views
{
    public partial class MonitorSlider : UserControl
    {
        public event EventHandler<int>? BrightnessChanged;
        private string _monitorId;

        public MonitorSlider(string id, string name, int brightness)
        {
            InitializeComponent();
            _monitorId = id;
            MonitorNameLabel.Text = name;
            BrightnessSlider.Value = brightness;
            BrightnessValue.Text = brightness.ToString();

            BrightnessSlider.ValueChanged += (s, e) =>
            {
                int val = (int)BrightnessSlider.Value;
                if (BrightnessValue.Text != val.ToString())
                {
                    BrightnessValue.Text = val.ToString();
                    BrightnessChanged?.Invoke(this, val);
                }
            };

            BrightnessValue.TextChanged += (s, e) =>
            {
                if (int.TryParse(BrightnessValue.Text, out int val))
                {
                    val = Math.Clamp(val, 0, 100);
                    if (BrightnessSlider.Value != val)
                    {
                        BrightnessSlider.Value = val;
                        BrightnessChanged?.Invoke(this, val);
                    }
                }
            };
        }
    }
}
