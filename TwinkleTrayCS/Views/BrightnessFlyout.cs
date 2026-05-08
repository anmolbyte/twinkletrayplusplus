using System;
using System.Drawing;
using System.Windows.Forms;
using TwinkleTrayCS.Worker;
using TwinkleTrayCS.Helpers;

namespace TwinkleTrayCS.Views
{
    public class BrightnessFlyout : Form
    {
        public BrightnessFlyout()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            this.Size = new Size(350, 0); // Height dynamic
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.Manual;

            LoadMonitors();
            AdjustPosition();

            this.Deactivate += (s, e) => this.Close();
        }

        private void LoadMonitors()
        {
            Engine.Instance.RefreshMonitors();
            int totalHeight = 20;

            foreach (var monitor in Engine.Instance.Monitors)
            {
                var slider = new MonitorSlider(monitor.Id, monitor.Name, monitor.Brightness);
                slider.BrightnessChanged += (s, level) => {
                    Engine.Instance.SetBrightness(monitor.Id, level);
                };
                this.Controls.Add(slider);
                totalHeight += slider.Height;
            }

            this.Height = totalHeight;
        }

        private void AdjustPosition()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                workingArea.Right - this.Width - 10,
                workingArea.Bottom - this.Height - 10
            );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
        }
    }
}
