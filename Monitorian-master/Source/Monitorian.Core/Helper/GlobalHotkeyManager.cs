using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Monitorian.Core.Models.Hotkeys;

namespace Monitorian.Core.Helper;

/// <summary>
/// Manages global hotkey registration and message dispatch using Win32 RegisterHotKey.
/// Uses a hidden HwndSource window to receive WM_HOTKEY messages without needing
/// a visible window.
/// </summary>
public class GlobalHotkeyManager : IDisposable
{
	#region Win32

	private const int WM_HOTKEY = 0x0312;

	// Modifier flags
	public const uint MOD_ALT = 0x0001;
	public const uint MOD_CONTROL = 0x0002;
	public const uint MOD_SHIFT = 0x0004;
	public const uint MOD_WIN = 0x0008;
	public const uint MOD_NOREPEAT = 0x4000;

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

	#endregion

	private HwndSource _hwndSource;
	private readonly List<HotkeyAction> _registered = new();
	private int _nextId = 9000; // start high to avoid conflicts

	/// <summary>
	/// Fired on the thread that created this manager (the UI thread) when a registered hotkey is pressed.
	/// </summary>
	public event Action<HotkeyAction> HotkeyTriggered;

	public GlobalHotkeyManager()
	{
		// Create a message-only hidden window
		var parameters = new HwndSourceParameters("GlobalHotkeyManager")
		{
			HwndSourceHook = WndProc,
			ParentWindow = new IntPtr(-3), // HWND_MESSAGE
			Width = 0,
			Height = 0,
			WindowStyle = 0,
		};
		_hwndSource = new HwndSource(parameters);
	}

	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == WM_HOTKEY)
		{
			int id = wParam.ToInt32();
			var action = _registered.Find(a => a.RegisteredId == id);
			if (action != null)
			{
				HotkeyTriggered?.Invoke(action);
				handled = true;
			}
		}
		return IntPtr.Zero;
	}

	/// <summary>
	/// Registers all hotkeys from the given collection. Unregisters any previously registered ones first.
	/// </summary>
	public void RegisterAll(IEnumerable<HotkeyAction> actions)
	{
		UnregisterAll();

		foreach (var action in actions)
		{
			if (action.VirtualKey == 0)
				continue;

			int id = _nextId++;
			bool ok = RegisterHotKey(_hwndSource.Handle, id, action.Modifiers | MOD_NOREPEAT, action.VirtualKey);
			if (ok)
			{
				action.RegisteredId = id;
				_registered.Add(action);
			}
		}
	}

	/// <summary>
	/// Unregisters all currently registered hotkeys.
	/// </summary>
	public void UnregisterAll()
	{
		foreach (var action in _registered)
		{
			UnregisterHotKey(_hwndSource.Handle, action.RegisteredId);
			action.RegisteredId = -1;
		}
		_registered.Clear();
	}

	#region IDisposable

	private bool _disposed;

	public void Dispose()
	{
		if (_disposed) return;
		UnregisterAll();
		_hwndSource?.Dispose();
		_hwndSource = null;
		_disposed = true;
	}

	#endregion
}
