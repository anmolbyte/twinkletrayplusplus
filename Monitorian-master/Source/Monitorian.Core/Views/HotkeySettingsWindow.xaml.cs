using System;
using System.Linq;
using System.Windows;
using Monitorian.Core.Models;
using Monitorian.Core.Models.Hotkeys;

namespace Monitorian.Core.Views
{
	public partial class HotkeySettingsWindow : Window
	{
		private readonly AppControllerCore _controller;
		public SettingsCore Settings => _controller.Settings;

		public HotkeyActionType[] ActionTypes => (HotkeyActionType[])Enum.GetValues(typeof(HotkeyActionType));

		public System.Collections.Generic.IEnumerable<object> AvailableMonitors
		{
			get
			{
				yield return new { Id = "*", Name = "All Monitors" };
				foreach (var m in _controller.Monitors)
				{
					yield return new { Id = m.DeviceInstanceId, Name = m.Name };
				}
			}
		}

		public HotkeySettingsWindow(AppControllerCore controller)
		{
			InitializeComponent();
			this._controller = controller ?? throw new ArgumentNullException(nameof(controller));
			this.DataContext = this;
		}

		private void AddHotkey_Click(object sender, RoutedEventArgs e)
		{
			var captureWindow = new HotkeyCaptureWindow { Owner = this };
			if (captureWindow.ShowDialog() == true)
			{
				var newAction = new HotkeyAction
				{
					DisplayText = captureWindow.HotkeyDisplayText,
					Modifiers = captureWindow.SelectedModifiers,
					VirtualKey = captureWindow.SelectedKey,
					TargetMonitorId = "*", // Default to All
					ActionType = HotkeyActionType.BrightnessOffset,
					Value = 5
				};

				Settings.Hotkeys.Add(newAction);
			}
		}

		private void DeleteHotkey_Click(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement element && element.DataContext is HotkeyAction action)
			{
				Settings.Hotkeys.Remove(action);
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
