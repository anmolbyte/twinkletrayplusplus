using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Monitorian.Core.Helper;

namespace Monitorian.Core.Views
{
	public partial class HotkeyCaptureWindow : Window
	{
		public uint SelectedModifiers { get; private set; }
		public uint SelectedKey { get; private set; }
		public bool Success { get; private set; }
		public string HotkeyDisplayText { get; private set; }

		public HotkeyCaptureWindow()
		{
			InitializeComponent();
			this.PreviewKeyDown += OnPreviewKeyDown;
			this.Loaded += (s, e) =>
			{
				this.Focus();
				Keyboard.Focus(this);
				UpdateDisplay();
			};
		}

		private void UpdateDisplay()
		{
			var modifierStrings = new System.Collections.Generic.List<string>();

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
				modifierStrings.Add("Ctrl");
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
				modifierStrings.Add("Alt");
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
				modifierStrings.Add("Shift");
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows))
				modifierStrings.Add("Win");

			if (modifierStrings.Count > 0)
				CapturedText.Text = string.Join(" + ", modifierStrings) + " + ...";
			else
				CapturedText.Text = "Press Ctrl/Alt/Shift/Win + key";
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;

			Key key = e.Key;
			if (key == Key.System)
				key = e.SystemKey;

			if (key == Key.Escape)
			{
				this.DialogResult = false;
				this.Close();
				return;
			}

			uint modifiers = 0;
			var modifierStrings = new System.Collections.Generic.List<string>();

			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
			{
				modifiers |= GlobalHotkeyManager.MOD_CONTROL;
				modifierStrings.Add("Ctrl");
			}
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
			{
				modifiers |= GlobalHotkeyManager.MOD_ALT;
				modifierStrings.Add("Alt");
			}
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				modifiers |= GlobalHotkeyManager.MOD_SHIFT;
				modifierStrings.Add("Shift");
			}
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows))
			{
				modifiers |= GlobalHotkeyManager.MOD_WIN;
				modifierStrings.Add("Win");
			}

			if (modifiers == 0)
			{
				UpdateDisplay();
				return;
			}

			if (key == Key.LeftCtrl || key == Key.RightCtrl ||
				key == Key.LeftAlt || key == Key.RightAlt ||
				key == Key.LeftShift || key == Key.RightShift ||
				key == Key.LWin || key == Key.RWin ||
				key == Key.Escape)
			{
				UpdateDisplay();
				return;
			}

			uint vk = (uint)KeyInterop.VirtualKeyFromKey(key);
			if (vk == 0)
			{
				UpdateDisplay();
				return;
			}

			SelectedModifiers = modifiers;
			SelectedKey = vk;

			string keyName = GetKeyName(key);
			modifierStrings.Add(keyName);
			HotkeyDisplayText = string.Join(" + ", modifierStrings);

			Success = true;
			this.DialogResult = true;
			this.Close();
		}

		private string GetKeyName(Key key)
		{
			return key switch
			{
				Key.Left => "Left",
				Key.Right => "Right",
				Key.Up => "Up",
				Key.Down => "Down",
				Key.Prior => "PageUp",
				Key.Next => "PageDown",
				Key.Home => "Home",
				Key.End => "End",
				Key.Insert => "Insert",
				Key.Delete => "Delete",
				Key.Divide => "NumPad/",
				Key.Multiply => "NumPad*",
				Key.Subtract => "NumPad-",
				Key.Add => "NumPad+",
				Key.Decimal => "NumPad.",
				Key.NumPad0 => "NumPad0",
				Key.NumPad1 => "NumPad1",
				Key.NumPad2 => "NumPad2",
				Key.NumPad3 => "NumPad3",
				Key.NumPad4 => "NumPad4",
				Key.NumPad5 => "NumPad5",
				Key.NumPad6 => "NumPad6",
				Key.NumPad7 => "NumPad7",
				Key.NumPad8 => "NumPad8",
				Key.NumPad9 => "NumPad9",
				_ => key.ToString()
			};
		}
	}
}
