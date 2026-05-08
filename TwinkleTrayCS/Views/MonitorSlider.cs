using System;
using System.Drawing;
using System.Windows.Forms;

namespace TwinkleTrayCS.Views
{
    public class MonitorSlider : UserControl
    {
        private Label _nameLabel;
        private TrackBar _trackBar;
        private NumericUpDown _numericUpDown;
        private string _monitorId;

        public event EventHandler<int>? BrightnessChanged;

        public MonitorSlider(string monitorId, string name, int currentBrightness)
        {
            _monitorId = monitorId;
            this.Dock = DockStyle.Top;
            this.Height = 80;
            this.Padding = new Padding(10);

            _nameLabel = new Label
            {
                Text = name,
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var sliderPanel = new Panel { Dock = DockStyle.Fill };

            _trackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = Math.Clamp(currentBrightness, 0, 100),
                Dock = DockStyle.Fill,
                TickStyle = TickStyle.None
            };
            _trackBar.ValueChanged += (s, e) => {
                _numericUpDown.Value = _trackBar.Value;
                BrightnessChanged?.Invoke(this, _trackBar.Value);
            };

            _numericUpDown = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = Math.Clamp(currentBrightness, 0, 100),
                Width = 50,
                Dock = DockStyle.Right
            };
            _numericUpDown.ValueChanged += (s, e) => {
                _trackBar.Value = (int)_numericUpDown.Value;
            };

            sliderPanel.Controls.Add(_trackBar);
            sliderPanel.Controls.Add(_numericUpDown);

            this.Controls.Add(sliderPanel);
            this.Controls.Add(_nameLabel);
        }
    }
}
