using Avalonia.Controls;
using System;

namespace TwinkleTrayAvalonia.Views
{
    public partial class MonitorSlider : UserControl
    {
        public event EventHandler<int>? BrightnessChanged;
        private string _monitorId = "";

        public MonitorSlider()
        {
            InitializeComponent();
        }

        public MonitorSlider(string id, string name, int brightness) : this()
        {
            _monitorId = id;
            var nameLabel = this.FindControl<TextBlock>("MonitorNameLabel");
            if (nameLabel != null) nameLabel.Text = name;

            var slider = this.FindControl<Slider>("BrightnessSlider");
            var num = this.FindControl<NumericUpDown>("BrightnessValue");

            if (slider != null && num != null)
            {
                slider.Value = brightness;
                num.Value = brightness;

                slider.PropertyChanged += (s, e) =>
                {
                    if (e.Property.Name == "Value")
                    {
                        int val = (int)slider.Value;
                        num.Value = val;
                        BrightnessChanged?.Invoke(this, val);
                    }
                };

                num.PropertyChanged += (s, e) =>
                {
                    if (e.Property.Name == "Value")
                    {
                        int val = (int)(num.Value ?? 0);
                        slider.Value = val;
                        BrightnessChanged?.Invoke(this, val);
                    }
                };
            }
        }
    }
}
